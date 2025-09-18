using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Aeon.HR.Infrastructure.Enums;
using Newtonsoft.Json;
using SyncOrgchartJob.Enums;
using SyncOrgchartJob.Models;
using SyncOrgchartJob.Models.eDocHR;

namespace SyncOrgchartJob.Service
{
    public class ApplyService
    {
        private readonly string _connHRStr = ConfigurationManager.AppSettings["HRDb"];
        private readonly string _connStr = ConfigurationManager.AppSettings["StagingDb"];
        private readonly bool usingAPI = bool.Parse(ConfigurationManager.AppSettings["usingAPI"]);
        private List<JobGrade> _jobGrades;
        private List<User> _userLists;
        private List<Position> _positionLists;
        private List<Position> _insertPositions = new List<Position>();
        private List<BusinessModel> _businessModels;
        private List<BusinessModelUnitMapping> _businessModelUnitMappings;
        private List<MasterData> _masterDatasWorkLocation;
        private List<Department> _insertDepartments = new List<Department>();
        private List<Department> _updateDepartments = new List<Department>();
        private List<Department> _deleteDepartments = new List<Department>();
        
        private List<Department> _allDepartments = new List<Department>();
        private List<StagingDetailDto> _allStagingDetails = new List<StagingDetailDto>();
        
        private HashSet<string> _newDepartmentCodes = new HashSet<string>(); // Dùng để xử lý deleteDepartments

        private List<UserDepartmentMapping> _insertUserDepartment = new List<UserDepartmentMapping>();
        private List<UserDepartmentMapping> _updateUserDepartment = new List<UserDepartmentMapping>();
        private List<UserDepartmentMapping> _deleteUserDepartment = new List<UserDepartmentMapping>();
        
        private List<User> _insertUsers = new List<User>();
        private List<User> _updateUsers = new List<User>();
        // private List<User> _deleteDepartments = new List<User>();
        
