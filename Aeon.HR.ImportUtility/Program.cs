using Aeon.HR.API.Helpers;
using Aeon.HR.BusinessObjects.Handlers.ExternalBO;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.Data;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.ExternalItem;
using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = Aeon.HR.Infrastructure.Enums.Group;

namespace Aeon.HR.ImportUtility
{
    class Program
    {
        private static object _lock = new object();
        private static Dictionary<string, ReferenceNumber> _refs;
        private const int BEGIN_INDEX = 0;
        private const int END_INDEX = 15;
        private const int POS_CODE_INDEX = 16;
        private const int CODE_IDX = 17;
        private const int JOB_GRADE_IDX = 18;
        private const int STORE_IDX = 23;
        private const int HR_IDX = 24;
        private const int ROW_INDEX = 0;

        static void Main(string[] args)
        {
            Console.Write("Loading Data........");
            //ImportData();
            //ImportPosData();
            //FixData();
            // MigratedData();
            //MigrateMissingTimeClockDetail();
            MigrateTrackingLogInitData();
            //FindSubmitPersonFromDepartmentId(new List<Guid>() {new Guid("D7FB9912-D108-41A4-9749-63D9140C02C9")});
            //MigrateUpdatePermissionUserInTargetPlanItem();
            //CreatePeriodBTAInListRequest();
            //UpdatePRDReamain();
            Console.Write("Done");
            Console.Read();
        }
        private static void UpdatePRDReamain()
        {
            var userTest = "00263337";
            using (var ctx = new HRDbContext())
            {
                var users = ctx.Users.Where(x => x.IsActivated && !x.IsDeleted).ToList();
                foreach (var item in users)
                {
                    var isStore = ctx.UserDepartmentMappings.Any(x => x.UserId == item.Id && x.IsHeadCount && x.Department.IsStore);
                    if (isStore)
                    {
                        var resignation = ctx.ResignationApplications.Where(x => x.UserSAPCode == item.SAPCode && x.Status.Contains("Completed")).OrderByDescending(x => x.OfficialResignationDate).FirstOrDefault();
                        var targetPlans = ctx.TargetPlanDetails.Where(x => x.SAPCode == item.SAPCode && x.TargetPlan.Status == "Completed");
                        if (targetPlans.Any())
                        {
                            var dataPRD = new RedundantPRDData();
                            var target1Data = targetPlans.Where(x => x.Type == TypeTargetPlan.Target1).ToList();
                            if (target1Data.Any())
                            {
                                foreach (var target in target1Data)
                                {
                                    var qualityPRDInCurrentMonth = 0.0;
                                    var periodFrom = target.TargetPlan.PeriodFromDate;
                                    var periodTo = target.TargetPlan.PeriodToDate;
                                    var startD = periodFrom;
                                    var endD = periodTo;
                                    if (item.StartDate.HasValue)
                                    {
                                        if (item.StartDate > periodFrom && item.StartDate < periodTo)
                                        {
                                            startD = item.StartDate.Value.Date;
                                        }
                                        else if (item.StartDate > periodTo)
                                        {
                                            startD = periodTo;
                                        }
                                    }
                                    if (resignation != null)
                                    {
                                        var officialDate = resignation.OfficialResignationDate.Date;
                                        if (officialDate > item.StartDate.Value.Date && officialDate < endD)
                                        {
                                            endD = officialDate;
                                        }

                                    }
                                    while (startD <= endD)
                                    {
                                        if (startD.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            qualityPRDInCurrentMonth++;
                                        }
                                        startD = startD.AddDays(1);
                                    }
                                    //if (qualityPRDInCurrentMonth > target.PRDQuality)
                                    //{
                                    //    qualityPRDInCurrentMonth = qualityPRDInCurrentMonth - target.PRDQuality.Value;
                                    //}
                                    dataPRD.JsonData.Add(new RedundantPRD
                                    {
                                        Year = periodTo.Year,
                                        Month = periodTo.Month,
                                        PRDRemain = qualityPRDInCurrentMonth == 5 && qualityPRDInCurrentMonth - target.PRDQuality.Value > 0 ? qualityPRDInCurrentMonth - target.PRDQuality.Value : 0
                                    }); ;
                                }
                                item.RedundantPRD = JsonConvert.SerializeObject(dataPRD);
                                Console.WriteLine(string.Format("SAPCode: {0}: Data: {1}", item.SAPCode, item.RedundantPRD));
                            }
                        }
                    }

                }
                ctx.SaveChanges();
            }
        }

