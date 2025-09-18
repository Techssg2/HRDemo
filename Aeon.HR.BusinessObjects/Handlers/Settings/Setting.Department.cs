using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Constants;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using EFSecondLevelCache;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        #region Static Variables
        private static string STR_DeparmentPreFix_CacheKey = "Department_";
        private static string STR_AllDeparment_CacheKey = STR_DeparmentPreFix_CacheKey + "AllListDepartment";
        private static string STR_AllDepartmentTreeViewModel_CacheKey = STR_DeparmentPreFix_CacheKey + "AllDepartmentTreeViewModel";
        private static IEnumerable<DepartmentTreeViewModel> AllDepartments = null;
        private static bool _ApplyChildSMMDMMD = false;
        private static object _lock = new object();
        #endregion
        /// <summary>
        /// Đệ quy trả về tất cả Id trong 1 node
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public IEnumerable<DepartmentTreeViewModel> DescendantsAndSelf(DepartmentTreeViewModel dp)
        {
            //List<DepartmentTreeViewModel> rs = new List<DepartmentTreeViewModel>();
            //rs.Add(dp);
            //foreach (var item in dp.Items.SelectMany(x => DescendantsAndSelf(x)))
            //{
            //     rs.Add(item);
            //}
            //return rs;
            yield return dp;
            foreach (var item in dp.Items.SelectMany(x => DescendantsAndSelf(x)))
            {
                yield return item;
            }
        }
        /// <summary>
        /// Kiểm tra parent của department được update, tránh circle reference
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public async Task<bool> HasDepartmentCircle(Guid departmentId, Guid parentId)
        {
            if (departmentId == parentId)
                return true;

            // Lấy toàn bộ departments một lần duy nhất
            var departments = (await _uow.GetRepository<Department>()
                .FindByAsync<DepartmentTreeViewModel>(x => true))
                .ToDictionary(x => x.Id, x => x.ParentId);

            // Truy ngược từ parentId về gốc
            Guid? current = parentId;

            while (current != null && departments.ContainsKey(current.Value))
            {
                if (departments[current.Value] == departmentId)
                    return true;

                current = departments[current.Value];
            }
            return false;
        }


        #region Department Management
        public async Task<ResultDTO> GetDepartmentByCode(string deptCode)
        {
            var department = await _uow.GetRepository<Department>().GetSingleAsync<DepartmentViewModel>(x => x.Code == deptCode);
            return new ResultDTO { Object = department, };

        }
        public async Task<ResultDTO> CreateDepartment(DepartmentArgs model)
        {
            var existing = await _uow.GetRepository<Department>().FindByAsync(x => x.Code == model.Code);
            if (existing.Any())
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { $"Code {model.Code} has existed in Department list" } };
            }
            else
            {
                if (!string.IsNullOrEmpty(model.SAPCode))
                {
                    var existSAPCode = await _uow.GetRepository<Department>().FindByAsync(x => x.SAPCode == model.SAPCode);
                    if (existSAPCode.Any()) return new ResultDTO { ErrorCodes = { 1001 }, Messages = { $"SAPCode {model.SAPCode} has existed in Department list" } };
                }
                model.Id = Guid.NewGuid();
                var department = Mapper.Map<Department>(model);
                if (model.ParentId.HasValue)
                {
                    var parentDepartment = await _uow.GetRepository<Department>().FindByIdAsync(model.ParentId.Value);
                    if (parentDepartment == null)
                    {
                        return new ResultDTO
                        {
                            ErrorCodes = { 2 },
                            Messages = { "Parent department not found" },
                        };
                    }
                }
                //else
                //{
                //    // Chỉ thêm được 1 department không có department cha
                //    bool hasExistingParent = await _uow.GetRepository<Department>().AnyAsync(x => x.ParentId == null || x.ParentId == x.Id);
                //    if (hasExistingParent)
                //    {
                //        return new ResultDTO
                //        {
                //            ErrorCodes = { 3003 },
                //            Messages = { "Department parent: field is required" },
                //        };
                //    }
                //}
                department.HasTrackingLog = true;
                if (model.BusinessModelId.HasValue)
                {
                    department.BusinessModelId = model.BusinessModelId.Value;
                }

                var parentIsCheckApplyChildSMMDMMD = FindParentIsCheckApplyChildSMMDMMD(department);
                if (parentIsCheckApplyChildSMMDMMD != null)
                {
                    department.IsMMD = parentIsCheckApplyChildSMMDMMD.IsMMD;
                    department.IsSM = parentIsCheckApplyChildSMMDMMD.IsSM;
                    department.IsMD = parentIsCheckApplyChildSMMDMMD.IsMD;
                }

                _uow.GetRepository<Department>().Add(department);
                await _uow.CommitAsync();
                _dashboardBO.ClearNode();

                ResetAllDepartmentCache();
                try
                {
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(Mapper.Map<DepartmentViewModel>(department)),
                        ItemId = department.Id,
                        ItemType = ItemTypeContants.Department,
                        Type = TrackingHistoryTypeContants.Create,
                        ItemRefereceNumberOrCode = department.Code,
                    });
                    await _uow.CommitAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError("Error: " + e.Message);
                }
                
                #region Clear cache module Department IT
                try
                {
                    await ClearCacheIT();
                }
                catch (Exception e)
                {
                    _logger.LogError("[Clear cache module Department HR] - Error: " + e.Message);
                }
                #endregion
            }
            return new ResultDTO { };
        }

        private Department FindParentIsCheckApplyChildSMMDMMD(Department department)
        {
            if (department.ParentId.HasValue)
            {
                var parentDepartment = _uow.GetRepository<Department>().GetSingle(x => x.Id == department.ParentId);
                if (parentDepartment.ApplyChildSMMDMMD)
                {
                    return parentDepartment;
                }
                else
                {
                    return FindParentIsCheckApplyChildSMMDMMD(parentDepartment);
                }
            }
            return null;
        }


        public async Task<ResultDTO> UpdateChildDepartment(IEnumerable<Department> childDepartments, Guid RegionId, bool isSM, bool isMD, bool isMMD)
        {
            foreach (var childDepartment in childDepartments)
            {
                childDepartment.RegionId = RegionId;
                if (_ApplyChildSMMDMMD)
                {
                    childDepartment.IsSM = isSM;
                    childDepartment.IsMD = isMD;
                    childDepartment.IsMMD = isMMD;
                }
                _uow.GetRepository<Department>().Update(childDepartment);
                var newChildDepartments = await _uow.GetRepository<Department>().FindByAsync(x => x.ParentId == childDepartment.Id);
                if (null != newChildDepartments && newChildDepartments.Count() > 0)
                {
                    await UpdateChildDepartment(newChildDepartments, RegionId, isSM, isMD, isMMD);
                }
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> UpdateDepartment(DepartmentArgs model)
        {
            var existing = await _uow.GetRepository<Department>().AnyAsync(x => x.Code == model.Code && x.Id != model.Id);
            if (existing)
            {
                return new ResultDTO
                {
                    ErrorCodes = { 1 },
                    Messages = { $"Code {model.Code} has existed in Department list" },
                };
            }
            else
            {
                if (!string.IsNullOrEmpty(model.SAPCode))
                {
                    var existSAPCode = await _uow.GetRepository<Department>().AnyAsync(x => x.SAPCode == model.SAPCode && x.Id != model.Id);
                    if (existSAPCode) return new ResultDTO { ErrorCodes = { 1 }, Messages = { $"SAPCode {model.SAPCode} has existed in Department list" } };
                }
                var department = await _uow.GetRepository<Department>().FindByIdAsync(model.Id.Value);
                if (model.ParentId.HasValue)
                {
                    if (department.Id == model.ParentId)
                    {
                        // chỉ được có 1 department có parent là chính nó, là department cao nhất mà thôi
                        var existingHighest = await _uow.GetRepository<Department>()
                            .AnyAsync(x => x.Id == x.ParentId && x.Id != model.Id.Value);
                        var existingEmptyParent = await _uow.GetRepository<Department>()
                            .AnyAsync(x => x.ParentId == null && x.Id != model.Id.Value);
                        if (existingHighest || existingEmptyParent)
                        {
                            return new ResultDTO
                            {
                                ErrorCodes = { 1 },
                                Messages = { "Can have only one highest department that have parent department is itself" },
                            };
                        }
                    }
                    var parentDepartment = await _uow.GetRepository<Department>().FindByIdAsync(model.ParentId.Value);
                    if (parentDepartment == null)
                    {
                        return new ResultDTO
                        {
                            ErrorCodes = { 2 },
                            Messages = { "Parent department not found" },
                        };
                    }
                    if (parentDepartment.ParentId == department.Id && department.Id != parentDepartment.Id)
                    {
                        return new ResultDTO
                        {
                            ErrorCodes = { 3 },
                            Messages = { "2 Department can not be parent of each other." },
                        };
                    }
                    //Kiểm tra circle reference trong quan hệ department parent
                    bool hasCircleReference = await HasDepartmentCircle(model.Id.Value, model.ParentId.Value);
                    if (hasCircleReference)
                    {
                        return new ResultDTO
                        {
                            ErrorCodes = { 4 },
                            Messages = { "The higher department can not be child of the lower department." },
                        };
                    }
                }
                else
                {
                    // Chỉ thêm được 1 department không có department cha
                    ///bool hasExistingParent = await _uow.GetRepository<Department>().AnyAsync(x => ((x.ParentId == null || x.ParentId == x.Id) && x.Id != department.Id ));
                    var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync(department.Id);
                    if (currentDepartment.ParentId != null)
                    {
                        return new ResultDTO
                        {
                            ErrorCodes = { 3003 },
                            Messages = { "Department parent: field is required" },
                        };
                    }
                }
                var departmentBeforeUpdate = Mapper.Map<DepartmentViewModel>(department);

                Mapper.Map(model, department);
                department.ParentId = model.ParentId;
                department.JobGradeId = model.JobGradeId.Value;
                department.Name = model.Name;
                department.Code = model.Code;
                department.PositionCode = model.PositionCode;
                department.PositionName = model.PositionName;
                department.SAPCode = model.SAPCode;
                department.Type = model.Type;
                department.Color = model.Color;
                department.Modified = DateTime.Now;
                department.HrDepartmentId = model.HrDepartmentId;
                department.IsHR = model.IsHR;
                department.IsCB = model.IsCB;
                department.IsAdmin = model.IsAdmin;
                department.IsPerfomance = model.IsPerfomance;
                department.IsStore = model.IsStore;
                department.CostCenterRecruitmentId = model.CostCenterRecruitmentId;
                department.IsFacility = model.IsFacility;
                department.RegionId = model.RegionId;
                department.EnableForPromoteActing = model.EnableForPromoteActing;
                department.IsAcademy = model.IsAcademy;
                department.Note = model.Note;

                department.IsMD = model.IsMD;
                department.IsSM = model.IsSM;
                department.IsMMD = model.IsMMD;
                department.ApplyChildSMMDMMD = model.ApplyChildSMMDMMD;
                if (model.IsEdoc)
                    department.IsPrepareDelete = false;
                
                _ApplyChildSMMDMMD = model.ApplyChildSMMDMMD;
                if (model.BusinessModelId.HasValue)
                {
                    department.BusinessModelId = model.BusinessModelId.Value;
                }
                _uow.GetRepository<Department>().Update(department);

                //Get child current department and update region
                var childDepartments = await _uow.GetRepository<Department>().FindByAsync(x => x.ParentId == model.Id.Value);

                if (null != childDepartments && childDepartments.Count() > 0)
                {
                    await UpdateChildDepartment(childDepartments, model.RegionId.Value, model.IsSM, model.IsMD, model.IsMMD);
                    // update isMD to child
                }

                try
                {
                    department.HasTrackingLog = true;
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(departmentBeforeUpdate),
                        ItemId = department.Id,
                        ItemName = department.Name,
                        ItemType = ItemTypeContants.Department,
                        Type = TrackingHistoryTypeContants.Update,
                        ItemRefereceNumberOrCode = department.Code,
                    });
                }
                catch (Exception e)
                {
                    _logger.LogError("Error: " + e.Message);
                }

                await _uow.CommitAsync();
                #endregion
                _dashboardBO.ClearNode();
                ResetAllDepartmentCache();
                #region Clear cache module Department HR
                try
                {
                    await ClearCacheIT();
                }
                catch (Exception e)
                {
                    _logger.LogError("[Clear cache module Department HR] - Error: " + e.Message);
                }
            }
            return new ResultDTO { };
        }
        
        public async Task ClearCacheIT()
        {
            var client = new HttpClient();
            string domain = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
            client.BaseAddress = new Uri(domain);
            var response = await client.GetAsync("it/api/Partner/ClearCacheDepartment"); // KHÔNG có dấu '/' đầu
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Cache department cleared successfully: {Content}", content);
            }
            else
            {
                _logger.LogError("Failed to clear cache department. Status code: {StatusCode}", response.StatusCode);
            }
        }
        
        public async Task<ResultDTO> DeleteDepartment(Guid Id)
        {
            var existing = await _uow.GetRepository<Department>().FindByAsync(x => x.Id == Id);
            if (!existing.Any())
            {
                return new ResultDTO { ErrorCodes = { 111 }, Messages = { "Not found department with id " + Id }, };
            }
            else
            {
                //Nếu đang có department con thì không xóa được.
                bool hasChildDepartment = await _uow.GetRepository<Department>().AnyAsync(x => x.Id != Id && x.ParentId == Id);
                if (hasChildDepartment)
                {
                    return new ResultDTO
                    {
                        ErrorCodes = { 222 },
                        Messages = { "Exist Sub-Department of this Department.<br /> You have to delete Sub-Department first" },
                    };
                }
                else
                {
                    var department = await _uow.GetRepository<Department>().FindByIdAsync(Id);
                    var departmentMappings = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.DepartmentId == department.Id);
                    if (departmentMappings.Any())
                    {
                        foreach (var mapping in departmentMappings)
                        {
                            mapping.DepartmentId = null;
                            mapping.IsHeadCount = false;
                        }
                    }
                    department.IsDeleted = true;

                    // Delete User Submit Person && delete line
                    if (null != Id)
                    {
                        var userSubmitPersion = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.DepartmentId == Id);
                        if (userSubmitPersion.Any())
                        {
                            foreach (var submitPerson in userSubmitPersion)
                                _uow.GetRepository<UserSubmitPersonDeparmentMapping>().Delete(submitPerson);
                        }
                    }

                    try
                    {
                        await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                        {
                            DataStr = JsonConvert.SerializeObject(Mapper.Map<DepartmentViewModel>(department)),
                            ItemId = department.Id,
                            ItemType = ItemTypeContants.Department,
                            Type = TrackingHistoryTypeContants.Delete,
                            ItemRefereceNumberOrCode = department.Code,
                        });
                        await _uow.CommitAsync();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Error: " + e.Message);
                    }

                    await _uow.CommitAsync();
                    _dashboardBO.ClearNode();
                    ResetAllDepartmentCache();
                    
                    #region Clear cache module Department IT
                    try
                    {
                        await ClearCacheIT();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("[Clear cache module Department IT] - Error: " + e.Message);
                    }
                    #endregion
                }
            }
            return new ResultDTO { };
        }

        public async Task<ArrayResultDTO> GetAllListDepartments()
        {
            try
            {
                var items = await _uow.GetRepository<Department>().FindByAsync<NavigationListUserDepartmentViewModel.Department>(x => !x.IsDeleted);
                var count = await _uow.GetRepository<Department>().CountAsync(x => !x.IsDeleted);
                var result = new ArrayResultDTO { Data = items, Count = count };
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<ResultDTO> GetDepartments(QueryArgs args)
        {
            var departmentList = await _uow.GetRepository<Department>().FindByAsync<ItemListDepartmentViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters, x => x.JobGrade);

            var departmentIds = departmentList.Select(d => d.Id).ToList();
            var itDepartments = new List<ITDepartmentDTO>();

            if (departmentIds.Any())
            {
                try
                {
                    var parameterNames = departmentIds.Select((_, i) => $"@p{i}").ToList();
                    var sql = $"SELECT Id, IsEdoc1 FROM dbo.ITDepartments WHERE Id IN ({string.Join(",", parameterNames)})";
                    var sqlParams = departmentIds.Select((id, i) => new SqlParameter($"@p{i}", id)).ToArray();

                    var repo = _uow.GetRepository<Department>();
                    itDepartments = await repo.ExecuteRawSqlQueryAsync<ITDepartmentDTO>(sql, sqlParams);
                }
                catch (Exception ex)
                {
                    return new ResultDTO { };
                }
            }

            foreach (var item in departmentList)
            {
                item.UserCount = _uow.GetRepository<UserDepartmentMapping>().Count(y => y.DepartmentId == item.Id);
                item.HasUserPrepareDelete = _uow.GetRepository<UserDepartmentMapping>().Any(y => y.DepartmentId == item.Id && y.IsPrepareDelete);
                if (null != item.RegionId)
                {
                    var regionData = await _uow.GetRepository<Region>().GetSingleAsync(x => x.Id == item.RegionId);
                    item.RegionName = regionData.RegionName;
                }

                // Map IsEdoc1 from ITDepartment
                var itDepartment = itDepartments.FirstOrDefault(x => x.Id == item.Id);
                item.IsEdoc1 = itDepartment?.IsEdoc1;
            }
            var count = await _uow.GetRepository<Department>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = departmentList, Count = count };
            return new ResultDTO
            {
                Object = result,
            };
        }
        public async Task<ResultDTO> GetDetailDepartment(DepartmentArgs data)
        {
            var department = await _uow.GetRepository<Department>().FindByAsync<ItemListDepartmentViewModel>(x => x.Id == data.Id);
            if (department.Any())
            {
                return new ResultDTO { Object = department };
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> GetDepartmentTree()
        {
            var lstDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentTreeViewModel>(x => true);
            List<DepartmentTreeViewModel> departmentTree = new List<DepartmentTreeViewModel>();
            List<DepartmentTreeViewModel> vmLstDepartment = lstDepartment.OrderByDescending(x => x.JobGradeGrade).ToList();
            var highestDepartment = lstDepartment.Where(x => x.ParentId == x.Id).FirstOrDefault();
            if (highestDepartment != null)
            {
                vmLstDepartment.ForEach(x =>
                {
                    x.Items = vmLstDepartment.Where(y => y.ParentId == x.Id && y.Id != x.Id).OrderByDescending(k => k.Name).ToList();
                }
                );

                departmentTree = vmLstDepartment.Where(item => item.Id == highestDepartment.Id).ToList();
                departmentTree.AddRange(vmLstDepartment.Where(item => !item.ParentId.HasValue).ToList());
                ResultDTO result = new ResultDTO
                {
                    Object = new ArrayResultDTO
                    {
                        Data = departmentTree,
                        Count = 1,
                    },
                };
                return result;
            }
            else
            {
                vmLstDepartment.ForEach(x =>
                x.Items = vmLstDepartment.Where(y => y.ParentId == x.Id).OrderByDescending(k => k.Name).ToList()

                );
                departmentTree = vmLstDepartment.Where(item => !item.ParentId.HasValue).ToList();
                if (!departmentTree.Any())
                {
                    departmentTree = vmLstDepartment;
                }
                ResultDTO result = new ResultDTO
                {
                    Object = new ArrayResultDTO
                    {
                        Data = departmentTree,
                        Count = 1,
                    },
                };
                return result;
            }
        }
        public async Task<ResultDTO> GetDepartmentByFilter(QueryArgs args)
        {
            //var lstDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentTreeViewModel>(x => true);
            var lstDepartment = new List<DepartmentTreeViewModel>();
            IEnumerable<DepartmentTreeViewModel> vmLstDepartment = null;
            IEnumerable<DepartmentTreeViewModel> departmentTree = new List<DepartmentTreeViewModel>();
            ObjectCache cache = MemoryCache.Default;

            Func<Task<List<DepartmentTreeViewModel>>> Prepare_AllListDepartment = async delegate ()
            {
                List<DepartmentTreeViewModel> returnValue = new List<DepartmentTreeViewModel>();
                try
                {
                    var data = await _uow.GetRepository<Department>().FindByAsync<DepartmentTreeViewModel>(x => true);
                    returnValue = data.ToList();
                }
                catch
                {
                    returnValue = new List<DepartmentTreeViewModel>();
                }
                return returnValue;
            };


            lstDepartment = await Prepare_AllListDepartment();
            #region Local Func
            Func<object[], string> GetPredicateParameters = delegate (object[] predicateParameters)
            {
                string returnValue = "";
                try
                {
                    List<string> paramsList = args.PredicateParameters.Where(x => x != null).Select(x => x + "").ToList();
                    if (paramsList.Count == 1)
                    {
                        returnValue = paramsList[0];
                    }
                    else if (paramsList.Count > 1)
                    {
                        returnValue = paramsList.Aggregate((x, y) => x + "_" + y);
                    }
                }
                catch
                {
                    returnValue = "";
                }
                return returnValue;
            };

            Func<List<DepartmentTreeViewModel>> PrepareAllDeparmentInfo = delegate ()
            {
                List<DepartmentTreeViewModel> returnValue = new List<DepartmentTreeViewModel>();
                try
                {
                    var allDeparment_CacheObject = cache.Get(STR_AllDepartmentTreeViewModel_CacheKey);
                    if (allDeparment_CacheObject is null)
                    {
                        var departmentList = lstDepartment.ToList();

                        // Tối ưu: Tạo lookup cho children
                        var childrenLookup = departmentList
                            .Where(x => x.ParentId.HasValue)
                            .ToLookup(x => x.ParentId.Value);

                        // Xây dựng cây phân cấp (giống logic cũ)
                        foreach (var item in departmentList)
                        {
                            var children = childrenLookup[item.Id]
                                .OrderByDescending(x => x.Name)
                                .ToList();

                            if (children.Any())
                            {
                                item.Items = children;
                                returnValue.Add(item);
                            }
                        }

                        // Logic thêm các item còn thiếu (giống hệt code gốc)
                        var existItems = departmentList
                            .Where(x => !returnValue.Contains(x) &&
                                  returnValue.Any(y => y.Items != null && !y.Items.Contains(x)))
                            .ToList();

                        if (existItems.Any())
                        {
                            returnValue.AddRange(existItems);
                        }

                        if (!returnValue.Any())
                        {
                            returnValue = departmentList;
                        }

                        cache.Set(STR_AllDepartmentTreeViewModel_CacheKey, returnValue, DateTime.Now.AddHours(1));
                    }
                    else
                    {
                        returnValue = allDeparment_CacheObject as List<DepartmentTreeViewModel>;
                    }
                }
                catch
                {
                    returnValue = new List<DepartmentTreeViewModel>();
                }
                return returnValue;
            };
            #endregion

            string keyCache = $"{STR_DeparmentPreFix_CacheKey}{args.Predicate}{GetPredicateParameters(args.PredicateParameters)}";
            var cacheObj = cache.Get(keyCache) as IEnumerable<object>;

            if (cacheObj is null || !cacheObj.Any() || (!(cacheObj.GetPropertyValue("Count") is null) && cacheObj.GetPropertyValue("Count").ToString() == "0"))
            {
                List<DepartmentTreeViewModel> temp = PrepareAllDeparmentInfo();

                if (!string.IsNullOrEmpty(args.Predicate))
                {
                    vmLstDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentTreeViewModel>(args.Predicate, args.PredicateParameters);
                    if (vmLstDepartment != null && vmLstDepartment.Any())
                    {
                        var g9Department = vmLstDepartment.Any(x => !x.ParentId.HasValue);
                        if (g9Department)
                        {
                            departmentTree = lstDepartment.Where(x => !x.ParentId.HasValue && vmLstDepartment.Any(y => y.Id == x.Id));
                        }
                        else
                        {
                            var instances = temp.Where(x => !(x is null) && vmLstDepartment.Any(m => m.Id == x.Id) || x.Items.Any(y => vmLstDepartment.Any(k => k.Id == y.Id))).ToList();
                            departmentTree = instances.Where(x => vmLstDepartment.Any(y => y.Id == x.Id)).OrderByDescending(x => x.JobGradeGrade).ToList();
                        }
                    }
                }
                else
                {
                    departmentTree = temp.Where(x => !x.ParentId.HasValue);
                }

                cache.Set(keyCache, departmentTree, DateTime.Now.AddHours(1));
            }
            else
            {
                departmentTree = cacheObj as IEnumerable<DepartmentTreeViewModel>;
            }

            try
            {
                await UpdateIsEdoc1Async(departmentTree);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update IsEdoc1 for department tree in GetDepartmentByFilter. " + "DepartmentTree Count: {Count}", departmentTree?.Count() ?? 0);
            }

            ResultDTO result = new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Data = departmentTree,
                    Count = 1,
                },
            };
            return result;
        }

        public async Task<ResultDTO> GetDepartmentByFilterV2(QueryArgs args)
        {
            var result = new ResultDTO() { };
            var departments = await _uow.GetRepository<Department>().FindByAsync<ItemListDepartmentViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters, x => x.JobGrade);
            result.Object = departments;
            return result;
        }
        public async Task<ResultDTO> GetDepartmentTreeByGrade(Guid jobGradeId)
        {
            var jobGrade = await _uow.GetRepository<JobGrade>().FindByIdAsync(jobGradeId);
            if (jobGrade == null)
            {
                return new ResultDTO
                {
                    Object = new ArrayResultDTO
                    {
                        Data = new Object { },
                        Count = 0,
                    }
                };
            }
            else
            {
                var lstDepartment = await _uow.GetRepository<Department>().FindByAsync(x => x.JobGrade.Grade > jobGrade.Grade);
                List<DepartmentTreeViewModel> departmentTree = new List<DepartmentTreeViewModel>();
                List<DepartmentTreeViewModel> vmLstDepartment = Mapper.Map<List<DepartmentTreeViewModel>>(lstDepartment);
                var highestDepartment = vmLstDepartment.Where(x => x.ParentId == x.Id).FirstOrDefault();
                if (highestDepartment != null)
                {
                    vmLstDepartment.ForEach(x =>
                    {
                        x.Items = vmLstDepartment.Where(y => y.ParentId == x.Id && y.Id != x.Id).OrderByDescending(y => y.Name).ToList();
                    }
                    );

                    departmentTree = vmLstDepartment.Where(item => item.Id == highestDepartment.Id).ToList();
                    departmentTree.AddRange(vmLstDepartment.Where(item => !item.ParentId.HasValue).ToList());
                    ResultDTO result = new ResultDTO
                    {
                        Object = new ArrayResultDTO
                        {
                            Data = departmentTree,
                            Count = departmentTree.Count,
                        },
                    };
                    return result;
                }
                else
                {
                    vmLstDepartment.ForEach(x => x.Items = vmLstDepartment.OrderByDescending(y => y.Name)
                                                .Where(y => y.ParentId == x.Id)
                                                .ToList()
                                   );

                    departmentTree = vmLstDepartment.Where(item => !item.ParentId.HasValue).ToList();
                    ResultDTO result = new ResultDTO
                    {
                        Object = new ArrayResultDTO
                        {
                            Data = departmentTree,
                            Count = 1,
                        },
                    };
                    return result;
                }
            }
        }
        public async Task<ResultDTO> GetAllDeptLineByGrade(Guid jobGradeId)
        {
            var jobGrade = await _uow.GetRepository<JobGrade>().FindByIdAsync(jobGradeId);
            if (jobGrade == null || jobGrade.Grade >= 9 || jobGrade.Grade < 1)
            {
                return new ResultDTO
                {
                    Object = new ArrayResultDTO
                    {
                        Data = new Object { },
                        Count = 0,
                    }
                };
            }
            else
            {
                var lstDepartment = await _uow.GetRepository<Department>().FindByAsync(x => x.JobGrade.Grade >= jobGrade.Grade);
                List<DepartmentTreeViewModel> departmentTree = new List<DepartmentTreeViewModel>();
                List<DepartmentTreeViewModel> vmLstDepartment = Mapper.Map<List<DepartmentTreeViewModel>>(lstDepartment);
                var highestDepartment = vmLstDepartment.Where(x => x.ParentId == x.Id).FirstOrDefault();
                if (highestDepartment != null)
                {
                    vmLstDepartment.ForEach(x =>
                    {
                        x.Items = vmLstDepartment.Where(y => y.ParentId == x.Id && y.Id != x.Id).OrderByDescending(k => k.Name).ToList();
                    }
                    );

                    departmentTree = vmLstDepartment.Where(item => item.Id == highestDepartment.Id).ToList();
                    departmentTree.AddRange(vmLstDepartment.Where(item => !item.ParentId.HasValue).ToList());
                    ResultDTO result = new ResultDTO
                    {
                        Object = new ArrayResultDTO
                        {
                            Data = departmentTree,
                            Count = departmentTree.Count,
                        },
                    };
                    return result;
                }
                else
                {
                    vmLstDepartment.ForEach(x => x.Items = vmLstDepartment.OrderByDescending(k => k.Name)
                                                .Where(y => y.ParentId == x.Id)
                                                .ToList()
                                   );

                    departmentTree = vmLstDepartment.Where(item => !item.ParentId.HasValue).ToList();
                    ResultDTO result = new ResultDTO
                    {
                        Object = new ArrayResultDTO
                        {
                            Data = departmentTree,
                            Count = 1,
                        },
                    };
                    return result;
                }
            }
        }
        public async Task<ResultDTO> GetDepartmentTreeByType(Guid departmentId, DepartmentFilterEnum option)
        {
            var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentTreeViewModel>(departmentId);
            IEnumerable<DepartmentTreeViewModel> mappingDepartmentTrees = new List<DepartmentTreeViewModel>();
            if (option == DepartmentFilterEnum.OnlyDepartment)
            {
                mappingDepartmentTrees = await _uow.GetRepository<Department>().FindByAsync<DepartmentTreeViewModel>(x => x.JobGrade.Grade < currentDepartment.JobGradeGrade && x.JobGrade.Grade > 4);
            }
            else
            {
                mappingDepartmentTrees = await _uow.GetRepository<Department>().FindByAsync<DepartmentTreeViewModel>(x => x.JobGrade.Grade < currentDepartment.JobGradeGrade);
            }
            List<DepartmentTreeViewModel> departmentTree = new List<DepartmentTreeViewModel>();
            List<DepartmentTreeViewModel> vmLstDepartment = mappingDepartmentTrees.ToList();
            //highest department sẽ là department hiện tại
            var temps = new List<DepartmentTreeViewModel>();
            vmLstDepartment.ForEach(x =>
            {
                x.Items = vmLstDepartment.Where(y => y.ParentId == x.Id && y.Id != x.Id).OrderByDescending(k => k.Name).ToList();

            });
            var findTitleG5 =
                await _uow.GetRepository<JobGrade>().GetSingleAsync(x => x.Title.ToUpper().Equals("G5"));
            int G5Grade = findTitleG5?.Grade ?? 5;
            vmLstDepartment = vmLstDepartment.Where(x => x.JobGradeGrade >= G5Grade && x.Items.Any() || x.JobGradeGrade < G5Grade).ToList();
            departmentTree = vmLstDepartment.Where(item => item.ParentId == currentDepartment.Id).OrderByDescending(k => k.JobGradeGrade).ToList();
            ResultDTO result = new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Data = departmentTree,
                    Count = departmentTree.Count,
                },
            };
            return result;
        }
        public async Task<ResultDTO> GetDepartmentByReferenceNumber(string prefix, Guid itemId)
        {
            var currentDeparts = new List<DepartmentViewModel>();
            var currentReferenceNumbers = await _uow.GetRepository<ReferenceNumber>().GetAllAsync();
            var item = currentReferenceNumbers.FirstOrDefault(x => x.Formula.Contains(prefix));
            if (item != null)
            {
                if (item.ModuleType == "RequestToHire")
                {
                    var currentItem = await _uow.GetRepository<RequestToHire>().FindByIdAsync<RequestToHireViewModel>(itemId);
                    if (currentItem != null)
                    {
                        var departmentId = currentItem.ReplacementFor == TypeOfNeed.NewPosition ? currentItem.DeptDivisionId : currentItem.ReplacementForId;
                        var currentDepartment = await _uow.GetRepository<Department>().ITFindByIdAsync<DepartmentViewModel>(departmentId.Value);
                        if (currentDepartment != null)
                        {
                            currentDeparts.Add(currentDepartment);
                        }
                        else
                        {
                            List<string> ignoreStatus = new List<string>() { "Draft", "Requested To Change" };
                            var departmentModel = new DepartmentViewModel() { };
                            if (!ignoreStatus.Contains(currentItem.Status))
                            {
                                if (currentItem.ReplacementFor == TypeOfNeed.NewPosition)
                                {
                                    departmentModel = new DepartmentViewModel()
                                    {
                                        Id = currentItem.DeptDivisionId ?? currentItem.DeptDivisionId.Value,
                                        Name = currentItem.DeptDivisionName,
                                        Code = currentItem.DeptDivisionCode
                                    };
                                }
                                else
                                {
                                    departmentModel = new DepartmentViewModel()
                                    {
                                        Id = currentItem.ReplacementForId ?? currentItem.ReplacementForId.Value,
                                        Name = currentItem.ReplacementForName,
                                        Code = currentItem.ReplacementForCode
                                    };
                                }
                            }
                            currentDeparts.Add(departmentModel);
                        }
                    }
                }
                if (item.ModuleType == "Acting")
                {
                    var currentItem = await _uow.GetRepository<Acting>().FindByIdAsync<ActingViewModel>(itemId);
                    if (currentItem != null)
                    {
                        var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(currentItem.DepartmentId);
                        if (currentDepartment != null)
                        {
                            currentDeparts.Add(currentDepartment);
                        }
                    }
                }
                if (item.ModuleType == "PromoteAndTransfer")
                {
                    var currentItem = await _uow.GetRepository<PromoteAndTransfer>().FindByIdAsync<PromoteAndTransferViewModel>(itemId);
                    if (currentItem != null)
                    {
                        // do NewDeptOrLineId là Guid?
                        var departmentId = new Guid(currentItem.NewDeptOrLineId.ToString());
                        var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(departmentId);
                        if (currentDepartment != null)
                        {
                            currentDeparts.Add(currentDepartment);
                        }

                    }
                }
                if (item.ModuleType == "Applicant")
                {
                    // applicant detail
                    var currentApplicant = await _uow.GetRepository<Applicant>().FindByIdAsync<ApplicantViewModel>(itemId);
                    if (currentApplicant != null)
                    {
                        //list items Relative In Aeon
                        var items = await _uow.GetRepository<ApplicantRelativeInAeon>().FindByAsync<ApplicantRelativeInAeonViewModel>(x => x.ApplicantId == itemId);
                        //currentApplicant.ApplicantRelativeInAeons = items.ToList();
                        if (items.Any())
                        {
                            // tu list items ApplicantRelativeInAeon lấy departmentId tìm trong Department lấy ra elem:
                            foreach (var i in items)
                            {
                                var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(i.DepartmentId.Value);
                                if (currentDepartment != null)
                                {
                                    currentDeparts.Add(currentDepartment);
                                }
                            }
                        }
                    }
                }
                if (item.ModuleType == "OvertimeApplication")
                {
                    var currentOvertime = await _uow.GetRepository<OvertimeApplication>().FindByIdAsync<OvertimeApplicationViewModel>(itemId);
                    if (currentOvertime != null)
                    {
                        var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(currentOvertime.DivisionId);
                        if (currentDepartment != null)
                        {
                            currentDeparts.Add(currentDepartment);
                        }
                    }
                }
                if (item.ModuleType == "ShiftExchangeApplication")
                {
                    var currentShiftExchange = await _uow.GetRepository<ShiftExchangeApplication>().FindByIdAsync(itemId);
                    if (currentShiftExchange != null)
                    {
                        if (currentShiftExchange.DeptDivisionId != null)
                        {
                            Guid departmentId = new Guid(currentShiftExchange.DeptDivisionId.ToString());
                            var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(departmentId);
                            if (currentDepartment != null)
                            {
                                currentDeparts.Add(currentDepartment);
                            }
                        }

                    }
                }
            }
            return new ResultDTO { Object = new ArrayResultDTO { Data = currentDeparts } };
        }

        public async Task<ArrayResultDTO> GetDepartmentsByUserKeyword(CommonArgs.Member.User.GetAllUserByKeyword args)
        {
            var result = new ArrayResultDTO { };
            List<string> dept = new List<string>();
            try
            {
                var allUsers = await _uow.GetRepository<User>().FindByAsync(args.Predicate, args.PredicateParameters);
                if (allUsers != null)
                {
                    List<Guid> userIds = allUsers.ToList().Select(x => x.Id).ToList();
                    var userDepartmentMapping = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(y => userIds.Contains((Guid)y.UserId));
                    if (userDepartmentMapping != null)
                    {
                        List<Guid> deptIds = userDepartmentMapping.ToList().Where(y => y.DepartmentId != null).Select(x => (Guid)x.DepartmentId).ToList();
                        var department = await _uow.GetRepository<Department>().FindByAsync(y => deptIds.Contains((Guid)y.Id));
                        if (department != null)
                        {
                            dept = department.ToList().Select(x => x.Code).ToList();
                        }
                    }
                }
                return result = new ArrayResultDTO { Data = dept, Count = dept.Count() }; ;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
        #region User in Department

        public async Task<ResultDTO> GetEmployeeCodes()
        {
            var listUser = await _uow.GetRepository<User>().GetAllAsync<UserSelectViewModel>();
            var count = await _uow.GetRepository<User>().CountAllAsync();
            return new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Count = count,
                    Data = listUser.OrderBy(x => x.SAPCode)
                }
            };
        }
        public async Task<ResultDTO> GetUserInDepartment(Guid departmentId)
        {
            var lstUser = await _uow.GetRepository<UserDepartmentMapping>()
                .FindByAsync(x => x.DepartmentId == departmentId, "Created DESC", y => y.User);
            List<UserDepartmentMappingViewModel> vmLstUser = Mapper.Map<List<UserDepartmentMappingViewModel>>(lstUser);
            return new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Count = vmLstUser.Count,
                    Data = vmLstUser
                }
            };
        }
        public async Task<ResultDTO> UpdateUserInDepartment(UpdateUserDepartmentMappingArgs model)
        {
            UserDepartmentMapping mapping = await _uow.GetRepository<UserDepartmentMapping>().FindByIdAsync(model.Id);
            if (mapping == null)
            {
                return new ResultDTO { ErrorCodes = { 001 }, Messages = { "Not found User in this department" } };
            }
            var oldUserInDepartment = Mapper.Map<CommonViewModel.LogHistories.UserDepartmentMappingViewModel>(mapping);
            if (model.IsHeadCount)
            {
                //check if already is headcount in another department
                var isAlreadyHeadcount = await _uow.GetRepository<UserDepartmentMapping>()
                .FindByAsync(x => x.DepartmentId != model.DepartmentId && x.IsHeadCount == true && x.UserId == model.UserId);
                if (isAlreadyHeadcount.Any())
                {
                    var currentDepartment = await _uow.GetRepository<Department>()
                        .FindByIdAsync(model.DepartmentId);
                    return new ResultDTO
                    {
                        ErrorCodes = { 505 },
                        Messages = { $"This user is checked headcount in department {isAlreadyHeadcount.First().Department.Name}." +
                                $"\nDo you want to move this user to department {currentDepartment.Name} ?" }
                    };
                }
            }
            Mapper.Map(model, mapping);
            mapping.IsHeadCount = model.IsHeadCount;
            mapping.Modified = DateTime.Now;
            mapping.Role = model.Role;
            mapping.UserId = model.UserId;
            mapping.Note = model.Note;
            if (model.IsEdoc)
                mapping.IsPrepareDelete = false;
            
            try
            {
                var currentDepartment = await _uow.GetRepository<Department>()
                        .FindByIdAsync(model.DepartmentId);
                if (!(currentDepartment is null))
                {
                    currentDepartment.HasTrackingLog = true;
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(oldUserInDepartment),
                        ItemId = currentDepartment.Id,
                        ItemType = ItemTypeContants.UserDepartmentMapping,
                        Type = TrackingHistoryTypeContants.UpdateUser,
                        ItemRefereceNumberOrCode = currentDepartment.Code,
                    });
                    await _uow.CommitAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error: " + e.Message);
            }
            await _uow.CommitAsync();
            _dashboardBO.ClearNode();
            return new ResultDTO { };
        }
        public async Task<ResultDTO> RemoveUserFromDepartment(Guid Id)
        {
            if (Id == null)
            {
                return new ResultDTO { ErrorCodes = { 001 }, Messages = { "Id can not be null" } };
            }
            else
            {
                UserDepartmentMapping mapping = await _uow.GetRepository<UserDepartmentMapping>().FindByIdAsync(Id);
                if (mapping == null)
                {
                    return new ResultDTO { ErrorCodes = { 001 }, Messages = { "User has not in this department yet" } };
                }
                if (mapping.DepartmentId.HasValue)
                {
                    try
                    {
                        var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync(mapping.DepartmentId.Value);
                        if (!(currentDepartment is null))
                        {
                            currentDepartment.HasTrackingLog = true;
                            await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                            {
                                DataStr = JsonConvert.SerializeObject(Mapper.Map<CommonViewModel.LogHistories.UserDepartmentMappingViewModel>(mapping)),
                                ItemId = currentDepartment.Id,
                                ItemType = ItemTypeContants.UserDepartmentMapping,
                                Type = TrackingHistoryTypeContants.DeleteUser,
                                ItemRefereceNumberOrCode = currentDepartment.Code,
                            });
                            await _uow.CommitAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Error: " + e.Message);
                    }
                }
                _uow.GetRepository<UserDepartmentMapping>().Delete(mapping);
                await _uow.CommitAsync();
                _dashboardBO.ClearNode();
                return new ResultDTO { };
            }
        }
        public async Task<ResultDTO> RemoveUserFromDepartmentByUser(Guid userId)
        {
            if (userId == null)
            {
                return new ResultDTO { ErrorCodes = { 001 }, Messages = { "UserId can not be null" } };
            }
            else
            {
                var mappings = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(i => i.UserId == userId);
                if (!mappings.Any())
                {
                    return new ResultDTO { ErrorCodes = { 001 }, Messages = { "Not found User in this department" } };
                }
                if (mappings.Count() > 0)
                {
                    foreach (var userInDepartment in mappings)
                    {
                        if (userInDepartment.DepartmentId.HasValue)
                        {
                            try
                            {
                                var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync(userInDepartment.DepartmentId.Value);
                                if (!(currentDepartment is null))
                                {
                                    currentDepartment.HasTrackingLog = true;
                                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                                    {
                                        DataStr = JsonConvert.SerializeObject(Mapper.Map<CommonViewModel.LogHistories.UserDepartmentMappingViewModel>(userInDepartment)),
                                        ItemId = currentDepartment.Id,
                                        ItemType = ItemTypeContants.UserDepartmentMapping,
                                        Type = TrackingHistoryTypeContants.DeleteUser,
                                        ItemRefereceNumberOrCode = currentDepartment.Code,
                                    });
                                    await _uow.CommitAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("Error: " + e.Message);
                            }
                        }
                    }
                }
                _uow.GetRepository<UserDepartmentMapping>().Delete(mappings);
                await _uow.CommitAsync();
                _dashboardBO.ClearNode();
                return new ResultDTO { };
            }
        }
        public async Task<ResultDTO> AddUserToDepartment(UserInDepartmentArgs model)
        {
            var existing = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == model.DepartmentId && x.Role == model.Role && x.UserId == model.UserId);
            if (existing)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "This user is already in this department" } };
            }
            else
            {
                if (model.IsHeadCount)
                {
                    //1 user chỉ có thể là headcount ở 1 department

                    //check if already is headcount in another department
                    var isAlreadyHeadcount = await _uow.GetRepository<UserDepartmentMapping>()
                    .FindByAsync(x => x.DepartmentId != model.DepartmentId && x.IsHeadCount == true && x.UserId == model.UserId);
                    if (isAlreadyHeadcount.Any())
                    {
                        var currentDepartment = await _uow.GetRepository<Department>()
                            .FindByIdAsync(model.DepartmentId);
                        return new ResultDTO
                        {
                            ErrorCodes = { 505 },
                            Messages = { $"This user is checked headcount in department {isAlreadyHeadcount.First().Department.Name}." +
                                $"\n Do you want to move this user to department {currentDepartment.Name} ?" }
                        };
                    }
                }
                if (model.Role == null)
                {
                    model.Role = Group.Member;
                }
                var mapping = Mapper.Map<UserDepartmentMapping>(model);
                mapping.Created = DateTime.Now;
                try
                {
                    if (mapping.DepartmentId.HasValue)
                    {
                        var currentDepartment = await _uow.GetRepository<Department>()
                            .FindByIdAsync(mapping.DepartmentId.Value);
                        if (!(currentDepartment is null))
                        {
                            currentDepartment.HasTrackingLog = true;
                            await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                            {
                                DataStr = JsonConvert.SerializeObject(Mapper.Map<CommonViewModel.LogHistories.UserDepartmentMappingViewModel>(mapping)),
                                ItemId = currentDepartment.Id,
                                ItemType = ItemTypeContants.UserDepartmentMapping,
                                Type = TrackingHistoryTypeContants.AddUser,
                                ItemRefereceNumberOrCode = currentDepartment.Code,
                            });
                            await _uow.CommitAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Error: " + e.Message);
                }
                _uow.GetRepository<UserDepartmentMapping>().Add(mapping);
                await _uow.CommitAsync();
                _dashboardBO.ClearNode();
            }
            return new ResultDTO { };
        }
        public async Task<ResultDTO> MoveHeadCountUpdate(UpdateUserDepartmentMappingArgs model)
        {
            UserDepartmentMapping mapping = await _uow.GetRepository<UserDepartmentMapping>().FindByIdAsync(model.Id);
            if (mapping == null)
            {
                return new ResultDTO { ErrorCodes = { 001 }, Messages = { "Not found User in this department" } };
            }

            var isAlreadyHeadcount = await _uow.GetRepository<UserDepartmentMapping>()
                    .FindByAsync(x => x.DepartmentId != model.DepartmentId && x.IsHeadCount == true && x.UserId == model.UserId);

            //isAlreadyHeadcount.ToList().ForEach(item => item.IsHeadCount = false);
            //Fix ticket 313
            if (isAlreadyHeadcount.Any())
            {
                List<UserDepartmentMapping> currentHeadCountDepartments = isAlreadyHeadcount.ToList();
                int isAlreadyHeadcount_count = currentHeadCountDepartments.Count;
                for (int i = isAlreadyHeadcount_count - 1; i >= 0; i--)
                {
                    _uow.GetRepository<UserDepartmentMapping>().Delete(currentHeadCountDepartments[i]);
                }
            }

            mapping.IsHeadCount = model.IsHeadCount;
            mapping.Modified = DateTime.Now;
            mapping.Role = model.Role;
            mapping.UserId = model.UserId;

            await _uow.CommitAsync();
            return new ResultDTO { };
        }
        public async Task<ResultDTO> MoveHeadCountAdd(UserInDepartmentArgs model)
        {
            var existing = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == model.DepartmentId && x.UserId == model.UserId);
            if (existing)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "This user is already in this department" } };
            }
            var isAlreadyHeadcount = await _uow.GetRepository<UserDepartmentMapping>()
                    .FindByAsync(x => x.DepartmentId != model.DepartmentId && x.IsHeadCount == true && x.UserId == model.UserId);
            isAlreadyHeadcount.ToList().ForEach(item => item.IsHeadCount = false);
            if (model.Role == null)
            {
                model.Role = Group.Member;
            }

            // Ghi log delete user
            try
            {
                var oldDepartment = isAlreadyHeadcount?.FirstOrDefault();
                if (oldDepartment != null)
                {
                    var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync((Guid)oldDepartment.DepartmentId);
                    if (currentDepartment != null)
                    {
                        currentDepartment.HasTrackingLog = true;
                        await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                        {
                            DataStr = JsonConvert.SerializeObject(
                                Mapper.Map<CommonViewModel.LogHistories.UserDepartmentMappingViewModel>(oldDepartment)
                            ),
                            ItemId = currentDepartment.Id,
                            ItemType = ItemTypeContants.UserDepartmentMapping,
                            Type = TrackingHistoryTypeContants.DeleteUser,
                            ItemRefereceNumberOrCode = currentDepartment.Code,
                        });
                    }
                }


                //Ghi log add user
                var newDepartment = await _uow.GetRepository<Department>().FindByIdAsync((Guid)model.DepartmentId);
                if (!(newDepartment is null))
                {
                    newDepartment.HasTrackingLog = true;
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(Mapper.Map<UserDepartmentMapping>(model)),
                        ItemId = newDepartment.Id,
                        ItemType = ItemTypeContants.UserDepartmentMapping,
                        Type = TrackingHistoryTypeContants.AddUser,
                        ItemRefereceNumberOrCode = newDepartment.Code,
                    });
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error while saving tracking history for user department mapping");
            }

            var mapping = Mapper.Map<UserDepartmentMapping>(model);
            _uow.GetRepository<UserDepartmentMapping>().Add(mapping);
            _uow.GetRepository<UserDepartmentMapping>().Delete(isAlreadyHeadcount);

            await _uow.CommitAsync();

            return new ResultDTO { };
        }
        public async Task<ResultDTO> GetDepartmentById(Guid Id)
        {
            var existing = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(Id);
            if (existing == null)
            {
                return new ResultDTO { ErrorCodes = { 111 }, Messages = { "Not found department with id " + Id }, };
            }
            else
            {
                return new ResultDTO { Object = existing };
            }
        }
        public async Task<ResultDTO> GetDepartmentsByUserId(Guid Id)
        {
            var existings = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.UserId == Id && (x.IsHeadCount || x.Role == Group.Member));
            var userInAllDepartments = new List<UserInAllDepartmentViewModel>();
            if (existings != null && existings.Any())
            {
                foreach (var item in existings)
                {
                    if (item.Department != null && !userInAllDepartments.Any(x => x.DeptLine != null && x.DeptLine.Id == item.DepartmentId))
                    {
                        var userInAllDepartment = new UserInAllDepartmentViewModel();
                        if (item.Department.Type == DepartmentType.Department)
                        {
                            userInAllDepartment.DeptLine = Mapper.Map<DepartmentViewModel>(item.Department);
                            userInAllDepartments.Add(userInAllDepartment);
                        }
                        else
                        {
                            var division = Mapper.Map<DepartmentViewModel>(item.Department);
                            var skip = false;
                            var departmentIdx = item.Department.ParentId;
                            while (!skip)
                            {
                                if (departmentIdx.HasValue)
                                {
                                    var dept = await _uow.GetRepository<Department>().GetSingleAsync<DepartmentViewModel>(x => x.Id == departmentIdx);
                                    if (dept != null)
                                    {
                                        if (dept.Type == DepartmentType.Department)
                                        {
                                            skip = true;
                                            var deptItem = userInAllDepartments.FirstOrDefault(x => x.DeptLine.Id == dept.Id);
                                            if (deptItem == null)
                                            {
                                                userInAllDepartment.DeptLine = dept;
                                                userInAllDepartment.Divisions.Add(division);
                                                userInAllDepartments.Add(userInAllDepartment);
                                            }
                                            else
                                            {
                                                deptItem.Divisions.Add(division);
                                            }
                                        }
                                        else
                                        {
                                            departmentIdx = dept.ParentId;
                                        }
                                    }
                                    else
                                    {

                                        skip = true;
                                    }
                                }
                                else
                                {
                                    skip = true;
                                }
                            }
                        }

                    }
                }
                if (userInAllDepartments.Any())
                {
                    foreach (var item in userInAllDepartments)
                    {
                        if (item.Divisions.Any())
                        {
                            await CheckValidDivision(item);
                        }
                    }
                }
                return new ResultDTO { Object = new ArrayResultDTO { Data = userInAllDepartments, Count = userInAllDepartments.Count() } };

            }
            else
            {
                return new ResultDTO { ErrorCodes = { 404 }, Messages = { "Not found department with id " + Id }, };

            }
        }
        /// <summary>
        /// Lấy danh sách department từ G4 trở xuống
        /// Nếu không có G4 thì sẽ lấy các jobgrade thấp hơn.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ResultDTO> GetDepartmentUpToG4ByUserId(Guid Id)
        {
            var existings = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.Department != null && !x.Department.IsFromIT && x.DepartmentId.HasValue && x.UserId == Id && (x.IsHeadCount || x.Role == Group.Member || x.Role == Group.Assistance));
            var userInAllDepartments = new List<UserInAllDepartmentViewModel>();
            if (existings != null && existings.Any())
            {
                foreach (var item in existings)
                {
                    if (!userInAllDepartments.Any(x => x.DeptLine != null && x.DeptLine.Id == item.DepartmentId))
                    {
                        var userInAllDepartment = new UserInAllDepartmentViewModel();
                        if (item.Department.Type == DepartmentType.Department)
                        {
                            userInAllDepartment.DeptLine = Mapper.Map<DepartmentViewModel>(item.Department);
                            userInAllDepartments.Add(userInAllDepartment);
                        }
                        else
                        {
                            var division = Mapper.Map<DepartmentViewModel>(item.Department);
                            var skip = false;
                            var departmentIdx = item.Department.ParentId;
                            while (!skip)
                            {
                                if (departmentIdx.HasValue)
                                {
                                    var dept = await _uow.GetRepository<Department>().GetSingleAsync<DepartmentViewModel>(x => x.Id == departmentIdx);
                                    if (dept != null)
                                    {
                                        if (dept.Type == DepartmentType.Department)
                                        {
                                            skip = true;
                                            var deptItem = userInAllDepartments.FirstOrDefault(x => x.DeptLine.Id == dept.Id);
                                            //var divisionList = await _uow.GetRepository<Department>().FindByAsync(x => x.JobGrade.Grade <= 4);
                                            var divisionList = await _uow.GetRepository<Department>().FindByAsync(x => x.Type == DepartmentType.Division);
                                            if (deptItem == null)
                                            {
                                                userInAllDepartment.DeptLine = dept;
                                                //userInAllDepartment.Divisions.Add(division);
                                                userInAllDepartment.Divisions.AddRange(GetHighestDivision(division, divisionList));
                                                userInAllDepartments.Add(userInAllDepartment);
                                            }
                                            else
                                            {
                                                deptItem.Divisions.AddRange(GetHighestDivision(division, divisionList));
                                            }
                                        }
                                        else
                                        {
                                            departmentIdx = dept.ParentId;
                                        }
                                    }
                                    else
                                    {

                                        skip = true;
                                    }
                                }
                                else
                                {
                                    skip = true;
                                }
                            }
                        }

                    }
                }
                if (userInAllDepartments.Any())
                {
                    foreach (var item in userInAllDepartments)
                    {
                        if (item.Divisions.Any())
                        {
                            await CheckValidDivision(item);
                        }
                    }
                }
                return new ResultDTO { Object = new ArrayResultDTO { Data = userInAllDepartments, Count = userInAllDepartments.Count() } };

            }
            else
            {
                return new ResultDTO { ErrorCodes = { 404 }, Messages = { "Not found department with id " + Id }, };

            }
        }
        /// <summary>
        /// Lấy department cao nhất từ G4 trở xuống có quan hệ với division hiện tại. <br/>
        /// Nếu department cha từ G5 trở lên thì lấy division hiện tại.
        /// </summary>
        /// <param name="division"></param>
        /// <returns></returns>
        private List<DepartmentViewModel> GetHighestDivision(DepartmentViewModel division, IEnumerable<Department> divisionList)
        {
            var jgG4 = _uow.GetRepository<JobGrade>().GetSingle(x => x.Title.Equals("G4"));
            if (jgG4 == null)
            {
                jgG4 = new JobGrade() { Grade = 4 };
            }
            List<DepartmentViewModel> resultList = new List<DepartmentViewModel>();
            //if (division.JobGradeGrade > 4)
            //if (division.Type == DepartmentType.Department)
            if (division.JobGradeGrade > jgG4.Grade)
            {
                return resultList;
            }

            var findParentDepartmentOfDivision = _uow.GetRepository<Department>().GetSingle(x => x.Id == division.ParentId);
            //if (division.JobGradeGrade == 4)
            //if (division.Type == DepartmentType.Division && findParentDepartmentOfDivision.Type == DepartmentType.Department)
            if (division.JobGradeGrade == jgG4.Grade)
            {
                resultList.Add(division);
            }
            else
            {
                bool found = false;
                while (!found)
                {
                    if (division.ParentId.HasValue == false)
                    {
                        resultList.Add(division);
                        found = true;
                    }
                    var parentDepartment = divisionList.Where(x => x.Id == division.ParentId).FirstOrDefault();
                    if (parentDepartment == null)
                    {
                        resultList.Add(division);
                        found = true;
                    }
                    else
                    {
                        var findParentDepartmentOfParentDepartment = _uow.GetRepository<Department>().GetSingle(x => x.Id == parentDepartment.ParentId);
                        //if (parentDepartment.JobGrade.Grade <= 4)
                        if (parentDepartment.JobGrade.Grade <= jgG4.Grade)
                        //if (parentDepartment.Type == DepartmentType.Division && findParentDepartmentOfParentDepartment.Type == DepartmentType.Department)
                        {
                            resultList.Add(Mapper.Map<DepartmentViewModel>(parentDepartment));
                            found = true;
                        }
                        else
                        {
                            resultList.AddRange(GetHighestDivision(division, divisionList));
                        }
                    }
                }
            }
            return resultList;
        }
        private async Task CheckValidDivision(UserInAllDepartmentViewModel data)
        {
            var jgG5 = _uow.GetRepository<JobGrade>().GetSingle(x => x.Title.Equals("G5"));
            if (jgG5 == null)
            {
                jgG5 = new JobGrade() { Grade = 5 };
            }
            var currentDivisions = data.Divisions.OrderByDescending(x => x.JobGradeGrade);
            data.Divisions = new List<DepartmentViewModel>();
            foreach (var item in currentDivisions)
            {
                if (!data.Divisions.Any())
                {
                    data.Divisions.Add(item);
                }
                else
                {
                    var skip = false;
                    var departmentId = item.ParentId;
                    while (!skip)
                    {
                        if (departmentId.HasValue)
                        {
                            var departmentParent = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(departmentId.Value);
                            if (departmentParent != null)
                            {
                                if (departmentParent.JobGradeGrade < jgG5.Grade && currentDivisions.Any(x => x.Id == departmentParent.Id))
                                {
                                    skip = true;
                                }
                                else if (departmentParent.JobGradeGrade >= jgG5.Grade)
                                {
                                    data.Divisions.Add(item);
                                    skip = true;
                                }
                                else
                                {
                                    skip = false;
                                    departmentId = departmentParent.ParentId;
                                }
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                        else
                        {
                            skip = true;
                        }
                    }
                }
            }


        }

        private async Task<IEnumerable<DepartmentTreeViewModel>> GetDepartmentTreesByFilter(QueryArgs args)
        {
            IEnumerable<DepartmentTreeViewModel> departmentTree = new List<DepartmentTreeViewModel>();
            IEnumerable<DepartmentTreeViewModel> vmLstDepartment = null;
            var temps = new List<DepartmentTreeViewModel>();
            var lstDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentTreeViewModel>(x => true);
            lstDepartment.ToList().ForEach(item =>
            {
                var items = lstDepartment.Where(y => y.ParentId == item.Id);
                if (items.Any())
                {
                    item.Items = items.OrderByDescending(x => x.Name);
                    temps.Add(item);
                }
            });
            var existItems = lstDepartment.Where(x => !temps.Contains(x) && temps.Any(y => !y.Items.Contains(x))).OrderByDescending(x => x.JobGradeGrade).ToList();

            if (existItems.Any())
            {

                temps.AddRange(existItems);
            }
            if (!temps.Any())
            {
                temps = lstDepartment.ToList();
            }
            if (!string.IsNullOrEmpty(args.Predicate))
            {
                vmLstDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentTreeViewModel>(args.Predicate, args.PredicateParameters);
                if (vmLstDepartment != null && vmLstDepartment.Any())
                {
                    var g9Department = vmLstDepartment.Any(x => !x.ParentId.HasValue);
                    if (g9Department)
                    {
                        departmentTree = lstDepartment.Where(x => !x.ParentId.HasValue && vmLstDepartment.Any(y => y.Id == x.Id));
                    }
                    else
                    {
                        departmentTree = temps.Where(x => vmLstDepartment.Any(m => m.Id == x.Id) || x.Items.Any(y => vmLstDepartment.Any(k => k.Id == y.Id))).OrderByDescending(x => x.JobGradeGrade);
                        departmentTree = departmentTree.Where(x => vmLstDepartment.Any(y => y.Id == x.Id));
                    }
                }
            }
            return departmentTree;
        }
        private void GetAllDepartments()
        {
            if (AllDepartments == null || AllDepartments.Count() == 0)
            {
                lock (_lock)
                {
                    if (AllDepartments == null || AllDepartments.Count() == 0)
                    {
                        var lstDepartment = _uow.GetRepository<Department>().FindBy<DepartmentTreeViewModel>(x => true).ToList();
                        IEnumerable<DepartmentTreeViewModel> departmentTree = new List<DepartmentTreeViewModel>();

                        var temp = new List<DepartmentTreeViewModel>();
                        var count = lstDepartment.Count();
                        for (int i = 0; i < count; i++)
                        {
                            var item = lstDepartment.ElementAt(i);
                            var items = lstDepartment.Where(y => y.ParentId == item.Id);
                            if (items.Any())
                            {
                                item.Items = items.OrderByDescending(x => x.Name);
                                temp.Add(item);
                            }
                        }
                        var existItems = lstDepartment.Where(x => !temp.Contains(x) && temp.Any(y => !y.Items.Contains(x)));
                        if (existItems.FirstOrDefault() != null)
                        {
                            temp.AddRange(existItems);
                        }
                        if (!temp.Any())
                        {
                            temp = lstDepartment.ToList();
                        }
                        AllDepartments = temp.Where(x => !x.ParentId.HasValue);
                    }
                }
            }
        }
        public async Task<ResultDTO> GetDepartmentByArg(QueryArgs args)
        {
            var listDepartment = new List<DepartmentTreeViewModel>();
            GetAllDepartments();
            if (!string.IsNullOrEmpty(args.Predicate))
            {
                var vmLstDepartment = await _uow.GetRepository<Department>(true).FindByAsync<DepartmentTreeViewModel>(args.Predicate, args.PredicateParameters, "JobGrade.Grade desc");
                if (vmLstDepartment.Any())
                {
                    var temp = vmLstDepartment;
                    vmLstDepartment = vmLstDepartment.Where(x => !temp.Any(y => y.ParentId == x.Id));
                    if (vmLstDepartment != null)
                    {
                        if (vmLstDepartment.Count() > 1)
                        {
                            foreach (var deparment in vmLstDepartment)
                            {
                                foreach (var treeItem in AllDepartments)
                                {
                                    listDepartment.AddRange(SearchDepartment(treeItem, deparment.Id));
                                }
                            }
                        }
                        else
                        {
                            foreach (var g9 in AllDepartments)
                            {
                                listDepartment.AddRange(SearchDepartment(g9, vmLstDepartment.FirstOrDefault().Id));
                            }
                        }

                    }
                    else
                    {
                        foreach (var deparment in temp)
                        {
                            foreach (var g9 in AllDepartments)
                            {
                                listDepartment.AddRange(SearchDepartment(g9, deparment.Id));
                            }
                        }
                    }


                }
            }
            else
            {
                listDepartment = AllDepartments.ToList();
            }
            return new ResultDTO { Object = new ArrayResultDTO { Data = listDepartment } };
        }
        public async Task<DepartmentTreeViewModel> GetDepartmentTreeById(Guid Id)
        {
            var listDepartment = new List<DepartmentTreeViewModel>();
            GetAllDepartments();
            if (Id != Guid.Empty)
            {
                var vmLstDepartment = await _uow.GetRepository<Department>(true).FindByAsync<DepartmentTreeViewModel>(x => x.Id == Id);
                if (vmLstDepartment.Any())
                {
                    var temp = vmLstDepartment;
                    vmLstDepartment = vmLstDepartment.Where(x => !temp.Any(y => y.ParentId == x.Id));
                    if (vmLstDepartment != null)
                    {
                        if (vmLstDepartment.Count() > 1)
                        {
                            foreach (var deparment in vmLstDepartment)
                            {
                                foreach (var treeItem in AllDepartments)
                                {
                                    listDepartment.AddRange(SearchDepartment(treeItem, deparment.Id));
                                }
                            }
                        }
                        else
                        {
                            foreach (var g9 in AllDepartments)
                            {
                                listDepartment.AddRange(SearchDepartment(g9, vmLstDepartment.FirstOrDefault().Id));
                            }
                        }

                    }
                    else
                    {
                        foreach (var deparment in temp)
                        {
                            foreach (var g9 in AllDepartments)
                            {
                                listDepartment.AddRange(SearchDepartment(g9, deparment.Id));
                            }
                        }
                    }


                }
            }

            return listDepartment.FirstOrDefault();
        }

        private IEnumerable<DepartmentTreeViewModel> SearchDepartment(DepartmentTreeViewModel dp, Guid searchDepartmentId)
        {
            if (dp.Id == searchDepartmentId)
                yield return dp;
            foreach (var item in dp.Items.SelectMany(x => SearchDepartment(x, searchDepartmentId)))
            {
                if (item.Id == searchDepartmentId)
                    yield return item;
            }
        }
        public ResultDTO ResetDepartments()
        {
            AllDepartments = null;
            return new ResultDTO() { };
        }
        public async Task<ResultDTO> GetAllDepartmentsByPositonName(string posiontionName)
        {
            if (!string.IsNullOrEmpty(posiontionName))
            {
                var openedPositions = await _uow.GetRepository<Data.Models.Position>(true).FindByAsync(x => x.PositionName.Trim() == posiontionName.Trim() && x.Status == PositionStatus.Opened);
                if (openedPositions != null && openedPositions.Any())
                {
                    List<string> ignoreStatus = new List<string>() { Const.Status.cancelled, Const.Status.draft, Const.Status.rejected };

                    var rthOfOpenedPositions = openedPositions.Select(x => x.RequestToHireNumber).ToList();
                    var rthIds = (await _uow.GetRepository<RequestToHire>(true).FindByAsync(x => x.PositionName.Trim() == posiontionName.Trim() && rthOfOpenedPositions.Contains(x.ReferenceNumber))).Select(x => x.Id).ToList();

                    //var promote_Transfer_NewDeptOrLineIds_test = (await _uow.GetRepository<PromoteAndTransfer>(true).FindByAsync(x => !ignoreStatus.Contains(x.Status))).Select(x => x.NewDeptOrLineCode).ToList();
                    // cheat code
                    var departmentIds = (await _uow.GetRepository<Department>(true).FindByAsync(x => rthIds.Contains(x.RequestToHireId.Value) || x.Code == "40002333" || x.Code == "DEPT_17032022" || x.Code == "50022551" || x.Code == "40001053-Transfer" || x.Code == "50033940" || x.Code == "40000375")).Select(x => x.Id).ToList(); ;

                    return new ResultDTO { Object = new ArrayResultDTO { Data = departmentIds } };
                }
                else
                {
                    return new ResultDTO { Object = new ArrayResultDTO() };
                }
            }
            else
            {
                return new ResultDTO { Object = new ArrayResultDTO() };
            }
        }

        public bool ResetAllDepartmentCache()
        {
            bool returnValue = false;
            try
            {
                ObjectCache cache = MemoryCache.Default;
                List<string> departmentKeys = cache.Where(x => !string.IsNullOrEmpty(x.Key) && x.Key.StartsWith(STR_DeparmentPreFix_CacheKey)).Select(x => x.Key).ToList();
                foreach (string current in departmentKeys)
                {
                    cache.Remove(current);
                }
                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }
        #endregion

        public async Task<ResultDTO> GetRegionList(QueryArgs args)
        {
            var regionData = await _uow.GetRepository<Region>().FindByAsync<RegionViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<Region>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = regionData, Count = count };
            return new ResultDTO { Object = result };
        }

        public async Task UpdateIsEdoc1Async(IEnumerable<DepartmentTreeViewModel> departmentTree)
        {
            IEnumerable<Guid> GetAllDepartmentIds(IEnumerable<DepartmentTreeViewModel> tree)
            {
                var ids = new List<Guid>();
                foreach (var dept in tree)
                {
                    ids.Add(dept.Id);
                    if (dept.Items != null && dept.Items.Any())
                    {
                        ids.AddRange(GetAllDepartmentIds(dept.Items));
                    }
                }
                return ids.Distinct().ToList();
            }

            var allIds = GetAllDepartmentIds(departmentTree).ToList();
            if (allIds.Any())
            {
                try
                {
                    var isEdoc1Dict = new Dictionary<Guid, bool?>(allIds.Count);
                    const int batchSize = 1000; 
                    var repo = _uow.GetRepository<Department>();

                    for (int i = 0; i < allIds.Count; i += batchSize)
                    {
                        var batch = allIds.Skip(i).Take(batchSize).ToList();
                        var parameterNames = batch.Select((_, j) => $"@p{j}").ToList();
                        var sql = $"SELECT Id, IsEdoc1 FROM dbo.ITDepartments WHERE Id IN ({string.Join(",", parameterNames)})";
                        var parameters = batch.Select((id, j) => new SqlParameter($"@p{j}", id)).ToArray();

                        var itDepartments = await repo.ExecuteRawSqlQueryAsync<ITDepartmentDTO>(sql, parameters);

                        foreach (var dept in itDepartments)
                        {
                            isEdoc1Dict[dept.Id] = dept.IsEdoc1;
                        }
                    }

                    void UpdateIsEdoc1(DepartmentTreeViewModel dept)
                    {
                        if (isEdoc1Dict.TryGetValue(dept.Id, out var isEdoc1))
                        {
                            dept.IsEdoc1 = isEdoc1;
                        }
                        if (dept.Items != null)
                        {
                            foreach (var child in dept.Items)
                            {
                                UpdateIsEdoc1(child);
                            }
                        }
                    }

                    foreach (var root in departmentTree)
                    {
                        UpdateIsEdoc1(root);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}