        public async Task RunAsync(int headerId)
        {
            Logger.Write("ApplyService started...", true);
            try
            {
                _jobGrades = await DataService.GetListJobGrade();
                _userLists = await DataService.GetListUsers();
                _positionLists = await DataService.GetListPositions();
                _businessModels = await DataService.GetListBusinessModels();
                _businessModelUnitMappings = await DataService.GetListBusinessModelUnitMapping();
                _masterDatasWorkLocation = await DataService.GetListCacheMasterDataWorkLocation();
                
                using (var conn = new SqlConnection(_connStr))
                {
                    await conn.OpenAsync();
                    // 1. Lấy header có trạng thái pending
                    var header = await DataService.GetPendingHeaderAsync(headerId, conn);
                    if (header == null)
                    {
                        Console.WriteLine("No pending header found.");
                        return;
                    }

                    Logger.Write($"Processing Header ID: {header.Id}, Type: {header.SyncType}", true);

                    // 2. Lấy danh sách detail theo HeaderId
                    _allStagingDetails = await DataService.GetDetailsByHeaderIdAsync(conn, header.Id);
                    Logger.Write($"Found Staging {_allStagingDetails.Count} detail rows.", true);

                    _allDepartments = await DataService.GetAllDepartmentsAsync();
                    Logger.Write($"Found HR {_allDepartments.Count} detail rows.", true);
                        
                    var allPersonLists = _allStagingDetails.Where(x => x.ObjectType == ObjectType.Person).ToList();
                    Logger.Write($"Found Staging Person: {allPersonLists.Count} detail rows.", true);
                    
                    // tìm cap G cao nhat
                    var tree = _allStagingDetails.BuildTreeWithPersons();
                    Logger.Write($"Found Staging tree with {tree.Count} root nodes.", true);
                    var treeHRs = _allDepartments.BuildTree();
                    //Logger.Write($"Found HR tree with {treeHRs.Count} root nodes.", true);
                    
                    var treeIgnoreRoot = tree[0].Children;                    
                    //Logger.Write($"Found Staging tree with {JsonConvert.SerializeObject(treeIgnoreRoot)} root nodes.", true);

                    // Duyệt toàn bộ cây mới (treeIgnoreRoot)
                    foreach (var root in treeIgnoreRoot)
                    {
                        ProcessNode(root, treeHRs);
                    }
                    Logger.Write($"Processed {_newDepartmentCodes.Count} new department codes.", true);
                    
                    using (var tran = conn.BeginTransaction())
                    {
                        SqlTransaction tranHR = null;
                        try
                        {
                            #region Test
                            /*foreach (var root in treeIgnoreRoot)
                            {
                                var child = root.Children.FirstOrDefault(x => x.ObjectId == "40001009");
                                if (child != null)
                                {
                                    child = child.Children.FirstOrDefault(x => x.ObjectId == "40001163");
                                    ProcessNode(child);
                                }
                            }*/
                            #endregion
                            #region Process Delete Departments
                            ProcessDeleteDepartments(treeHRs);
                            #endregion
                            
                            ProcessUsers(allPersonLists);
                            
                            #region Save log

                            var connHR = new SqlConnection(_connHRStr);
                            await connHR.OpenAsync();
                            tranHR = connHR.BeginTransaction();
                            try
                            {
                                #region MyRegion
                                await CreateNewPosition(connHR, tranHR);
                                #endregion
                                
                                #region Save Department
                                await ApplyDepartments(connHR, tranHR);
                                #endregion
                                
                                #region Save UserDepartmentMapping
                                await ApplyUserDepartmentMappings(connHR, tranHR);
                                #endregion
                                
                                #region Save User
                                await ApplyUser(connHR, tranHR);
                                #endregion
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Error saving logs for save log update persons: {e.Message}");
                                Logger.Write($"Error saving logs for save log update persons: {e.Message}", true);
                                tranHR.Rollback();
                                throw new Exception($"Error saving logs for save log update persons: {e.Message}");
                            }
                            #endregion
                            
                            tranHR.Commit();
                            // 3. Cập nhật header về Done
                            Console.WriteLine($"✅ Apply {_allStagingDetails.Count} records to staging. HeaderId: {header.Id}");
                            await UpdateHeaderStatusAsync(conn, tran, header.Id, "Success", "Apply completed successfully.");
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tranHR.Rollback();
                            tran.Rollback();
                            await UpdateHeaderStatusAsync(conn, tran, header.Id, "Failed", ex.Message);
                            Console.WriteLine("Apply failed: " + ex.Message);
                            throw new Exception($"Apply failed: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write($"Error running ApplyService: {ex.Message}", true);
                throw new Exception($"Error running ApplyService: {ex.Message}");
            }
        }
        
        private void ProcessNode(StagingDetailDto node, List<Department> treeHRs)
        {
            if (node.ObjectType == ObjectType.Organization || node.ObjectType == ObjectType.Position)
            {
                try 
                {
                    _newDepartmentCodes.Add(node.ObjectId.ToString());
                } catch (Exception e)
                {
                    Logger.Write($"Error processing node {node.ObjectId}: {e.Message}", true);
                    throw new Exception($"Error processing node {node.ObjectId}: {e.Message}");
                }
            }
            
            if (node.ObjectType == ObjectType.Position)
            {
                ProcessPosition(node, treeHRs);
            }

            if (node.Children.Any())
            {
                SortChildren(node.Children);
                foreach (var child in node.Children)
                {
                    ProcessNode(child, treeHRs);
                }
            }
        }
        
        public static void SortChildren(List<StagingDetailDto> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            // Sắp xếp: ObjectType = "S" trước, sau đó tới các loại khác, giữ nguyên thứ tự gốc trong mỗi nhóm
            nodes.Sort((a, b) =>
            {
                bool aIsS = a.ObjectType == "S";
                bool bIsS = b.ObjectType == "S";

                if (aIsS && !bIsS) return -1; // a trước
                if (!aIsS && bIsS) return 1;  // b trước
                return 0; // Giữ nguyên thứ tự nếu cùng nhóm
            });

            // Gọi đệ quy cho các node con
            foreach (var node in nodes)
            {
                SortChildren(node.Children);
            }
        }
        
        private async Task CreateNewPosition(SqlConnection connHR, SqlTransaction trans)
        {
            await _insertPositions.InsertPositions(connHR, trans);
        }
        
        private async Task ApplyDepartments(SqlConnection connHR, SqlTransaction trans)
        {
            await _insertDepartments.ProcessSyncData("Add", connHR, trans);
            await _updateDepartments.ProcessSyncData("Update", connHR, trans);
            await _deleteDepartments.ProcessSyncData("Delete", connHR, trans);
        }

        private async Task ApplyUserDepartmentMappings(SqlConnection connHR, SqlTransaction trans)
        {
            await _insertUserDepartment.ProcessSyncData("Add", connHR, trans);
            await _updateUserDepartment.ProcessSyncData("Update", connHR, trans);
            await _deleteUserDepartment.ProcessSyncData("Delete", connHR, trans);
        }

        private async Task ApplyUser(SqlConnection connHR, SqlTransaction trans)
        {
            // await _insertUsers.ProcessSyncData("Add", connHR, trans);
            await _updateUsers.ProcessSyncData("Update", connHR, trans);
            // await _updateUsers.ProcessSyncData("Delete", connHR);
        }
        
        private void ProcessPosition(
            StagingDetailDto pos,
            List<Department> treeHRs)
        {
            try
            {
                var sapCode = pos.ObjectId.ToString();
                var hrNode = Helpers.FindTreeHR(sapCode, treeHRs);
                if (hrNode != null)
                {
                    var errorList = new List<string>();
                    var department = new Department();
                    var newData = new NewData();
                    
                    if (pos.ObjectId.Equals("40001021"))
                    {
                        var data = "";
                    }
                    //var findParent = Helpers.FindTreeHRById(hrNode.ParentId ?? Guid.Empty, treeHRs);
                    var sapCodeInHR = "";
                    var find0 = _allStagingDetails.FirstOrDefault(x => x.ObjectId == pos.ParentId);
                    if (pos.ObjectId.Equals("50045959"))
                    {
                        var data = "";
                    }
                    if (pos.ObjectId.Equals("40004274"))
                    {
                        var data = "";
                    }
                    if (find0 != null)
                    {
                        var findS = _allStagingDetails.Where(x => x.ParentId == find0.ObjectId && x.ObjectType.Equals(ObjectType.Position)).ToList();
                        // case 2 position have more than 1 head
                        if (findS.Any() && findS.Count > 1 &&
                            findS.Any(x => x.ObjectId == pos.ObjectId && !x.IsHead.Equals("1")))
                        {
                            sapCodeInHR = findS.FirstOrDefault(x => x.IsHead.Equals("1"))?.ObjectId;

                            // AEONCR112-777 Điều chỉnh nhánh 50026270 - ADMIN (G5)
                            if (sapCodeInHR == null)
                                sapCodeInHR = FindSapCodeInHRForPosition(pos, errorList);
                        }
                        else
                        {
                            // var findS = _allStagingDetails.FirstOrDefault(x => x.ParentId == find0.ParentId && x.ObjectType.Equals(ObjectType.Position) && x.IsHead.Equals("1"));
                            findS = _allStagingDetails.Where(x => x.ParentId == find0.ParentId && x.ObjectType.Equals(ObjectType.Position)).ToList();
                            if (findS.Count() > 1)
                            {
                                var findHead = findS.FirstOrDefault(x => x.IsHead.Equals("1"));
                                sapCodeInHR = findHead != null ? findHead.ObjectId : FindSapCodeInHRForPosition(findS.FirstOrDefault(), errorList);
                                //sapCodeInHR = findHead != null ? findHead.ObjectId : findS.FirstOrDefault()?.ObjectId;
                            }
                            else
                            {
                                //sapCodeInHR = findS.Any() ? findS.FirstOrDefault()?.ObjectId : null;
                                if (findS.Any())
                                {
                                    var currentS = findS.FirstOrDefault();
                                    if (currentS != null)
                                    {
                                        sapCodeInHR = currentS.IsHead.Equals("1") ? currentS.ObjectId : FindSapCodeInHRForPosition(currentS, errorList);
                                    }
                                    //sapCodeInHR = findS.Any() ? findS.FirstOrDefault()?.ObjectId : null;
                                }
                                else
                                {
                                    // AEONCR112-673 Bổ sung case 50038039 (0) không có chứa childrend S
                                    sapCodeInHR = FindSapCodeInHR(pos, _allStagingDetails, errorList);
                                    if (string.IsNullOrEmpty(sapCodeInHR))
                                    {
                                        Logger.Write($"Cannot find ParentId for Position: {pos.ObjectId}", true);
                                        errorList.Add(LogError($"Cannot_Exist_S_with_Position: {pos.ObjectId} - findS: {find0.ParentId}", pos));
                                    }
                                }
                            }
                        }
                        
                        //Logger.Write($"Found SAP Code in HR: {sapCodeInHR} for Position: {find0.ObjectId}", true);
                    }
                    if (hrNode.ParentId != null)
                    {
                        var currentParentInHr = _allDepartments.FirstOrDefault(x => x.Id == hrNode.ParentId);
                        if (currentParentInHr != null)
                            newData.IsParentIsEdoc = currentParentInHr.IsEdoc;
                    }

                    Department findParent = null;
                    if (!string.IsNullOrEmpty(sapCodeInHR))
                    {
                        findParent = Helpers.FindTreeHR(sapCodeInHR, treeHRs);
                        
                        if (findParent == null)
                            findParent = _insertDepartments.FirstOrDefault(x => x.NewData.SAPCode == sapCodeInHR);
                            
                        if (findParent == null)
                            findParent = _updateDepartments.FirstOrDefault(x => x.NewData.SAPCode == sapCodeInHR);
                        
                        if (findParent == null)
                        {
                            Logger.Write($"UP-> Cannot find parent department for Position: SAPCodeInHR: {sapCodeInHR} - {JsonConvert.SerializeObject(pos)}", true);
                            errorList.Add(LogError("UP-> Parent department not found in HR tree.", pos));
                        } 
                        
                        if (findParent != null && findParent.NewData != null && findParent.NewData.ErrorList.Any(x => x.Contains("Cannot_Exist_S_with_Position")))
                        {
                            var data = FindSapCodeInHR(pos, _allStagingDetails, errorList);
                            Logger.Write($"UP-> SAPCodeInHR: {sapCodeInHR} - findParent.Code: {findParent.Code} - Parent has error Cannot_Exist_S_with_Position: - {JsonConvert.SerializeObject(pos)}", true);
                            errorList.Add(LogError($"UP-> Parent has error Cannot_Exist_S_with_Position: - {findParent.Code} - {findParent.Name}", pos));
                            findParent = null;
                        }
                    }
                    else
                    {
                        Logger.Write($"UP-> Cannot find parent department for Position: SAPCodeInHR is null - {JsonConvert.SerializeObject(pos)}", true);
                    }

                    int? currentJobGradeValue = null;
                    if (!string.IsNullOrEmpty(pos.OrgJobgrade))
                    {
                        var currentGrade = _jobGrades.FirstOrDefault(x => x.Title.ToLower().Equals(pos.OrgJobgrade.ToLower()));
                        if (currentGrade != null)
                        {
                            currentJobGradeValue = currentGrade.Grade;
                            newData.JobGradeId = currentGrade.Id;
                            newData.JobGrade = currentGrade.Grade;
                            newData.JobGradeTitle = currentGrade.Title;
                            newData.Type = currentGrade.DepartmentType;
                        }
                        else
                        {
                            errorList.Add(LogError("UP-> Cannot find Job Grade", pos));
                            Logger.Write($"UP-> Position {pos.ObjectId} does not have OrgJobGrade", true);
                        }
                    }
                    else
                    {
                        Logger.Write($"UP-> Position {pos.ObjectId} does not have OrgJobGrade", true);
                        errorList.Add(LogError("UP-> Column OrgJobGrade is required", pos));
                    }

					if (string.IsNullOrEmpty(newData.PositionCode))
	                {
	                    /*var findPosition = _positionLists.FirstOrDefault(x => x.Code.ToLower().Equals(pos.ObjectName.ToLower()));
	                    if (findPosition != null)
	                    {
	                        newData.PositionCode = findPosition.Code;
	                        newData.PositionName = findPosition.Name;
	                    }
	                    else
	                    {*/
	                    if (!string.IsNullOrEmpty(pos.AbbrName))
	                    {
	                        var findPosition = _positionLists.FirstOrDefault(x => x.Code.ToLower().Equals(pos.AbbrName.ToLower()));
	                        if (findPosition != null)
	                        {
	                            newData.PositionCode = findPosition.Code;
	                            newData.PositionName = findPosition.Name;
	                        }
                            else
                            {
                                var newPosition = new Position
                                {
                                    Id = Guid.NewGuid(),
                                    Name = pos.ObjectName,
                                    Code = pos.AbbrName,
                                    JobGradeId = newData.JobGradeId
                                };
                                newData.PositionCode = newPosition.Code;
                                newData.PositionName = newPosition.Name;
                                _insertPositions.Add(newPosition);
                                _positionLists.Add(newPosition);
                            }
	                    }
                        else
                        {
                            errorList.Add(LogError("UP-> Column AbbrName is null", pos));
                        }
	                    //}
                	}
                    
                    if (!hrNode.IsEdoc && !hrNode.IsFromIT)
                    {
                        /*if (hrNode.IsEdoc1)
                        {
                            errorList.Add(LogError($"Department - {hrNode.Code} - {hrNode.Name} is checked IsEdoc1", pos));
                        }*/

                        string businessUnitCode = string.Empty;
                        if (!string.IsNullOrEmpty(pos.SubArea))
                        {
                            var findBusinessModelUnit = _businessModelUnitMappings.FirstOrDefault(x => x.BusinessUnitCode.ToLower().Equals(pos.SubArea.ToLower()));
                            if (findBusinessModelUnit != null)
                            {
                                businessUnitCode = findBusinessModelUnit.BusinessUnitCode;
                                var findBusinessModel = _businessModels.FirstOrDefault(x => x.Id == findBusinessModelUnit.BusinessModelId); 
                                if (findBusinessModel != null)
                                {
                                    newData.BusinessModelId = findBusinessModel.Id;
                                    newData.BusinessModelCode = findBusinessModel.Code;
                                    newData.BusinessModelName = findBusinessModel.Name;
                                    newData.IsStore = findBusinessModelUnit.IsStore;
                                }
                                else
                                {
                                    Logger.Write($"UP-> Cannot find Business Model with id: {findBusinessModelUnit.BusinessModelId} for Position: {pos.ObjectId}", true);
                                    errorList.Add(LogError($"Cannot find Business Model with id: {findBusinessModelUnit.BusinessModelId} ", pos));
                                }
                            }
                            else
                            {
                                Logger.Write($"UP-> Cannot find Business Model Unit with code: {pos.SubArea} for Position: {pos.ObjectId}", true);
                                errorList.Add(LogError($"Cannot find Business Model Mapping with code: {pos.SubArea} ", pos));
                            }
                        }
                        
                        
                        #region  AEONCR112-693Bổ sung rule gen tên department khi đồng bộ cây orgchart eDoc
                        newData.Name = $"{pos.ParentName} ({newData.JobGradeTitle})";
                        if (!string.IsNullOrEmpty(businessUnitCode) && currentJobGradeValue != null)
                        {
                            SetNewDataName(businessUnitCode, currentJobGradeValue, pos, newData);
                        }
                        #endregion
                        
                        department.Id = hrNode.Id;
                        department.Code = pos.ObjectId;
                        department.Name = pos.ParentName;
                        department.SAPCode = pos.ObjectId;
                        
                        newData.Id = hrNode.Id;
                        newData.Code = pos.ObjectId;
                        //newData.Name = pos.ObjectName;
                        newData.SAPCode = pos.ObjectId;
                        //newData.RegionId = findParent?.RegionId;
                        newData.RegionId = hrNode.RegionId;
                        //newData.ParentId = hrNode.ParentId;
                        newData.ParentCode = findParent?.Code;
                        newData.ParentId = findParent?.Id;
                        newData.ParentSapCode = findParent?.SAPCode;
                        newData.ParentName = findParent?.Name;
                        newData.RegionName = hrNode.RegionName;
                        newData.SAPObjectType = pos.ObjectType;
                        newData.SAPDepartmentParentId = pos.ParentId;
                        newData.SAPLevel = pos.Level;
                        newData.SAPObjectId = pos.ObjectId;
                        newData.SAPDepartmentParentName = pos.ParentName;
                        newData.SAPValidFrom = pos.ValidFrom;
                        newData.SAPValidTo = pos.ValidTo;
                        newData.SubArea = pos.SubArea;
                        newData.PersonalArea = pos.PersonalArea;
                        newData.ErrorList = errorList;
                        
                        department.NewData = newData;
                        _updateDepartments.Add(department);
                        foreach (var person in pos.Persons)
                        {
                            try
                            {
                                var personCode = person.ObjectId;
                                UserDepartmentMapping exists = hrNode.Persons.FirstOrDefault(p => p.UserSAPCode == personCode);
                                var countUserInDepartments = hrNode.Persons.Where(p => p.UserSAPCode == personCode).ToList();

                                #region Neu co 2 user trung 1 sap code trong 1 phong ban thi tien hanh chi lay 1 user headcount de update
                                if (countUserInDepartments != null && countUserInDepartments.Count > 1 && countUserInDepartments.Any(x => x.IsHeadCount))
                                    exists = countUserInDepartments.FirstOrDefault(x => x.IsHeadCount);
                                #endregion

                                if (exists != null && (exists.IsEdoc || exists.IsFromIT))
                                {
                                    LogError(
                                        $"Person UserSAPCode: {exists.UserSAPCode} IseDoc = {exists.IsEdoc} - isFromIT = {exists.IsFromIT}",
                                        person);
                                    continue;
                                }
                                
                                errorList = new List<string>();
                                if (string.IsNullOrEmpty(personCode))
                                {
                                    errorList.Add(LogError("Person code is null or empty", person));
                                    Logger.Write($"Person code is null or empty in position {pos.ObjectId}", true);
                                    continue;
                                }
                                
                                var validTo = DateTimeOffset.ParseExact(person.ValidTo, "yyyyMMdd", CultureInfo.InvariantCulture);
                                if (validTo < DateTimeOffset.Now)
                                {
                                    errorList.Add(LogError($"Valid to is Expired: {person.ValidTo} ", person));
                                    Logger.Write($"Valid to is Expired: {person.ValidTo} in position {pos.ObjectId}", true);
                                }

                                if (exists != null)
                                {
                                    if (exists.IsEdoc)
                                    {
                                        errorList.Add(LogError($"User - {exists.FullName} - {exists.UserSAPCode} is checked IsEdoc", person));
                                        Logger.Write($"User - {exists.FullName} - {exists.UserSAPCode} is checked IsEdoc in position {pos.ObjectId}", true);
                                    }
                                    if (exists.IsFromIT)
                                    {
                                        errorList.Add(LogError($"User - {exists.FullName} - {exists.UserSAPCode} is checked IsFromIT", person));
                                        Logger.Write($"User - {exists.FullName} - {exists.UserSAPCode} is checked IsFromIT in position {pos.ObjectId}", true);
                                    }
                                }
                                
                                var targetList = exists != null ? _updateUserDepartment : _insertUserDepartment;

                                var userDepartmentMapping = new UserDepartmentMapping{};
                                userDepartmentMapping.UserSAPCode = personCode;
                                userDepartmentMapping.FullName = person.ObjectName;
                                userDepartmentMapping.LoginName = exists?.LoginName ?? null;
                                userDepartmentMapping.DepartmentId = hrNode.Id;
                                userDepartmentMapping.DepartmentCode = hrNode.Code;
                                userDepartmentMapping.DepartmentName = hrNode.Name;

                                var processDataUDM = new NewDataUDM();
                                processDataUDM.Id = exists?.Id ?? Guid.Empty;
                                processDataUDM.DepartmentId = hrNode.Id;
                                if (exists != null)
                                {
                                    processDataUDM.UserId = exists.UserId;
                                }
                                else
                                {
                                    var findUser = _userLists.FirstOrDefault(u => u.SAPCode == personCode);
                                    if (findUser != null)
                                    {
                                        processDataUDM.UserId = findUser.Id;
                                    }
                                    else
                                    {
                                        errorList.Add(LogError("Cannot find User ", person));
                                        Logger.Write($"Cannot find User with SAPCode: {personCode} in position {pos.ObjectId}", true);
                                    }
                                }
                                
                                processDataUDM.UserSAPCode = personCode;
                                processDataUDM.FullName = person.ObjectName;
                                processDataUDM.LoginName = exists?.LoginName ?? null;
                                processDataUDM.DepartmentCode = hrNode.Code;
                                processDataUDM.DepartmentName = hrNode.Name;
                                processDataUDM.DepartmentEdocSAPCode = hrNode.SAPCode;
                                // processDataUDM.IsHeadCount = !string.IsNullOrEmpty(pos.IsHead) && (pos.IsHead == "1") && pos.Concurrently == "0" && pos.EmployeeId == personCode;
                                processDataUDM.IsConcurrently = pos.Concurrently == "1"; // kiem nhiem
                                processDataUDM.IsHeadCount = !processDataUDM.IsConcurrently && pos.EmployeeId == personCode;
                                processDataUDM.Authorizated = false;
                                processDataUDM.Role = processDataUDM.IsHeadCount ? Group.Member : 0;
                                processDataUDM.ErrorList = errorList;
                                
                                userDepartmentMapping.NewData = processDataUDM;
                                targetList.Add(userDepartmentMapping);
                            }
                            catch (Exception ex)
                            {
                                Logger.Write($"Error processing udm person {person.ObjectName} in position {pos.ObjectId}: {ex.Message}", true);
                                throw new Exception(ex.Message, ex);
                            }
                        }

                        var deleted = hrNode.Persons
                            .Where(p => !pos.Persons.Any(x => x.ObjectId == p.UserSAPCode))
                            .GroupBy(x => x.UserId) // Group theo UserId để loại trùng
                            .Select(g => g.First()) // Lấy phần tử đầu tiên trong mỗi nhóm
                            .Select(x =>
                                new UserDepartmentMapping
                                {
                                    Id = x.Id,
                                    UserId = x.UserId,
                                    LoginName = x.LoginName,
                                    FullName = x.FullName,
                                    IsHeadCount = x.IsHeadCount,
                                    IsEdoc = x.IsEdoc,
                                    IsFromIT = x.IsFromIT,
                                    IsConcurrently = x.IsConcurrently,
                                    UserSAPCode = x.UserSAPCode,
                                    DepartmentCode = hrNode.Code,
                                    DepartmentEdocSAPCode = hrNode.SAPCode,
                                    DepartmentName = hrNode.Name,
                                    DepartmentId = hrNode.Id,
                                    NewData = new NewDataUDM()
                                    {
                                        Id = x.Id,
                                        DepartmentId = hrNode.Id,
                                        DepartmentName = hrNode.Name,
                                        DepartmentEdocSAPCode = hrNode.SAPCode,
                                        UserSAPCode = x.UserSAPCode,
                                        FullName = x.FullName,
                                        LoginName = x.LoginName,
                                        DepartmentCode = hrNode.Code,
                                        IsHeadCount = x.IsHeadCount,
                                        Authorizated = x.Authorizated,
                                        UserId = x.UserId,
                                        ErrorList = new List<string>()
                                    }
                                }
                            );
                        deleted = deleted.Where(x => !x.IsFromIT && !x.IsEdoc);
                        _deleteUserDepartment.AddRange(deleted);
                    }
                    else
                    {
                        LogError(
                            $"Department SAPCode: {hrNode.SAPCode} IseDoc = {hrNode.IsEdoc} - isFromIT = {hrNode.IsFromIT}", pos);
                    }
                }
                else
                {
                    var department = new Department
                    {
                        Id = Guid.NewGuid(),
                        SAPCode = pos.ObjectId,
                        Code = pos.ObjectId,
                        Name = pos.ObjectName,
                    };
                    var newData = new NewData();
                    var errorList = new List<string>();
                    
                    if (pos.ObjectId.Equals("50045959"))
                    {
                        var data = "";
                    }
                    if (pos.ObjectId.Equals("40004167"))
                    {
                        var data = "";
                    }


                    var sapCodeInHR = "";
                    var find0 = _allStagingDetails.FirstOrDefault(x => x.ObjectId == pos.ParentId);
                    if (find0 != null)
                    {
                        var findS = _allStagingDetails.Where(x => x.ParentId == find0.ObjectId && x.ObjectType.Equals(ObjectType.Position)).ToList();
                        // case 2 position have more than 1 head
                        if (findS.Any() && findS.Count > 1 && findS.Any(x => x.ObjectId == pos.ObjectId && !x.IsHead.Equals("1")))
                        {
                            sapCodeInHR = findS.FirstOrDefault(x => x.IsHead.Equals("1"))?.ObjectId;
                            
                            // AEONCR112-777 Điều chỉnh nhánh 50026270 - ADMIN (G5)
                            if (sapCodeInHR == null)
                                sapCodeInHR = FindSapCodeInHRForPosition(pos, errorList);
                        }
                        else
                        {
                            findS = _allStagingDetails.Where(x => x.ParentId == find0.ParentId && x.ObjectType.Equals(ObjectType.Position)).ToList();
                            if (findS.Count() > 1)
                            {
                                var findHead = findS.FirstOrDefault(x => x.IsHead.Equals("1"));
                                sapCodeInHR = findHead != null ? findHead.ObjectId : findS.FirstOrDefault()?.ObjectId;
                            }
                            else
                            {
                                if (findS.Any())
                                {
                                    var currentS = findS.FirstOrDefault();
                                    if (currentS != null)
                                    {
                                        sapCodeInHR = currentS.IsHead.Equals("1") ? currentS.ObjectId : FindSapCodeInHRForPosition(currentS, errorList);
                                    }
                                    //sapCodeInHR = findS.Any() ? findS.FirstOrDefault()?.ObjectId : null;
                                }
                                else
                                {
                                    // AEONCR112-673 Bổ sung case 50038039 (0) không có chứa childrend S
                                    sapCodeInHR = FindSapCodeInHR(pos, _allStagingDetails, errorList);
                                    if (string.IsNullOrEmpty(sapCodeInHR))
                                    {
                                        Logger.Write($"Cannot find ParentId for Position: {pos.ObjectId}", true);
                                        errorList.Add(LogError($"Cannot_Exist_S_with_Position: {pos.ObjectId} - findS: {find0.ParentId}", pos));
                                    }
                                }
                            }
                        }
                    }
                    
                    
                    Department findParent = null;
                    if (!string.IsNullOrEmpty(sapCodeInHR))
                    {
                        findParent = Helpers.FindTreeHR(sapCodeInHR, treeHRs);
                        
                        if (findParent == null)
                            findParent = _insertDepartments.FirstOrDefault(x => x.NewData.SAPCode == sapCodeInHR);
                            
                        if (findParent == null)
                            findParent = _updateDepartments.FirstOrDefault(x => x.NewData.SAPCode == sapCodeInHR);
                    }
                    
                    if (findParent is null)
                    {
                        Logger.Write($"Cannot find parent department for Position: SAPCodeInHR: {sapCodeInHR} - {JsonConvert.SerializeObject(pos)}", true);
                        errorList.Add(LogError("Parent department not found in HR tree.", pos));
                    } 
                    
                    if (findParent != null && findParent.NewData != null && findParent.NewData.ErrorList.Any(x => x.Contains("Cannot_Exist_S_with_Position")))
                    {
                        Logger.Write($"UP-> SAPCodeInHR: {sapCodeInHR} - findParent.Code: {findParent.Code} - Parent has error Cannot_Exist_S_with_Position: - {JsonConvert.SerializeObject(pos)}", true);
                        errorList.Add(LogError($"UP-> Parent has error Cannot_Exist_S_with_Position: - {findParent.Code} - {findParent.Name}", pos));
                        findParent = null;
                    }
                    
                    // AEONCR112-695 Xử lý trường hợp có nếu Dept được tích IsEdoc thì k ParentId của eDoc
                    /*if (findParent != null)
                        newData.IsParentIsEdoc = findParent.IsEdoc;*/

                    int? currentJobGradeValue = null;
                    if (!string.IsNullOrEmpty(pos.OrgJobgrade))
                    {
                        var currentGrade = _jobGrades.FirstOrDefault(x => x.Title.ToLower().Equals(pos.OrgJobgrade.ToLower()));
                        if (currentGrade != null)
                        {
                            currentJobGradeValue = currentGrade.Grade;
                            newData.JobGradeId = currentGrade.Id;
                            newData.JobGrade = currentGrade.Grade;
                            newData.JobGradeTitle = currentGrade.Title;
                            newData.Type = currentGrade.DepartmentType;
                        }
                        else
                        {
                            errorList.Add(LogError("Cannot find Job Grade", pos));
                            Logger.Write($"Position {pos.ObjectId} does not have OrgJobGrade", true);
                        }
                    }
                    else
                    {
                        Logger.Write($"Position {pos.ObjectId} does not have OrgJobGrade", true);
                        errorList.Add(LogError("Column OrgJobGrade is required", pos));
                    }

                    #region Old Position Code Logic

                    /*if (!string.IsNullOrEmpty(pos.AbbrName))
                    {
                        var findPosition = _positionLists.FirstOrDefault(x => x.Code.ToLower().Equals(pos.AbbrName.ToLower()));
                        if (findPosition != null)
                        {
                            newData.PositionCode = findPosition.Code;
                            newData.PositionName = findPosition.Name;
                        }
                        else
                            errorList.Add(LogError($"Cannot find Position with code: {pos.AbbrName} ", pos));
                    }
                    else
                        errorList.Add(LogError("Column AbbrName is null", pos));*/

                    #endregion

                    #region New Position Code Logic
                    if (!string.IsNullOrEmpty(pos.AbbrName))
                    {
                        var findPosition = _positionLists.FirstOrDefault(x => x.Code.ToLower().Equals(pos.AbbrName.ToLower()));
                        if (findPosition != null)
                        {
                            newData.PositionCode = findPosition.Code;
                            newData.PositionName = findPosition.Name;
                        }
                        else
                        {
                            var newPosition = new Position
                            {
                                Id = Guid.NewGuid(),
                                Name = pos.ObjectName,
                                Code = pos.AbbrName,
                                JobGradeId = newData.JobGradeId
                            };
                            newData.PositionCode = newPosition.Code;
                            newData.PositionName = newPosition.Name;
                            _insertPositions.Add(newPosition);
                            _positionLists.Add(newPosition);
                        }
                    }
                    else
                    {
                        Logger.Write($"Position {pos.ObjectId} does not have AbbrName", true);
                        errorList.Add(LogError("Column AbbrName is null", pos));
                    }
                    #endregion

                    string businessUnitCode = string.Empty;
                    if (!string.IsNullOrEmpty(pos.SubArea))
                    {
                        var findBusinessModelUnit = _businessModelUnitMappings.FirstOrDefault(x => x.BusinessUnitCode.ToLower().Equals(pos.SubArea.ToLower()));
                        if (findBusinessModelUnit != null)
                        {
                            businessUnitCode = findBusinessModelUnit.BusinessUnitCode;
                            var findBusinessModel = _businessModels.FirstOrDefault(x => x.Id == findBusinessModelUnit.BusinessModelId); 
                            if (findBusinessModel != null)
                            {
                                newData.BusinessModelId = findBusinessModel.Id;
                                newData.BusinessModelCode = findBusinessModel.Code;
                                newData.BusinessModelName = findBusinessModel.Name;
                                newData.IsStore = findBusinessModelUnit.IsStore;
                            }
                            else
                            {
                                Logger.Write($"Cannot find Business Model with id: {findBusinessModelUnit.BusinessModelId} for Position: {pos.ObjectId}", true);
                                errorList.Add(LogError($"Cannot find Business Model with id: {findBusinessModelUnit.BusinessModelId} ", pos));
                            }
                        }
                        else
                        {
                            Logger.Write($"Cannot find Business Model Unit with code: {pos.SubArea} for Position: {pos.ObjectId}", true);
                            errorList.Add(LogError($"Cannot find Business Model Mapping with code: {pos.SubArea} ", pos));
                        }
                    }

                    #region  AEONCR112-693Bổ sung rule gen tên department khi đồng bộ cây orgchart eDoc
                    newData.Name = $"{pos.ParentName} ({newData.JobGradeTitle})";
                    if (!string.IsNullOrEmpty(businessUnitCode) && currentJobGradeValue != null)
                    {
                        SetNewDataName(businessUnitCode, currentJobGradeValue, pos, newData);
                    }
                    #endregion
                    
                    newData.Id = department.Id;
                    newData.Code = pos.ObjectId;
                    newData.ParentId = findParent?.Id;
                    newData.ParentCode = findParent?.Code;
                    newData.ParentSapCode = findParent?.SAPCode;
                    newData.ParentName = findParent?.Name;
                    newData.SAPCode = pos.ObjectId;
                    newData.RegionId = findParent?.RegionId;
                    newData.RegionName = findParent?.RegionName;
                    newData.SAPObjectType = pos.ObjectType;
                    newData.SAPDepartmentParentId = pos.ParentId;
                    newData.SAPLevel = pos.Level;
                    newData.SAPObjectId = pos.ObjectId;
                    newData.SAPDepartmentParentName = pos.ParentName;
                    newData.SAPValidFrom = pos.ValidFrom;
                    newData.SAPValidTo = pos.ValidTo;
                    newData.SubArea = pos.SubArea;
                    newData.PersonalArea = pos.PersonalArea;
                    newData.SubArea = pos.SubArea;
                    newData.PersonalArea = pos.PersonalArea;
                    newData.ErrorList = errorList;
                    
                    department.NewData = newData;
                    _insertDepartments.Add(department);
                }   
            } catch (Exception e)
            {
                Logger.Write($"Error processing position {pos.ObjectId}: {e.Message} - {e.InnerException.Message}", true);
                throw new Exception($"Error processing position {pos.ObjectId}: {e.Message}");
            }
        }
        
        private string FindSapCodeInHRForPosition(StagingDetailDto pos, 
            List<string> errorList)
        {
            // Tìm Organization cha của Position
            var findO = _allStagingDetails
                .FirstOrDefault(x => x.ObjectId == pos.ParentId && 
                                     x.ObjectType.Equals(ObjectType.Organization));
            if (findO == null)
                return null;

            // Tìm tất cả Position cùng ParentId
            var findS = _allStagingDetails
                .Where(x => x.ParentId == findO.ParentId && 
                            x.ObjectType.Equals(ObjectType.Position))
                .ToList();

            string sapCodeInHR = null;
            if (findS.Count > 1)
            {
                // Nếu có nhiều position thì ưu tiên Head
                var findIsHead = findS.FirstOrDefault(x => x.IsHead.Equals("1"));
                // AEONCR112-792: Check và fix nhánh MD & OP MY CLOSET (G4): Dept code: DEP4077003, SAPcode:40004192
                sapCodeInHR = findIsHead != null ? findIsHead.ObjectId : FindSapCodeInHRForPosition(findS.FirstOrDefault(), errorList);
                /*sapCodeInHR = findHead != null 
                    ? findHead.ObjectId 
                    : findS.FirstOrDefault()?.ObjectId;*/
            }
            else
            {
                if (findS.Any())
                {
                    var currentS = findS.FirstOrDefault();
                    if (currentS != null)
                    {
                        // AEONCR112-792: Check và fix nhánh MD & OP MY CLOSET (G4): Dept code: 40004274, SAPcode: 40004274
                        sapCodeInHR = currentS.IsHead.Equals("1") ? currentS.ObjectId : FindSapCodeInHRForPosition(currentS, errorList);
                    }
                    //sapCodeInHR = findS.FirstOrDefault()?.ObjectId;
                }
                else
                {
                    // AEONCR112-673: Bổ sung case không có S con
                    sapCodeInHR = FindSapCodeInHR(pos, _allStagingDetails, errorList);

                    if (string.IsNullOrEmpty(sapCodeInHR))
                    {
                        Logger.Write($"Cannot find ParentId for Position: {pos.ObjectId}", true);
                        errorList.Add(
                            LogError($"Cannot_Exist_S_with_Position: {pos.ObjectId} - findS: {findO.ParentId}", pos)
                        );
                    }
                }
            }

            return sapCodeInHR;
        }
        
        private void SetNewDataName(
            string businessUnitCode, 
            int? currentJobGradeValue, 
            StagingDetailDto pos, 
            NewData newData)
        {
            if (string.IsNullOrEmpty(businessUnitCode) || currentJobGradeValue == null)
                return;

            var grade4 = _jobGrades.FirstOrDefault(x => x.Title.Equals("G4", StringComparison.CurrentCulture));
            var grade3 = _jobGrades.FirstOrDefault(x => x.Title.Equals("G3", StringComparison.CurrentCulture));

            var findSubArea = _masterDatasWorkLocation
                .FirstOrDefault(x => x.Code.Equals(businessUnitCode, StringComparison.CurrentCultureIgnoreCase));

            var subAreaName = findSubArea?.Name ?? string.Empty;

            if (businessUnitCode.StartsWith("4") && grade3 != null && currentJobGradeValue <= grade3?.Grade)
            {
                if (!string.IsNullOrEmpty(subAreaName))
                    newData.Name = $"{pos.ParentName} - {subAreaName} ({newData.JobGradeTitle})";
                else
                    newData.Name = $"{pos.ParentName} ({newData.JobGradeTitle})";
            }
            else if (businessUnitCode.StartsWith("1") && grade4 != null && currentJobGradeValue <= grade4?.Grade)
            {
                if (!string.IsNullOrEmpty(subAreaName))
                    newData.Name = $"{pos.ParentName} - {subAreaName} ({newData.JobGradeTitle})";
                else
                    newData.Name = $"{pos.ParentName} ({newData.JobGradeTitle})";
            }
        }
        
        private string FindSapCodeInHR(StagingDetailDto pos, List<StagingDetailDto> allDetails, List<string> errorList)
        {
            // tìm node hiện tại
            var current = allDetails.FirstOrDefault(x => x.ObjectId == pos.ParentId);
            if (current == null)
            {
                Logger.Write($"Cannot find ParentId for Position: {pos.ObjectId}", true);
                errorList.Add(LogError($"Cannot_Exist_S_with_Position: {pos.ObjectId} - ParentId: {pos.ParentId}", pos));
                return null;
            }

            // nếu đã tới S thì dừng
            if (current.ObjectType.Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                return current.ObjectId;
            }

            // nếu cha có nhiều Position
            var siblings = allDetails
                .Where(x => x.ParentId == current.ParentId && x.ObjectType.Equals(ObjectType.Position))
                .ToList();

            if (siblings.Count > 1)
            {
                // ưu tiên head
                var head = siblings.FirstOrDefault(x => x.IsHead == "1");
                return head?.ObjectId ?? siblings.First().ObjectId;
            }

            if (siblings.Count == 1)
            {
                return siblings.First().ObjectId;
            }

            // nếu chưa tìm được thì gọi đệ quy lên cha
            return FindSapCodeInHR(current, allDetails, errorList);
        }

        private void ProcessDeleteDepartments(List<Department> hrTree)
        {
            foreach (var dept in hrTree)
            {
                if (!_newDepartmentCodes.Contains(dept.SAPCode))
                {
                    var errorList = new List<string>();

                    if (dept.IsFromIT || dept.IsEdoc)
                    {
                        Logger.Write($"Case Delete Id: {dept.Id} - SapCode: {dept.SAPCode}: IsFromIT = {dept.IsFromIT} - IsEdoc = {dept.IsEdoc}");
                    }
                    
                    /*if (dept.IsEdoc1)
                    {
                        errorList.Add($"Department - {dept.Code} - {dept.Name} is checked IsFromIT");
                    }*/
                    
                    if (!dept.IsEdoc && !dept.IsFromIT)
                    {
                        var findParent = _allDepartments.FirstOrDefault(x => x.Id == dept.ParentId);
                        var newData = new NewData
                        {
                            Id = dept.Id,
                            Code = dept.Code,
                            Name = dept.Name,
                            SAPCode = dept.SAPCode,
                            RegionId = dept?.RegionId,
                            SAPObjectType = dept.SAPObjectType,
                            SAPDepartmentParentId = dept.ParentId.ToString(),
                            SAPLevel = dept.SAPLevel,
                            SAPObjectId = dept.SAPObjectId,
                            SAPDepartmentParentName = dept.SAPDepartmentParentName,
                            SAPValidFrom = dept.SAPValidFrom,
                            SAPValidTo = dept.SAPValidTo,
                            JobGradeId = dept.Id,
                            Type = dept.Type,
                            ParentId = findParent?.Id,
                            ParentCode = findParent?.Code,
                            ParentSapCode = findParent?.SAPCode,
                            ParentName = findParent?.Name,
                            PositionCode = dept.PositionCode,
                            JobGrade = dept.Grade,
                            JobGradeTitle = dept.GradeTile,
                            RegionName = dept.RegionName,
                            PositionName = dept.PositionName,
                            ErrorList = errorList
                        };
                    
                        dept.NewData = newData;
                        _deleteDepartments.Add(dept);
                        // AEONCR112-820 Dept bị xóa sau khi sync orgchart vẫn hiển thị khi làm phiếu
                        if (dept.Persons != null && dept.Persons.Any())
                        {
                            var persons = dept.Persons
                                .Select(x => new UserDepartmentMapping
                                {
                                    Id = x.Id,
                                    UserId = x.UserId,
                                    LoginName = x.LoginName,
                                    FullName = x.FullName,
                                    IsHeadCount = x.IsHeadCount,
                                    IsEdoc = x.IsEdoc,
                                    IsFromIT = x.IsFromIT,
                                    NewData = new NewDataUDM()
                                    {
                                        Id = x.Id,
                                        DepartmentId = dept.Id,
                                        DepartmentName = dept.Name,
                                        DepartmentEdocSAPCode = dept.SAPCode,
                                        UserSAPCode = x.UserSAPCode,
                                        FullName = x.FullName,
                                        LoginName = x.LoginName,
                                        DepartmentCode = dept.Code,
                                        IsHeadCount = x.IsHeadCount,
                                        Authorizated = x.Authorizated,
                                        UserId = x.UserId,
                                        ErrorList = new List<string>()
                                    }
                                }).ToList();
                            _deleteUserDepartment.AddRange(persons);
                        }
                    }
                }

                if (dept.Children != null && dept.Children.Any())
                {
                    ProcessDeleteDepartments(dept.Children);
                }
            }
        }

        private void ProcessUsers(List<StagingDetailDto> allPercentLists)
        {
            foreach (var person in allPercentLists)
            {
                var user = _userLists.FirstOrDefault(x => x.SAPCode == person.ObjectId);
                if (user != null)
                {
                    try
                    {
                        if (user.IsEdoc || user.IsFromIT)
                        {
                            LogError($"User SAPCode: {user.SAPCode} IseDoc = {user.IsEdoc} - isFromIT = {user.IsFromIT}", person);
                            continue;
                        }
                        
                        var newData = new NewDataUser()
                        {
                            Id = user.Id,
                            FullName = person.ObjectName,
                            LoginName = user.LoginName,
                            SAPCode = person.ObjectId,
                            Type = LoginType.Membership,
                            Role = UserRole.Member
                        };
                        if (!string.IsNullOrEmpty(person.ValidFrom))
                        {
                            try
                            {
                                DateTimeOffset startDate =
                                    DateTime.ParseExact(person.ValidFrom, "yyyyMMdd", CultureInfo.InvariantCulture);
                                newData.StartDate = startDate.Date;
                            }
                            catch (Exception e)
                            {
                                var errorMessage = LogError($"Error parsing ValidFrom: {person.ValidFrom} - {e.Message}", person);
                                Console.WriteLine(errorMessage);
                                newData.ErrorList.Add(errorMessage);
                            }
                        }

                        bool inactiveUser = false;
                        if (!string.IsNullOrEmpty(person.ValidTo))
                        {
                            try
                            {
                                DateTimeOffset endDate =
                                    DateTime.ParseExact(person.ValidTo, "yyyyMMdd", CultureInfo.InvariantCulture);
                                // inactiveUser = endDate < DateTimeOffset.Now;

                                #region AEONCR112-821 Bổ sung thêm rule để inactive user sau khi sync orgchart
                                inactiveUser = endDate.AddDays(14) < DateTimeOffset.Now.Date;
                                #endregion
                            }
                            catch (Exception e)
                            {
                                var errorMessage = LogError($"Error parsing ValidTo: {person.ValidTo} - {e.Message}", person);
                                Console.WriteLine(errorMessage);
                                newData.ErrorList.Add(errorMessage);
                            }
                        }

                        if (inactiveUser)
                        {
                            newData.ErrorList.Add($"Inactive User");
                            user.NewData = newData;
                            _updateUsers.Add(user);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else
                {
                    try
                    {
                        user = new User
                        {
                            Id = Guid.Empty,
                            LoginName = person.ObjectName,
                            SAPCode = person.ObjectId.ConvertToSAP()
                        };
                        var newData = new NewDataUser()
                        {
                            Id = user.Id,
                            FullName = person.ObjectName,
                            LoginName = user.LoginName,
                            SAPCode = person.ObjectId,
                            Type = LoginType.Membership,
                            Role = UserRole.Member
                        };
                        if (!string.IsNullOrEmpty(person.ValidFrom))
                        {
                            try
                            {
                                DateTimeOffset startDate =
                                    DateTime.ParseExact(person.ValidFrom, "yyyyMMdd", CultureInfo.InvariantCulture);
                                newData.StartDate = startDate.Date;   
                            } catch (Exception e)
                            {
                                var errorMessage = LogError($"Error parsing ValidFrom: {person.ValidFrom} - {e.Message}", person);
                                Console.WriteLine(errorMessage);
                                newData.ErrorList.Add(errorMessage);
                            }
                        }
                    
                        user.NewData = newData;
                        _insertUsers.Add(user);
                    }
                    catch (Exception e)
                    {
                        Logger.Write($"Error processing CR user {person.ObjectName}: {e.Message}", true);
                        throw new Exception($"Error processing CR user {person.ObjectName}: {e.Message}");
                    }
                }
            }
        }

        private string LogError(string message, StagingDetailDto pos)
        {
            return $"{message} for ObjectId: {pos.ObjectId} - ObjectName: {pos.ObjectName} - ObjectType: {pos.ObjectType} - ParentId: {pos.ParentId}.";
        }

        private async Task UpdateHeaderStatusAsync(SqlConnection conn, SqlTransaction sqlTransaction, int headerId, string status, string note)
        {
            var query = @"UPDATE Staging_Header SET Status = @status, Note = @note WHERE Id = @id";
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Transaction = sqlTransaction;
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@note", (object)note ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", headerId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
