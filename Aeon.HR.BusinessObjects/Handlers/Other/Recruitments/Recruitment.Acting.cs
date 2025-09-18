using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.ViewModels.Args;
using AutoMapper;
using System.Globalization;
using System.Security;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.ViewModels.PrintFormViewModel;
using System.IO;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class RecruitmentBO
    {
        public async Task<ResultDTO> CreateActing(MasterActingArgs arg)
        {

            var result = new ResultDTO();
            var acting = await _uow.GetRepository<Acting>().GetSingleAsync(x => x.Id == arg.Acting.Id);
            if (acting != null)
            {
                acting.CurrentJobGrade = arg.Acting.CurrentJobGrade;
                acting.CurrentPosition = arg.Acting.CurrentPosition;
                acting.DeptCode = arg.Acting.DeptCode;
                acting.DepartmentId = arg.Acting.DepartmentId;
                acting.DepartmentSAPId = arg.Acting.DepartmentSAPId;
                acting.DeptName = arg.Acting.DeptName;
                acting.DivisionCode = arg.Acting.DivisionCode;
                acting.DivisionName = arg.Acting.DivisionName;
                acting.FirstAppraiserId = arg.Acting.FirstAppraiserId;
                acting.FullName = arg.Acting.FullName;
                acting.IsCompletedTranning = arg.Acting.IsCompletedTranning;
                //CR222 ================================================
                acting.PersonnelArea = arg.Acting.PersonnelArea;
                acting.PersonnelAreaText = arg.Acting.PersonnelAreaText;
                acting.EmployeeGroup = arg.Acting.EmployeeGroup;
                acting.EmployeeGroupDescription = arg.Acting.EmployeeGroupDescription;
                acting.EmployeeSubgroup = arg.Acting.EmployeeSubgroup;
                acting.EmployeeSubgroupDescription = arg.Acting.EmployeeSubgroupDescription;
                acting.PayScaleArea = arg.Acting.PayScaleArea;
                acting.FirstAppraiserNote = arg.Acting.FirstAppraiserNote;
                acting.SecondAppraiserNote = arg.Acting.SecondAppraiserNote;
                //======================================================
                acting.Period1From = arg.Acting.Period1From;
                acting.Period1To = arg.Acting.Period1To;
                acting.Period2From = arg.Acting.Period2From;
                acting.Period2To = arg.Acting.Period2To;
                acting.Period3From = arg.Acting.Period3From;
                acting.Period3To = arg.Acting.Period3To;
                acting.Period4From = arg.Acting.Period4From;
                acting.Period4To = arg.Acting.Period4To;
                acting.PositionId = arg.Acting.PositionId;
                acting.PositionName = arg.Acting.PositionName;
                acting.ReferenceNumber = arg.Acting.ReferenceNumber;
                acting.SecondAppraiserId = arg.Acting.SecondAppraiserId;
                acting.StartingDate = arg.Acting.StartingDate;
                acting.TableCompulsoryTraining = arg.Acting.TableCompulsoryTraining;
                acting.TemplateGoal = arg.Acting.TemplateGoal;
                acting.TitleInActingPeriodName = arg.Acting.TitleInActingPeriodName;
                acting.UserId = arg.Acting.UserId;
                //if(arg.Acting.UserSAPCode != "********")
                //{
                //    acting.UserSAPCode = arg.Acting.UserSAPCode;
                //}
                acting.UserSAPCode = arg.Acting.UserSAPCode;

                acting.WorkLocationCode = arg.Acting.WorkLocationCode;
                acting.WorkLocationName = arg.Acting.WorkLocationName;

                acting.NewPersonnelArea = arg.Acting.NewPersonnelArea;
                acting.NewPersonnelAreaText = arg.Acting.NewPersonnelAreaText;
                acting.NewWorkLocationCode = arg.Acting.NewWorkLocationCode;
                acting.NewWorkLocationName = arg.Acting.NewWorkLocationName;

                acting.JobGradeName = arg.Acting.JobGradeName;
                acting.JobGradeValue = arg.Acting.JobGradeValue;
                acting.UserDepartmentId = arg.Acting.UserDepartmentId;
                #region firstAppraiserDepartmentId
                if (arg.Acting.FirstAppraiserId.HasValue && !acting.FirstAppraiserDepartmentId.HasValue)
                {
                    var firstAppraiser_User = arg.Acting.FirstAppraiserId.Value.GetUserById(_uow, true);
                    acting.FirstAppraiserDepartmentId = firstAppraiser_User != null ? firstAppraiser_User.GetHeadCountDepartmentID() : null;
                }
                #endregion

                #region secondAppraiserDepartmentId
                if (arg.Acting.SecondAppraiserId.HasValue && !acting.SecondAppraiserDepartmentId.HasValue)
                {
                    var secondAppraiser_User = arg.Acting.SecondAppraiserId.Value.GetUserById(_uow, true);
                    acting.SecondAppraiserDepartmentId = secondAppraiser_User != null ? secondAppraiser_User.GetHeadCountDepartmentID() : null;
                }
                #endregion

                _uow.GetRepository<Acting>().Update(acting);
                //delete period
                var existPeriods = await _uow.GetRepository<Period>().FindByAsync(x => x.ActingId == acting.Id);

                if (existPeriods.Any())
                {
                    _uow.GetRepository<Period>().Delete(existPeriods);
                }

            }
            else
            {
                var FirstAppraiserDepartmentId = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.UserId == arg.Acting.FirstAppraiserId && x.IsHeadCount == true);
                var SecondAppraiserDepartmentId = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.UserId == arg.Acting.SecondAppraiserId && x.IsHeadCount == true);

                acting = Mapper.Map<Acting>(arg.Acting);
                //CR - 420

                if (FirstAppraiserDepartmentId != null)
                {
                    acting.FirstAppraiserDepartmentId = FirstAppraiserDepartmentId.DepartmentId;
                }

                if (SecondAppraiserDepartmentId != null)
                {
                    acting.SecondAppraiserDepartmentId = SecondAppraiserDepartmentId.DepartmentId;
                }

                _uow.GetRepository<Acting>().Add(acting);
            }
            var periods = new List<Period>();
            foreach (var item in arg.Periods)
            {
                if (item.FromDate != null && item.FromDate != DateTimeOffset.MinValue)
                {
                    var period = new Period
                    {
                        FromDate = item.FromDate,
                        ToDate = item.ToDate,
                        ActingId = acting.Id,
                        Appraising = item.Appraising,
                        Priority = item.Priority
                    };
                    periods.Add(period);
                }
            }
            if (periods.Any())
            {
                _uow.GetRepository<Period>().Add(periods);
            }
            await _uow.CommitAsync();
            result.Object = Mapper.Map<ActingViewModel>(acting);
            return result;
        }
        public async Task<ResultDTO> GetActings(QueryArgs args)
        {
            var actings = await _uow.GetRepository<Acting>().FindByAsync<ActingRequestViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
			foreach (var item in actings)
            {
                item.CreatedByFullNameView = item.CreatedByFullName;
                if (item.Mapping != null)
                {
                    var getDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == item.Mapping.DepartmentId);
                    if (getDepartment != null && getDepartment.RegionId != null)
                    {
                        item.RegionName = getDepartment.Region.RegionName;
                    }
                }
			}
            var count = await _uow.GetRepository<Acting>().CountAsync(args.Predicate, args.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = actings, Count = count } };
        }
        public async Task<ResultDTO> GetActingByReferenceNumber(ActingRequestArgs args)
        {
            var acting = await _uow.GetRepository<Acting>().GetSingleAsync<ActingViewModel>(x => x.Id == args.Id);
            var period = await _uow.GetRepository<Period>().FindByAsync<PeriodViewModel>(x => x.ActingId == acting.Id);
            var data = new ActingResponeViewModel
            {
                Acting = acting,
                Period = period
            };
            return new ResultDTO { Object = new ArrayResultDTO { Data = data } };
        }
        public async Task<byte[]> PrintFormActing(Guid Id)
        {
            byte[] result = null;
            var record = await _uow.GetRepository<Acting>().FindByIdAsync(Id, x => x.Periods);
            if (record != null)
            {
                var dataToPrint = Mapper.Map<ActingPrintFormViewModel>(record);
                var periodFrom = record.Period1From.Value.LocalDateTime.ToString("dd/MM/yyyy");
                var periodTo = record.Period1To.Value.LocalDateTime.ToString("dd/MM/yyyy");
                if (record.Period4To.HasValue)
                {
                    periodTo = record.Period4To.Value.LocalDateTime.ToString("dd/MM/yyyy");
                }
                else if (record.Period3To.HasValue)
                {
                    periodTo = record.Period3To.Value.LocalDateTime.ToString("dd/MM/yyyy");
                }
                else if (record.Period2To.HasValue)
                {
                    periodTo = record.Period2To.Value.LocalDateTime.ToString("dd/MM/yyyy");
                }
                dataToPrint.ActingPeriodTime = string.Format("Acting period: From {0} To {1}", periodFrom, periodTo);
                var properties = typeof(ActingPrintFormViewModel).GetProperties();
                var pros = new Dictionary<string, string>();
                var targets = new List<TargetListViewModel>();
                var tableCompulsoryTrainnings = new List<TableCompulsoryViewModel>();
                if (record.Periods.Count > 0)
                {
                    var periods = record.Periods.OrderBy(x => x.FromDate);
                    var keyValuePairs = new Dictionary<string, List<PeriodAppraisingViewModel>>();
                    foreach (var period in periods)
                    {
                        keyValuePairs.Add(period.FromDate.ToString("dd/MM/yyyy"), JsonConvert.DeserializeObject<List<PeriodAppraisingViewModel>>(period.Appraising));
                    }

                    List<ActingGoalViewModel> goals = JsonConvert.DeserializeObject<List<ActingGoalViewModel>>(JsonConvert.DeserializeObject<string>(record.TemplateGoal)); ;
                    for (var i = 0; i < goals.Count; i++)
                    {
                        var goal = goals.ElementAt(i);
                        var target = new TargetListViewModel()
                        {
                            GoalTitle = goal.Goal,
                            Weight = goal.Weight.ToString(),
                        };
                        if (record.Period1From.HasValue && keyValuePairs.ContainsKey(record.Period1From.Value.ToString("dd/MM/yyyy")))
                        {
                            var data = keyValuePairs[record.Period1From.Value.ToString("dd/MM/yyyy")].ElementAt(i);
                            target.Target1 = data.Target;
                            target.Actual1 = data.Actual;                            
                        }
                        if (record.Period2From.HasValue && keyValuePairs.ContainsKey(record.Period1From.Value.ToString("dd/MM/yyyy")))
                        {
                            var data = keyValuePairs[record.Period2From.Value.ToString("dd/MM/yyyy")].ElementAt(i);
                            target.Target2 = data.Target;
                            target.Actual2 = data.Actual;
                        }
                        if (record.Period3From.HasValue && keyValuePairs.ContainsKey(record.Period1From.Value.ToString("dd/MM/yyyy")))
                        {
                            var data = keyValuePairs[record.Period3From.Value.ToString("dd/MM/yyyy")].ElementAt(i);
                            target.Target3 = data.Target;
                            target.Actual3 = data.Actual;
                        }
                        if (record.Period4From.HasValue && keyValuePairs.ContainsKey(record.Period1From.Value.ToString("dd/MM/yyyy")))
                        {
                            var data = keyValuePairs[record.Period4From.Value.ToString("dd/MM/yyyy")].ElementAt(i);
                            target.Target4 = data.Target;
                            target.Actual4 = data.Actual;
                        }
                        targets.Add(target);
                    }
                }
                var tbPros = await GetWorkFlowHistories(Id, ObjectToPrintFromType.Acting, dataToPrint);
                foreach (var property in properties)
                {
                    var value = Convert.ToString(property.GetValue(dataToPrint));
                    pros[property.Name] = SecurityElement.Escape(value);
                }
                if (!string.IsNullOrEmpty(record.TableCompulsoryTraining))
                {
                    tableCompulsoryTrainnings = JsonConvert.DeserializeObject<List<TableCompulsoryViewModel>>(JsonConvert.DeserializeObject<string>(record.TableCompulsoryTraining));
                }

                result = ExportXLS("Acting.xlsx", pros, targets, tableCompulsoryTrainnings, tbPros);
            }
            return result;
        }
        public byte[] ExportXLS(string template, Dictionary<string, string> pros, List<TargetListViewModel> tblPros, List<TableCompulsoryViewModel> tblTrainnings, List<Dictionary<string, string>> tbPros)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory; // You get main rott
            var filePath = Path.Combine(path, "PrintDocument", template);
            var memoryStream = new MemoryStream();
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                using (var pck = new ExcelPackage(stream))
                {
                    ExcelWorkbook WorkBook = pck.Workbook;
                    ExcelWorksheet worksheet = WorkBook.Worksheets.First();
                    //range.Merge = true;  
                    //CR210======
                    InsertCheckBox(worksheet, 21, pros);
                    //===========
                    InsertTargetData(worksheet, 8, tblPros);
                    InsertTargetData(worksheet, tblTrainnings);
                    InsertWorkflowHistoriesData(worksheet, 34 + (tblPros.Count -1), tbPros);
                    var regex = new Regex(@"\[\[[\d\w\s]*\]\]", RegexOptions.IgnoreCase);
                    var tokens = worksheet.Cells.Where(x => x.Value != null && regex.Match(x.Value.ToString()).Success);
                    foreach (var token in tokens)
                    {
                        var fieldToken = token.Value.ToString().Trim(new char[] { '[', ']' });
                        if (pros.ContainsKey(fieldToken))
                        {
                            token.Value = pros[fieldToken];
                        }
                        //CR210==============
                        else if (token.Value.ToString().Contains("[[FirstAppraiserComment]]"))
                        {
                            token.Value = token.Value.ToString().Replace("[[FirstAppraiserComment]]", pros["FirstAppraiserComment"]);
                        }
                        else if (token.Value.ToString().Contains("[[SecondAppraiserComment]]"))
                        {
                            token.Value = token.Value.ToString().Replace("[[SecondAppraiserComment]]", pros["SecondAppraiserComment"]);
                        }
                        else if (token.Value.ToString().Contains(": [[FirstAppraiserConfirmation]]"))
                        {
                            token.Value = token.Value.ToString().Replace("[[FirstAppraiserConfirmation]]", pros["FirstAppraiserConfirmation"]);
                        }
                        else if (token.Value.ToString().Contains(": [[SecondAppraiserConfirmation]]"))
                        {
                            token.Value = token.Value.ToString().Replace("[[SecondAppraiserConfirmation]]", pros["SecondAppraiserConfirmation"]);
                        }
                        //===================
                    }

                    pck.SaveAs(memoryStream);

                }
            }
            return memoryStream.ToArray();
        }
        private void InsertCheckBox(ExcelWorksheet worksheet, int styleRow, Dictionary<string, string> pros)
        {
            //CR210==================================
            if (pros.ContainsKey("FirstAppraiserNote"))
            {
                var row = worksheet.Cells[$"B{styleRow}"];
                if (row != null && row.Value != null && !string.IsNullOrEmpty(row.Value.ToString()))
                {
                    if (pros["FirstAppraiserNote"] == "Passed")
                    {
                        row.Value = row.Value.ToString().Replace("[[IsPassed]]", "☒");
                        row.Value = row.Value.ToString().Replace("[[IsFailed]]", "☐");
                    }
                    else if (pros["FirstAppraiserNote"] == "Failed")
                    {
                        row.Value = row.Value.ToString().Replace("[[IsPassed]]", "☐");
                        row.Value = row.Value.ToString().Replace("[[IsFailed]]", "☒");
                    }
                    else
                    {
                        row.Value = row.Value.ToString().Replace("[[IsPassed]]", "☐");
                        row.Value = row.Value.ToString().Replace("[[IsFailed]]", "☐");
                    }
                }
            }
            if (pros.ContainsKey("SecondAppraiserNote"))
            {
                var row = worksheet.Cells[$"K{styleRow}"];
                if (row != null && row.Value != null && !string.IsNullOrEmpty(row.Value.ToString()))
                {
                    if (pros["SecondAppraiserNote"] == "Passed")
                    {
                        row.Value = row.Value.ToString().Replace("[[IsPassed]]", "☒");
                        row.Value = row.Value.ToString().Replace("[[IsFailed]]", "☐");
                    }
                    else if (pros["SecondAppraiserNote"] == "Failed")
                    {
                        row.Value = row.Value.ToString().Replace("[[IsPassed]]", "☐");
                        row.Value = row.Value.ToString().Replace("[[IsFailed]]", "☒");
                    }
                    else
                    {
                        row.Value = row.Value.ToString().Replace("[[IsPassed]]", "☐");
                        row.Value = row.Value.ToString().Replace("[[IsFailed]]", "☐");
                    }
                }
            }
        }
        private void InsertTargetData(ExcelWorksheet worksheet, int styleRow, List<TargetListViewModel> tblPros)
        {
            var index = 0;
            var fromRow = styleRow + 1;
            var toRow = fromRow + tblPros.Count;
            for (int i = fromRow; i < toRow; i++)
            {
                var target = tblPros.ElementAt(index);
                worksheet.InsertRow(i, 1, styleRow);
                var row = worksheet.Row(i);
                row.Height = 26;
                ExcelRange mergeRange1 = worksheet.Cells[$"C{i}:F{i}"];
                ExcelRange mergeRange2 = worksheet.Cells[$"J{i}:K{i}"];
                mergeRange1.Merge = true;
                mergeRange2.Merge = true;
                // Style               
                worksheet.Cells[$"C{i}"].Style.WrapText = true;
                worksheet.Cells[$"C{i}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                worksheet.Cells[$"C{i}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                worksheet.Cells[$"C{i}"].AutoFitColumns();
                // Update Data
                worksheet.Cells[$"B{i}"].Value = ++index;
                worksheet.Cells[$"C{i}"].Value = target.GoalTitle;
                worksheet.Cells[$"G{i}"].Value = target.Weight;
                worksheet.Cells[$"H{i}"].Value = target.Target1;
                worksheet.Cells[$"I{i}"].Value = target.Actual1;
                worksheet.Cells[$"J{i}"].Value = target.Target2;
                worksheet.Cells[$"L{i}"].Value = target.Actual2;
                worksheet.Cells[$"M{i}"].Value = target.Target3;
                worksheet.Cells[$"N{i}"].Value = target.Actual3;
                worksheet.Cells[$"O{i}"].Value = target.Target4;
                worksheet.Cells[$"P{i}"].Value = target.Actual4;
            }
            worksheet.DeleteRow(styleRow);
        }
        private void InsertTargetData(ExcelWorksheet worksheet, List<TableCompulsoryViewModel> array)
        {
            var compulsoryTrainningCell = worksheet.Cells.Where(x => x.Value != null && x.Value.ToString().Trim().Equals("Course name")).FirstOrDefault();
            if (compulsoryTrainningCell != null)
            {
                var from = compulsoryTrainningCell.Start.Row + 1;
                var to = from + array.Count;
                var index = 0;
                for (int i = from; i < to; i++)
                {
                    var record = array.ElementAt(index);
                    //Style
                    worksheet.Cells[$"C{i}"].Style.WrapText = true;
                    worksheet.Cells[$"C{i}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    worksheet.Cells[$"C{i}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{i}"].AutoFitColumns();
                    // Mapping Data
                    worksheet.Cells[$"C{i}"].Value = record.CourseName;
                    worksheet.Cells[$"H{i}"].Value = record.Duration;
                    worksheet.Cells[$"I{i}"].Value = record.ActualResult;
                    index++;
                }
            }

        }
        private void InsertWorkflowHistoriesData(ExcelWorksheet worksheet, int styleRow, List<Dictionary<string, string>> tblPros)
        {
            var index = 0;
            var fromRow = styleRow + 1;
            var toRow = fromRow + tblPros.Count;
            for (int i = fromRow; i < toRow; i++)
            {
                var target = tblPros.ElementAt(index);
                worksheet.InsertRow(i, 1, styleRow);
                var row = worksheet.Row(i);
                row.Height = 26;
                ExcelRange mergeRange1 = worksheet.Cells[$"B{i}:E{i}"];
                ExcelRange mergeRange2 = worksheet.Cells[$"F{i}:G{i}"];
                mergeRange1.Merge = true;
                mergeRange2.Merge = true;
                // Style               
                worksheet.Cells[$"C{i}"].Style.WrapText = true;
                worksheet.Cells[$"C{i}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                worksheet.Cells[$"C{i}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                worksheet.Cells[$"C{i}"].AutoFitColumns();
                // Update Data
                ++index;
                worksheet.Cells[$"B{i}"].Value = target["AssignedBy"];
                worksheet.Cells[$"F{i}"].Value = target["AssignedDate"];
                worksheet.Cells[$"H{i}"].Value = target["Outcome"];
                worksheet.Cells[$"I{i}"].Value = target["Comment"];
            }
            worksheet.DeleteRow(styleRow);
        }
    }
}