        private static void CreatePeriodBTAInListRequest()
        {
            using (var ctx = new HRDbContext())
            {
                var allBTAItems = ctx.BusinessTripApplications.ToList();
                foreach (var item in allBTAItems)
                {
                    var listBusinessTrip = item.BusinessTripApplicationDetails;
                    if (item.Status == "Completed Changing")
                    {
                        var changeCancelBusinessTripDetails = ctx.ChangeCancelBusinessTripDetails.Where(x => x.BusinessTripApplicationId.HasValue && x.BusinessTripApplicationId == item.Id).ToList();
                        foreach (var changeCancelBusinessTripDetail in changeCancelBusinessTripDetails)
                        {
                            if (!changeCancelBusinessTripDetail.IsCancel)
                            {
                                var oldItem = listBusinessTrip.FirstOrDefault(x => x.BusinessTripApplicationId == changeCancelBusinessTripDetail.BusinessTripApplicationId);
                                oldItem.FromDate = changeCancelBusinessTripDetail.FromDate;
                                oldItem.ToDate = changeCancelBusinessTripDetail.ToDate;
                            }
                        }
                    }
                    item.BusinessTripFrom = listBusinessTrip.OrderBy(x => x.FromDate).FirstOrDefault().FromDate;
                    item.BusinessTripTo = listBusinessTrip.OrderByDescending(x => x.ToDate).FirstOrDefault().ToDate;
                }
                ctx.SaveChanges();
            }
        }
        private static void MigrateUpdatePermissionUserInTargetPlanItem()
        {
            using (var ctx = new HRDbContext())
            {
                var addUserCount = 0;
                var exisUserCount = 0;
                var listMigrateUsers = new List<Permission>();
                var users = ctx.Users.Where(x => x.IsActivated && !x.IsDeleted).ToList();
                var allTargetPlanDetails = ctx.TargetPlanDetails.ToList().GroupBy(x => new { x.SAPCode, x.TargetPlanId }).Select(y => y.FirstOrDefault());
                foreach (var item in allTargetPlanDetails)
                {
                    Console.WriteLine($"SAPCode: {item.SAPCode}");
                    var user = users.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                    if (user != null)
                    {
                        if (!ctx.Permissions.Any(x => x.ItemId == item.TargetPlanId && x.UserId == user.Id))
                        {
                            addUserCount++;
                            listMigrateUsers.Add(new Permission
                            {
                                Id = Guid.NewGuid(),
                                Created = DateTime.Now,
                                Modified = DateTime.Now,
                                DepartmentId = null,
                                DepartmentType = 0,
                                Perm = Right.View,
                                ItemId = item.TargetPlanId,
                                UserId = user.Id
                            });
                        }
                        else
                        {
                            exisUserCount++;
                        }
                    }


                }
                ctx.Permissions.AddRange(listMigrateUsers);
                ctx.SaveChanges();
                Console.WriteLine($"Completed Migrate: Sum: {addUserCount + exisUserCount} ,Added: {addUserCount}, Exist: {exisUserCount}");
            }
        }
        private static void MigratedData()
        {
            using (var ctx = new HRDbContext())
            {

                var rths = ctx.RequestToHires.OrderBy(x => x.ReferenceNumber).ToList();
                foreach (var rth in rths)
                {
                    Console.WriteLine("Fix data for {0}", rth.ReferenceNumber);
                    if (rth.DeptDivisionId.HasValue)
                    {
                        var dept = LoadDept(ctx, rth.DeptDivisionId.Value);
                        rth.DeptCode = dept.Code;
                        rth.DeptName = dept.Name;
                    }
                }
                var prs = ctx.PromoteAndTransfers.OrderBy(x => x.ReferenceNumber).ToList();
                foreach (var pr in prs)
                {
                    Console.WriteLine("Fix data for {0}", pr.ReferenceNumber);
                    if (pr.NewDeptOrLineId.HasValue)
                    {
                        var dept = LoadDept(ctx, pr.NewDeptOrLineId.Value);
                        pr.TargetDeptCode = dept.Code;
                        pr.TargetDeptName = dept.Name;
                    }
                    if (pr.CurrentDepartmentId.HasValue)
                    {
                        var dept = LoadDept(ctx, pr.CurrentDepartmentId.Value);
                        pr.DeptCode = dept.Code;
                        pr.DeptName = dept.Name;
                    }
                }
                ctx.SaveChanges();
            }
        }
        public static Department LoadDept(DbContext ctx, Guid deptId)
        {
            try
            {
                var department = ctx.Set<Department>().Where(x => x.Id == deptId).FirstOrDefault();
                while (department != null && department.ParentId.HasValue && department.JobGrade.Grade < 5)
                {
                    var lastDept = ctx.Set<Department>().Where(x => x.Id == department.ParentId && !x.IsDeleted).FirstOrDefault();
                    if (lastDept != null)
                    {
                        department = lastDept;
                    }
                }
                return department;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static void FixData()
        {
            List<string> fixItems = new List<string>();
            List<LeaveQuotaMapping> LeaveQuotaMappings;
            using (var ctx = new HRDbContext())
            {
                var trackingItems = ctx.TrackingRequests.Where(x => x.Created >= new DateTime(2020, 08, 10) && x.ReferenceNumber.Contains("LEA")).ToList();
                var groupItems = trackingItems.GroupBy(x => x.ReferenceNumber).Select(x => new { Ref = x.Key, TotalItems = x.Count() });
                var fixedItems = groupItems.Where(x => x.TotalItems >= 2).Select(x => x.Ref).Distinct().ToList();
                var leaveQuotaMappings = JsonHelper.GetJsonContentFromFile("Mappings", "leave-quota-mapping.json");
                if (!string.IsNullOrEmpty(leaveQuotaMappings))
                {
                    var quotas = JsonConvert.DeserializeObject<List<LeaveQuotaMapping>>(leaveQuotaMappings);
                    if (quotas.Any())
                    {
                        LeaveQuotaMappings = quotas;
                    }
                }

                var leaves = ctx.LeaveApplications.Where(x => fixedItems.Contains(x.ReferenceNumber)).ToList();
                foreach (var model in leaves)
                {
                    var submittedItems = new List<LeaveApplicationInfo>();
                    var items = ctx.LeaveApplicationDetails.Where(x => x.LeaveApplicationId == model.Id).ToList();
                    foreach (var item in items)
                    {
                        var dataInfo = new LeaveApplicationInfo
                        {
                            EmployeeCode = model.UserSAPCode,
                            UserEdoc = model.CreatedBy
                        };
                        dataInfo.FromDate = item.FromDate.LocalDateTime.ToSAPFormat();
                        dataInfo.ToDate = item.ToDate.LocalDateTime.ToSAPFormat();
                        if (item.FromDate.DateTime == item.ToDate.DateTime)
                        {
                            dataInfo.LeaveKind = item.LeaveCode;
                            submittedItems.Add(dataInfo);
                        }
                        else
                        {
                            dataInfo.FromDate = dataInfo.FromDate;
                            dataInfo.ToDate = dataInfo.ToDate;
                            dataInfo.LeaveKind = item.LeaveCode;
                            submittedItems.Add(dataInfo);
                        }
                    }
                    for (var j = 0; j < submittedItems.Count; j++)
                    {

                        var payload = BuildJsonFromObject(submittedItems[j]);
                        var data = trackingItems.Where(x => x.Payload == payload && x.ReferenceNumber == model.ReferenceNumber).ToList();
                        if (data.Count() == submittedItems.Count())
                        {
                            for (var i = data.Count() - 1; i >= 0; i--)
                            {
                                if (i != j)
                                {
                                    data[i].Payload = BuildJsonFromObject(submittedItems[i]);
                                    data[i].Status = "Failed";
                                    fixItems.Add(model.ReferenceNumber);
                                }
                            }
                            break;
                        }
                    }
                }
                File.WriteAllText("C:\\abc.txt", string.Join(",", fixItems.Distinct().ToList()));
                //ctx.SaveChanges();
            }
        }
        private static string BuildJsonFromObject(ISAPEntity data)
        {
            var jsonData = JsonHelper.GetJsonContentFromFile("Mappings", "leaveBalanceInfo.json");
            var mappingFiles = GenericExtension<List<FieldMappingDTO>>.DeserializeObject(jsonData);
            return ProcessingFields(mappingFiles, data, TypeProcessingField.Push);
        }

        protected static string ProcessingFields(List<FieldMappingDTO> fields, object data, TypeProcessingField type)
        {
            var result = "";
            if (fields.Count > 0)
            {
                if (type == TypeProcessingField.Get)
                {
                    result = data.ToString();
                    foreach (var item in fields)
                    {
                        result = result.SafeReplace(item.TargetField, item.SourceField, true);
                    }
                }
                else
                {
                    result = JsonConvert.SerializeObject(data);
                    foreach (var item in fields)
                    {
                        result = result.SafeReplace(item.SourceField, item.TargetField, true);
                    }
                }

            }
            return result;
        }

        private static void ImportPosData()
        {
            using (var ctx = new HRDbContext())
            {
                _refs = ctx.ReferenceNumbers.ToDictionary(x => x.ModuleType, x => x);
                var costCenters = ctx.CostCenterRecruitments.ToDictionary(x => x.Code, x => x);
                using (var reader = new StreamReader("pos.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    using (var dr = new CsvDataReader(csv))
                    {
                        var dt = new DataTable();
                        dt.Load(dr);
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            var sapcode = Convert.ToString(row[3]);
                            var dept = ctx.Departments.FirstOrDefault(x => x.SAPCode.Contains(sapcode) && x.IsDeleted == false);
                            var caption = Convert.ToString(row[4]);
                            var jd = ctx.JobGrades.FirstOrDefault(x => x.Caption == caption && x.IsDeleted == false);
                            var usersapCode = Convert.ToString(row[15]);
                            var user = ctx.Users.FirstOrDefault(x => x.SAPCode.Contains(usersapCode) && x.IsDeleted == false);
                            var posstr = Convert.ToString(row[1]).Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault<string>().Trim();
                            var pos = ctx.MasterDatas.FirstOrDefault(x => x.MetadataType.Name == "Position" && x.Name == posstr && x.IsDeleted == false);
                            var otherReason = Convert.ToString(row[16]);
                            var cost = Convert.ToString(row[17]).Trim();
                            if (!costCenters.ContainsKey(cost))
                            {
                                Console.WriteLine("Cannot import pos " + row[0] + ", missing cost center  " + cost);
                                continue;
                            }

                            if (user == null)
                            {
                                Console.WriteLine("Cannot import pos " + row[0] + ", missing user  " + usersapCode);
                                continue;
                            }
                            if (jd == null)
                            {
                                Console.WriteLine("Cannot import pos " + row[0] + ", missing  jd " + caption);
                                continue;
                            }
                            if (dept == null)
                            {
                                Console.WriteLine("Cannot import pos " + row[0] + ", missing  dept " + sapcode);
                                continue;
                            }
                            if (pos == null)
                            {
                                Console.WriteLine("Cannot import pos " + row[0] + ", missing  pos " + posstr);
                                continue;
                            }
                            var item = new
                            {
                                AssignToId = user.Id,
                                DeptDivisionId = dept.Id,
                                ExpiredDay = (!string.IsNullOrEmpty(Convert.ToString(row[5])) ? Convert.ToInt32(row[5]) : 0),
                                ExpiredDate = (string.IsNullOrEmpty(Convert.ToString(row[9])) ? DateTime.Now.AddMonths(1) : DateTime.Parse(Convert.ToString(row[9]), new CultureInfo("vi-VN"))),
                                HasBudget = (!string.IsNullOrEmpty(Convert.ToString(row[10])) ? Convert.ToInt32(row[10]) : 0),
                                Quantity = (!string.IsNullOrEmpty(Convert.ToString(row[12])) ? Convert.ToInt32(row[12]) : 1),
                                LocationCode = Convert.ToString(row[6]),
                                LocationName = Convert.ToString(row[6]),
                                Status = PositionStatus.Opened,
                                PositionName = pos.Name,
                                PosCode = pos.Code,
                                posId = pos.Id,
                                IsStore = Convert.ToString(row[13]) == "STORE",
                                DepartmentName = Convert.ToString(row[16]),
                                NewPosition = int.Parse(Convert.ToString(row[2])),
                                ReasonOptions = ReasonOptions.Resign,
                                CostId = costCenters[cost].Id
                            };
                            if ((TypeOfNeed)item.NewPosition == TypeOfNeed.NewPosition && dept.JobGradeId == jd.Id)
                            {
                                Console.WriteLine("Cannot import pos " + row[0] + ", same job grade");
                                continue;
                            }
                            if ((TypeOfNeed)item.NewPosition == TypeOfNeed.ReplacementFor)
                            {
                                var deptJg = ctx.JobGrades.FirstOrDefault(x => x.Id == dept.JobGradeId);
                                if (deptJg.Grade != jd.Grade)
                                {
                                    Console.WriteLine("Cannot import pos " + row[0] + ", replacement for does not have same job grade");
                                    continue;
                                }
                            }
                            //Create request to hire
                            var requestToHire = new RequestToHire()
                            {
                                Id = Guid.NewGuid(),
                                AssignToId = new Guid?(item.AssignToId),
                                LocationCode = item.LocationCode,
                                LocationName = item.LocationName,
                                ContractTypeCode = "FT",
                                ContractTypeName = "Full time",
                                ExpiredDayPosition = item.ExpiredDay,
                                Status = "Completed",
                                JobGradeCaption = jd.Caption,
                                JobGradeId = jd.Id,
                                PositionCode = pos.Code,
                                Quantity = item.Quantity,
                                PositionName = pos.Name,
                                PositionId = new Guid?(pos.Id),
                                JobGradeGrade = new int?(jd.Grade),
                                HasBudget = (item.HasBudget == 1 ? CheckBudgetOption.Budget : CheckBudgetOption.Non_Budget),
                                IsStore = item.IsStore,
                                DeptDivisionName = dept.Name,
                                DeptDivisionGrade = jd.Grade.ToString(),
                                DeptDivisionId = new Guid?(item.DeptDivisionId),
                                ReplacementForId = new Guid?(item.DeptDivisionId),
                                DepartmentName = item.DepartmentName,
                                NewPosition = item.NewPosition == 1,
                                DeptDivisionCode = dept.Code,
                                Created = DateTime.Now,
                                Modified = DateTime.Now,
                                ReplacementFor = (TypeOfNeed)item.NewPosition,
                                ReplacementForGrade = jd.Grade,
                                ReferenceNumber = GenerateReferenceNumber("RequestToHire"),
                                Reason = item.ReasonOptions,
                                CostCenterRecruitmentId = item.CostId
                            };
                            ctx.RequestToHires.Add(requestToHire);
                            ctx.Permissions.Add(new Permission()
                            {
                                Id = Guid.NewGuid(),
                                Created = DateTime.Now,
                                Modified = DateTime.Now,
                                UserId = item.AssignToId,
                                ItemId = pos.Id,
                                Perm = Right.View
                            });
                            Execute(ctx, requestToHire);
                            ctx.SaveChanges();
                        }
                    }
                }
            }
        }
        private static void ImportData()
        {
            using (var ctx = new HRDbContext())
            {
                var jobGrades = ctx.JobGrades.ToDictionary(x => x.Caption, x => x);
                var posMappings = GetPositionMappings();
                var users = GetUsers();
                var oIndexes = new Dictionary<int, string>(); var pIndexes = new Dictionary<int, string>();
                var gIndexes = new Dictionary<int, Department>();
                var dIndexes = new Dictionary<int, Department>();
                var prIndexes = new Dictionary<Guid, string>();
                var levels = new List<Department>();
                var listUserMapping = new List<UserDepartmentMapping>();
                var listUser = new List<User>();
                var lastIndex = 0;
                var lastCode = "";
                var lastIsStore = false;
                Department lastDepartment = null;
                using (var reader = new StreamReader("ORG.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    using (var dr = new CsvDataReader(csv))
                    {
                        var dt = new DataTable();
                        dt.Load(dr);
                        for (var i = ROW_INDEX; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            var code = Convert.ToString(row[CODE_IDX]);
                            var jobGrade = Convert.ToString(row[JOB_GRADE_IDX]);
                            var positionCode = Convert.ToString(row[POS_CODE_INDEX]);
                            var isStore = Convert.ToString(row[STORE_IDX]) == "1";
                            var isHR = Convert.ToString(row[HR_IDX]) == "1";
                            for (var j = BEGIN_INDEX; j <= END_INDEX; j++)
                            {
                                var value = row[j].ToString();
                                if (!string.IsNullOrEmpty(value))
                                {
                                    if (code.StartsWith("O"))
                                    {
                                        lastIsStore = isStore;
                                        oIndexes[j] = value;
                                        var idxs = oIndexes.Where(x => x.Key > j).Select(x => x.Key).ToList();
                                        foreach (var idx in idxs)
                                        {
                                            oIndexes.Remove(idx);
                                        }
                                        lastCode = "O";
                                    }
                                    else if (code.StartsWith("S"))
                                    {
                                        lastCode = "S";
                                        var department = new Department();
                                        var prefix = "";
                                        if (oIndexes.ContainsKey(j - 1))
                                        {
                                            prefix += oIndexes[j - 1];
                                        }
                                        department.Id = Guid.NewGuid();
                                        prIndexes[department.Id] = prefix;
                                        department.Name = $"{prefix} {value}".Trim();
                                        department.PositionCode = positionCode;
                                        department.IsStore = lastIsStore;
                                        department.IsHR = isHR;
                                        department.PositionName = value;
                                        if (jobGrades.ContainsKey(jobGrade))
                                        {
                                            department.JobGradeId = jobGrades[jobGrade].Id;
                                            if (jobGrades[jobGrade].Grade <= 4)
                                            {
                                                department.Type = Infrastructure.Enums.DepartmentType.Division;
                                            }
                                            else
                                            {
                                                department.Type = Infrastructure.Enums.DepartmentType.Department;
                                            }
                                        }
                                        else
                                        {
                                            if (posMappings.ContainsKey(department.PositionName))
                                            {
                                                jobGrade = posMappings[department.PositionName];
                                                department.JobGradeId = jobGrades[jobGrade].Id;
                                                if (jobGrades[jobGrade].Grade <= 4)
                                                {
                                                    department.Type = Infrastructure.Enums.DepartmentType.Division;
                                                }
                                                else
                                                {
                                                    department.Type = Infrastructure.Enums.DepartmentType.Department;
                                                }
                                            }
                                            else
                                            {

                                                Console.WriteLine("Cannot find job grade at line {0}, Name {1}", i + 1, department.PositionName);
                                                break;
                                            }
                                        }
                                        department.Created = DateTime.Now;
                                        department.Modified = DateTime.Now;
                                        department.SAPCode = code.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                                        department.Code = department.SAPCode;
                                        //Add root
                                        if (j == 2)
                                        {
                                            dIndexes[j] = department;
                                            lastDepartment = department;
                                        }
                                        else
                                        {
                                            if (positionCode.ToLower() == "asst" || positionCode.ToLower() == "assit")
                                            {
                                                department.ParentId = lastDepartment.Id;
                                            }
                                            else
                                            {
                                                //reset jobgrade index
                                                dIndexes[j] = department;
                                                var keys = dIndexes.Keys.OrderBy(x => x).ToList();
                                                var previousIdx = keys[keys.IndexOf(j) - 1];


                                                department.ParentId = dIndexes[previousIdx].Id;
                                                if (!oIndexes.ContainsKey(j - 1) && prIndexes.ContainsKey(department.ParentId.Value))
                                                {
                                                    department.Name = $"{prIndexes[department.ParentId.Value]} {value}".Trim();
                                                }
                                                //Remove all key after;
                                                var removedKeys = dIndexes.Keys.Where(x => x > j).ToList();
                                                foreach (var removeKey in removedKeys)
                                                {
                                                    dIndexes.Remove(removeKey);
                                                }
                                                lastDepartment = department;
                                            }
                                        }

                                        levels.Add(department);
                                        lastIndex = j;
                                    }
                                    else if (code.StartsWith("P"))
                                    {
                                        lastCode = "P";
                                        var sapCode = code.Substring(code.Length - 6, 6);
                                        if (users.ContainsKey(sapCode))
                                        {
                                            var csvUser = users[sapCode];
                                            var user = new User();
                                            user.Id = Guid.NewGuid();
                                            user.FullName = csvUser.FullName;
                                            //user.Email = csvUser.Email;
                                            user.LoginName = csvUser.LoginName;
                                            user.SAPCode = sapCode;
                                            user.Type = Infrastructure.Enums.LoginType.Membership;
                                            user.Created = DateTime.Now;
                                            user.Modified = DateTime.Now;
                                            listUser.Add(user);
                                            listUserMapping.Add(new UserDepartmentMapping()
                                            {
                                                IsHeadCount = true,
                                                Role = Infrastructure.Enums.Group.Member,
                                                DepartmentId = lastDepartment.Id,
                                                Id = Guid.NewGuid(),
                                                UserId = user.Id,
                                                Created = DateTime.Now,
                                                Modified = DateTime.Now
                                            });
                                        }
                                        else
                                        {
                                            var user = new User();
                                            user.Id = Guid.NewGuid();
                                            user.FullName = value;
                                            user.LoginName = sapCode;
                                            user.SAPCode = sapCode;
                                            user.Type = Infrastructure.Enums.LoginType.Membership;
                                            user.Created = DateTime.Now;
                                            user.Modified = DateTime.Now;
                                            listUser.Add(user);
                                            listUserMapping.Add(new UserDepartmentMapping()
                                            {
                                                IsHeadCount = true,
                                                Role = Infrastructure.Enums.Group.Member,
                                                DepartmentId = lastDepartment.Id,
                                                Id = Guid.NewGuid(),
                                                UserId = user.Id,
                                                Created = DateTime.Now,
                                                Modified = DateTime.Now
                                            });
                                        }
                                    }
                                    break;
                                }
                                else if (j == END_INDEX && code.StartsWith("P"))
                                {
                                    lastCode = "P";
                                    var sapCode = code.Substring(code.Length - 6, 6);
                                    if (users.ContainsKey(sapCode))
                                    {
                                        var csvUser = users[sapCode];
                                        var user = new User();
                                        user.Id = Guid.NewGuid();
                                        user.FullName = csvUser.FullName;
                                        //user.Email = csvUser.Email;
                                        user.LoginName = csvUser.LoginName;
                                        user.SAPCode = sapCode;
                                        user.Type = Infrastructure.Enums.LoginType.Membership;
                                        user.Created = DateTime.Now;
                                        user.Modified = DateTime.Now;
                                        listUser.Add(user);
                                        listUserMapping.Add(new UserDepartmentMapping()
                                        {
                                            IsHeadCount = true,
                                            Role = Infrastructure.Enums.Group.Member,
                                            DepartmentId = lastDepartment.Id,
                                            Id = Guid.NewGuid(),
                                            UserId = user.Id,
                                            Created = DateTime.Now,
                                            Modified = DateTime.Now
                                        });
                                    }
                                    else
                                    {
                                        var user = new User();
                                        user.Id = Guid.NewGuid();
                                        user.FullName = positionCode;
                                        user.LoginName = sapCode;
                                        user.SAPCode = sapCode;
                                        user.Type = Infrastructure.Enums.LoginType.Membership;
                                        user.Created = DateTime.Now;
                                        user.Modified = DateTime.Now;
                                        listUser.Add(user);
                                        listUserMapping.Add(new UserDepartmentMapping()
                                        {
                                            IsHeadCount = true,
                                            Role = Infrastructure.Enums.Group.Member,
                                            DepartmentId = lastDepartment.Id,
                                            Id = Guid.NewGuid(),
                                            UserId = user.Id,
                                            Created = DateTime.Now,
                                            Modified = DateTime.Now
                                        });
                                    }
                                }
                            }

                        }
                    }
                }


                ctx.Departments.AddRange(levels);

                ctx.Users.AddRange(listUser);
                ctx.UserDepartmentMappings.AddRange(listUserMapping);
                ctx.SaveChanges();
            }
        }

        public static void Execute(HRDbContext ctx, RequestToHire item)
        {
            var deptDivisionid = Guid.Empty;
            if (item.ReplacementFor == TypeOfNeed.NewPosition)
            {
                var jobGrade = ctx.JobGrades.FirstOrDefault(x => x.Caption == item.JobGradeCaption);
                var dept = ctx.Departments.FirstOrDefault(x => x.Id == item.DeptDivisionId);
                var deptName = dept.Name;
                var r = new Regex(@"\(+.+\)");
                var match = r.Match(deptName);
                if (match.Success)
                {
                    deptName = deptName.Replace(match.Value, string.Empty).Trim();
                }
                var countDept = ctx.Departments.Count();
                for (var i = 0; i < item.Quantity; i++)
                {
                    var newDept = new Department()
                    {
                        Name = $"{deptName} ({jobGrade.Caption})",
                        PositionCode = item.PositionCode,
                        PositionName = item.PositionName,
                        Code = $"DEP{String.Format("{0:D5}", (countDept + i + 1))}",
                        Type = jobGrade.Grade < 5 ? DepartmentType.Division : DepartmentType.Department,
                        Color = dept.Color,
                        JobGradeId = jobGrade.Id,
                        ParentId = item.DeptDivisionId,
                        IsStore = dept.IsStore,
                        IsHR = dept.IsHR,
                        RequestToHireId = item.Id,
                        Id = Guid.NewGuid(),
                        Created = DateTime.Now,
                        Modified = DateTime.Now

                    };
                    if (!string.IsNullOrEmpty(item.DepartmentName))
                    {
                        newDept.Name = item.DepartmentName;
                    }
                    ctx.Departments.Add(newDept);
                    deptDivisionid = newDept.Id;
                }
            }
            else
            {
                var dept = ctx.Departments.FirstOrDefault(x => x.Id == item.ReplacementForId);
                dept.RequestToHireId = item.Id;
                deptDivisionid = dept.Id;
            }
            //Create request to hire
            var pos = new Position()
            {
                AssignToId = item.AssignToId,
                DeptDivisionId = deptDivisionid,
                RequestToHireId = item.Id,
                RequestToHireNumber = item.ReferenceNumber,
                ExpiredDay = item.ExpiredDayPosition,
                ExpiredDate = DateTime.Now.AddDays(item.ExpiredDayPosition),
                HasBudget = item.HasBudget == CheckBudgetOption.Budget,
                Quantity = item.Quantity,
                LocationCode = item.LocationCode,
                LocationName = item.LocationName,
                Status = PositionStatus.Opened,
                PositionName = item.PositionName,
                Id = Guid.NewGuid(),
                Created = DateTime.Now,
                Modified = DateTime.Now,
                ReferenceNumber = GenerateReferenceNumber("Position")
            };
            ctx.Position.Add(pos);
            var assignedToDept = ctx.Departments.FirstOrDefault(x => x.UserDepartmentMappings.Any(u => u.UserId == item.AssignToId && u.IsHeadCount && u.Role == Group.Member));
            ctx.Permissions.Add(new Permission()
            {
                UserId = item.AssignToId,
                DepartmentId = assignedToDept?.Id,
                DepartmentType = Group.Member,
                ItemId = pos.Id,
                Perm = Right.Edit,
                Id = Guid.NewGuid(),
                Created = DateTime.Now,
                Modified = DateTime.Now
            });
            ctx.Permissions.Add(new Permission()
            {
                UserId = item.AssignToId,
                DepartmentId = assignedToDept?.Id,
                DepartmentType = Group.Member,
                ItemId = item.Id,
                Perm = Right.View,
                Id = Guid.NewGuid(),
                Created = DateTime.Now,
                Modified = DateTime.Now
            });
        }

        public static IEnumerable<string> FindFieldTokens(string str)
        {
            foreach (string str1 in Program.FindTokens(str, "\\{[\\d\\w\\s\\:]*\\}"))
            {
                yield return str1;
            }
        }

        public static IEnumerable<string> FindTokens(string str, string pattern)
        {
            MatchCollection matchCollections = (new Regex(pattern)).Matches(str);
            if (matchCollections.Count > 0)
            {
                foreach (Match match in matchCollections)
                {
                    yield return match.Value;
                }
            }
        }
        private static string GenerateReferenceNumber(string name)
        {
            var refNumber = string.Empty;
            if (_refs.ContainsKey(name))
            {
                var refEnitty = _refs[name];
                lock (_lock)
                {

                    if (refEnitty.IsNewYearReset && refEnitty.CurrentYear != DateTime.Now.Year || refEnitty.CurrentYear == 0)
                    {
                        refEnitty.CurrentNumber = 1;
                        refEnitty.CurrentYear = DateTime.Now.Year;
                    }
                    else
                    {
                        refEnitty.CurrentNumber++;
                    }
                    //Create new context to prevent trackchange
                    using (var ctx = new HRDbContext())
                    {
                        ctx.Entry(refEnitty).State = EntityState.Modified;
                        ctx.SaveChanges();
                    }
                }
                refNumber = refEnitty.Formula;
                var tokens = FindFieldTokens(refNumber);
                foreach (var token in tokens)
                {
                    switch (token.ToLower())
                    {
                        case "{year}":
                            refNumber = refNumber.Replace(token, refEnitty.CurrentYear.ToString());
                            break;
                        //For Autonumber field
                        default:
                            var tokenParts = token.Trim(new char[] { '{', '}' }).Split(new char[] { ':' });
                            if (tokenParts.Length > 1)
                            {
                                refNumber = refNumber.Replace(token, refEnitty.CurrentNumber.ToString($"D{tokenParts[1]}"));
                            }
                            break;

                    }
                }

            }
            return refNumber;
        }


        private static Dictionary<string, CSVUser> GetUsers()
        {

            using (var reader = new StreamReader("users.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<CSVUser>().ToDictionary(x => x.SAPCode, x => x);
            }
        }
        private static Dictionary<string, string> GetPositionMappings()
        {

            using (var reader = new StreamReader("missingmapping.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<CSVGrade>().ToDictionary(x => x.Position, x => x.Grade);
            }
        }
        private static void MigrateMissingTimeClockDetail()
        {
            try
            {
                using (var ctx = new HRDbContext())
                {
                    Console.WriteLine("Start");
                    if (ctx.MissingTimeClockDetails.ToList().Count() == 0)
                    {
                        List<MissingTimeClockDetail> missingTimeClockDetails = new List<MissingTimeClockDetail>();
                        var allMissingTimeClocks = ctx.MissingTimeClocks;
                        foreach (var missing in allMissingTimeClocks)
                        {
                            Console.WriteLine(string.Format("{0}: {1}", missing.ReferenceNumber, missing.Status));
                            List<MissingTimeClockDetail> details = JsonConvert.DeserializeObject<List<MissingTimeClockDetail>>(missing.ListReason);
                            if (details.Any())
                            {
                                foreach (var detailItem in details)
                                {
                                    detailItem.Id = Guid.NewGuid();
                                    detailItem.Created = missing.Created;
                                    detailItem.Modified = missing.Modified;
                                    detailItem.MissingTimeClockId = missing.Id;
                                }
                                missingTimeClockDetails.AddRange(details);
                            }
                        }
                        if (missingTimeClockDetails.Any())
                        {
                            Console.WriteLine(string.Format("Count MissingTimeClocks: {0}", missingTimeClockDetails.Count));
                            ctx.MissingTimeClockDetails.AddRange(missingTimeClockDetails);
                            ctx.SaveChanges();
                        }
                        //Console.WriteLine("End");
                    }
                    else
                    {
                        Console.WriteLine("No data");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private static void MigrateTrackingLogInitData()
        {
            try
            {
                using (var ctx = new HRDbContext())
                {
                    Console.WriteLine("Start");
                    if (ctx.TrackingLogInitDatas.Count() == 0)
                    {
                        var trackingRequests = ctx.TrackingRequests;
                        List<TrackingLogInitData> LogInitDatas = new List<TrackingLogInitData>();
                        if (trackingRequests.Any())
                        {
                            foreach (var tracking in trackingRequests)
                            {
                                dynamic payload = JsonConvert.DeserializeObject<dynamic>(tracking.Payload);
                                if (payload != null)
                                {
                                    TrackingLogInitData LogInitData = new TrackingLogInitData();
                                    string fromDate = string.Empty;
                                    string toDate = string.Empty;
                                    LogInitData.Id = Guid.NewGuid();
                                    LogInitData.TrackingLogId = tracking.Id;
                                    LogInitData.SAPCode = payload["Pernr"];
                                    LogInitData.CreatedByFullName = tracking.UserName;
                                    LogInitData.Created = tracking.Created;
                                    LogInitData.Modified = tracking.Modified;
                                    LogInitData.ReferenceNumber = tracking.ReferenceNumber;
                                    try
                                    {
                                        if (tracking.Url.Contains("ShiftExchange"))
                                        {
                                            fromDate = payload["Date"].ToString();
                                            toDate = payload["Date"].ToString();
                                            LogInitData.Code = payload["Tprog"];
                                            LogInitData.FunctionType = "ShiftExchange";

                                        }
                                        else if (tracking.Url.Contains("OverTime"))
                                        {
                                            fromDate = payload["Date"].ToString();
                                            toDate = payload["Date"].ToString();
                                            LogInitData.FunctionType = "OverTime";
                                        }
                                        else if (tracking.Url.Contains("Resignation"))
                                        {
                                            fromDate = payload["Begda"].ToString();
                                            toDate = payload["Begda"].ToString();
                                            LogInitData.FunctionType = "Resignation";
                                        }
                                        else if (tracking.Url.Contains("MissingTimeclock"))
                                        {
                                            fromDate = payload["Date"].ToString();
                                            toDate = payload["Date"].ToString();
                                            LogInitData.FunctionType = "OverTime";
                                        }
                                        else if (tracking.Url.Contains("LeaveBalance"))
                                        {
                                            fromDate = payload["Begda"].ToString();
                                            toDate = payload["Endda"].ToString();
                                            LogInitData.Code = payload["Awart"];
                                            LogInitData.FunctionType = "LeaveBalance";
                                        }
                                        else if (tracking.Url.Contains("Add_EmployeeSet"))
                                        {
                                            fromDate = payload["Begda"].ToString();
                                            toDate = payload["Begda"].ToString();
                                            LogInitData.FunctionType = "Employee";
                                        }
                                        else if (tracking.Url.Contains("TargetPlan"))
                                        {
                                            var period = payload["Period"].ToString();
                                            var year = payload["Zyear"].ToString();
                                            if (period != "12")
                                            {
                                                var nextPeriod = int.Parse(period) + 1;
                                                if (nextPeriod > 10)
                                                {
                                                    fromDate = string.Format("{0}{1}{2}", year, period, "26");
                                                    toDate = string.Format("{0}{1}{2}", year, nextPeriod, "25");
                                                }
                                                else
                                                {
                                                    fromDate = string.Format("{0}0{1}{2}", year, period, "26");
                                                    if (nextPeriod == 10)
                                                    {
                                                        toDate = string.Format("{0}{1}{2}", year, nextPeriod, "25");
                                                    }
                                                    else
                                                    {
                                                        toDate = string.Format("{0}0{1}{2}", year, nextPeriod, "25");
                                                    }

                                                }

                                            }
                                            else
                                            {
                                                var nextYear = int.Parse(year) + 1;
                                                fromDate = string.Format("{0}{1}{2}", year, "12", "26");
                                                toDate = string.Format("{0}{1}{2}", nextYear, "01", "25");
                                            }
                                            LogInitData.FunctionType = "TargetPlan";

                                        }

                                        if (!String.IsNullOrEmpty(fromDate))
                                        {
                                            LogInitData.FromDate = DateTime.ParseExact(fromDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                                        }
                                        if (!String.IsNullOrEmpty(toDate))
                                        {
                                            LogInitData.ToDate = DateTime.ParseExact(toDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                                        }
                                        LogInitDatas.Add(LogInitData);

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                            if (LogInitDatas.Any())
                            {
                                Console.WriteLine(string.Format("Count TrackingLogInitDatas: {0}", LogInitDatas.Count));
                                ctx.TrackingLogInitDatas.AddRange(LogInitDatas);
                                ctx.SaveChanges();
                            }
                            Console.WriteLine("End");
                        }
                    }


                    else
                    {
                        Console.WriteLine("No data");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private static void FindSubmitPersonFromDepartmentId(List<Guid> Ids)
        {
            try
            {
                foreach (var Id in Ids)
                {
                    using (var ctx = new HRDbContext())
                    {
                        Department hasSubmitPersonDepartment = null;
                        var dept = ctx.Departments.Where(x => x.Id == Id).FirstOrDefault();
                        Console.WriteLine(string.Format("Current Department: {0}-{1}", dept.Code, dept.Name));
                        if (dept != null)
                        {
                            hasSubmitPersonDepartment = dept.UserSubmitPersonDeparmentMappings.Any() ? dept : null;
                            if (hasSubmitPersonDepartment == null)
                            {
                                var isPause = false;
                                while (!isPause)
                                {
                                    var parentDeptId = dept.ParentId;
                                    if (parentDeptId.HasValue)
                                    {
                                        var parentDept = ctx.Departments.FirstOrDefault(x => x.Id == parentDeptId);
                                        hasSubmitPersonDepartment = parentDept.UserSubmitPersonDeparmentMappings.Any() ? parentDept : null;
                                        if (hasSubmitPersonDepartment == null)
                                        {
                                            dept = parentDept;
                                            Console.WriteLine(string.Format("Parent Department: {0}-{1}", dept.Code, dept.Name));
                                        }
                                        else
                                        {
                                            isPause = true;
                                            Console.WriteLine(string.Format("Submit Department: {0}-{1}", hasSubmitPersonDepartment.Code, hasSubmitPersonDepartment.Name));
                                        }
                                    }
                                    else
                                    {
                                        isPause = true;

                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine(string.Format("Submit Department: {0}-{1}", hasSubmitPersonDepartment.Code, hasSubmitPersonDepartment.Name));
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errors: {ex.Message}");
            }
        }
    }

    public class CSVUser
    {
        public string LoginName { get; set; }
        public string FullName { get; set; }
        public string SAPCode { get; set; }
        public string Email { get; set; }
    }

    public class CSVGrade
    {
        public string Position { get; set; }
        public string Grade { get; set; }
    }
    public class RedundantPRD
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double PRDRemain { get; set; }
    }
    public class RedundantPRDData
    {
        public RedundantPRDData()
        {
            JsonData = new List<RedundantPRD>();
        }
        public List<RedundantPRD> JsonData { get; set; }
    }

}
