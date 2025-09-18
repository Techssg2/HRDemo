using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.PrintFormViewModel;
using System.Security;
using Aeon.HR.BusinessObjects.Helpers;
using System.Globalization;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.BusinessObjects.DataHandlers;
using System.Text.RegularExpressions;

namespace Aeon.HR.BusinessObjects.Handlers
{
	public partial class RecruitmentBO : IRecruitmentBO
	{
		public async Task<ResultDTO> CreatePromoteAndTransfer(PromoteAndTransferDataForCreatingArgs data)
		{
			var existPromoteAndTransfer = await _uow.GetRepository<PromoteAndTransfer>().FindByAsync(x => x.Id == data.Id);
			if (existPromoteAndTransfer.Any())
			{
				return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "PromoteAndTransfer is exist" } };
			}
			else
			{
				var promoteAndTransfer = Mapper.Map<PromoteAndTransfer>(data);
				promoteAndTransfer.IsSameDepartment = await isSameDepartment(promoteAndTransfer);
				_uow.GetRepository<PromoteAndTransfer>().Add(promoteAndTransfer);
				await _uow.CommitAsync();
				return new ResultDTO { Object = Mapper.Map<PromoteAndTransferViewModel>(promoteAndTransfer) };
			}
		}

		private async Task<bool> isSameDepartment(PromoteAndTransfer data)
		{
			try
			{
				var currentUserDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.UserDepartmentMappings.Any(t => t.UserId == data.UserId && t.IsHeadCount));
				data.CurrentDepartmentId = currentUserDept.Id;


				var newDeptId = await GetDepartmentId(data.NewDeptOrLineId);
				var currentDeptId = await GetDepartmentId(currentUserDept.Id);
				//Update current dept
				if (currentDeptId != Guid.Empty)
				{
                    //var dept = DbHelper.LoadDept(_uow, currentDeptId);
                    //var dept = DbHelper.LoadDept(_uow, currentDeptId);
                    var dept = _uow.GetRepository<Department>().FindById(currentDeptId);
                    data.DeptCode = dept.Code;
					data.DeptName = dept.Name;
				}
				//Update target dept
				if (newDeptId != Guid.Empty)
				{
                    //var targetDept = DbHelper.LoadDept(_uow, newDeptId);
                    var targetDept = _uow.GetRepository<Department>().FindById(newDeptId);
                    data.TargetDeptCode = targetDept.Code;
					data.TargetDeptName = targetDept.Name;
				}
				return newDeptId == currentDeptId;
			}
			catch { }
			return false;
		}

		private async Task<Guid> GetDepartmentId(Guid? nodeId)
		{
			var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == nodeId);
			var allGrades = await _uow.GetRepository<JobGrade>().GetAllAsync();
			var jd4Value = allGrades?
				.FirstOrDefault(x => (x.Title ?? string.Empty).ToUpper() == "G4")
				?.Grade ?? 4;
			if (department != null)
			{
				if (department.Type == DepartmentType.Department)
				{
					return department.Id;
				}
				var isStore = department.IsStore;
				while (department != null && department.ParentId.HasValue)
				{
					department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == department.ParentId, "", x => x.JobGrade);
					//if department is store and equal g4 is counted as different deparment 
					if (department.Type == Infrastructure.Enums.DepartmentType.Department || isStore && department.JobGrade.Grade == jd4Value)
					{
						return department.Id;
					}
				}
			}
			return Guid.Empty;
		}

		public async Task<ArrayResultDTO> GetListPromoteAndTransfers(QueryArgs args)
		{
			var promoteAndTransfers = await _uow.GetRepository<PromoteAndTransfer>().FindByAsync<PromoteAndTransferViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
			foreach (var item in promoteAndTransfers)
			{
				item.CreatedByFullNameView = item.CreatedByFullName;
				var getCurrentDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == item.CurrentDepartmentId);
				if (getCurrentDepartment != null && getCurrentDepartment.RegionId != null)
				{
					item.RegionName = getCurrentDepartment.Region.RegionName;
				}
			}
			var count = await _uow.GetRepository<PromoteAndTransfer>().CountAsync(args.Predicate, args.PredicateParameters);
			var result = new ArrayResultDTO { Data = promoteAndTransfers, Count = count };
			return result;
		}

		public async Task<ResultDTO> GetPromoteAndTransferById(PromoteAndTransferDataForCreatingArgs arg)
		{
			var promoteAndTransfer = await _uow.GetRepository<PromoteAndTransfer>().FindByIdAsync<PromoteAndTransferViewModel>(arg.Id);
			if (promoteAndTransfer != null)
			{
				return new ResultDTO { Object = promoteAndTransfer };
			}
			return new ResultDTO { };
		}

		public async Task<ResultDTO> SearchListPromoteAndTransfers(PromoteAndTransferDataForCreatingArgs data)
		{
			// search viet tạm
			var promoteAndTransfers = await _uow.GetRepository<PromoteAndTransfer>().FindByAsync<PromoteAndTransferViewModel>(x => x.ReferenceNumber == data.ReferenceNumber);
			if (promoteAndTransfers.Any())
			{
				return new ResultDTO { Object = promoteAndTransfers };
			}
			return new ResultDTO { };
		}

		public async Task<ResultDTO> DeletePromoteAndTransfer(PromoteAndTransferDataForCreatingArgs data)
		{
			var promoteAndTransfer = await _uow.GetRepository<PromoteAndTransfer>().GetSingleAsync(x => x.Id == data.Id);
			if (promoteAndTransfer == null)
			{
				return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "PromoteAndTransfer is not exist" } };
			}
			else
			{
				_uow.GetRepository<PromoteAndTransfer>().Delete(promoteAndTransfer);
				await _uow.CommitAsync();
			}
			return new ResultDTO { };
		}

		public async Task<ResultDTO> UpdatePromoteAndTransfer(PromoteAndTransferDataForCreatingArgs data)
		{
			var existPromoteAndTransfer = await _uow.GetRepository<PromoteAndTransfer>().GetSingleAsync(x => x.Id == data.Id);
			if (existPromoteAndTransfer == null)
			{
				return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "PromoteAndTransfer is not exist" } };
			}
			else
			{
				existPromoteAndTransfer.TypeCode = data.TypeCode;
				existPromoteAndTransfer.FullName = data.FullName;
				existPromoteAndTransfer.TypeName = data.TypeName;
				existPromoteAndTransfer.RequestFrom = data.RequestFrom;
				existPromoteAndTransfer.UserId = data.UserId;
				existPromoteAndTransfer.ReasonOfPromotion = data.ReasonOfPromotion;
				existPromoteAndTransfer.EffectiveDate = data.EffectiveDate;
				existPromoteAndTransfer.NewSalaryOrBenefit = data.NewSalaryOrBenefit;
				existPromoteAndTransfer.NewTitleCode = data.NewTitleCode;
				existPromoteAndTransfer.NewTitleName = data.NewTitleName;
				existPromoteAndTransfer.NewJobGradeId = data.NewJobGradeId;
				existPromoteAndTransfer.NewJobGradeName = data.NewJobGradeName;
				existPromoteAndTransfer.NewDeptOrLineId = data.NewDeptOrLineId;
				existPromoteAndTransfer.NewDeptOrLineCode = data.NewDeptOrLineCode;
				existPromoteAndTransfer.NewDeptOrLineName = data.NewDeptOrLineName;
				existPromoteAndTransfer.NewWorkLocationCode = data.NewWorkLocationCode;
				existPromoteAndTransfer.NewWorkLocationName = data.NewWorkLocationName;
				existPromoteAndTransfer.CurrentTitle = data.CurrentTitle;
				existPromoteAndTransfer.CurrentJobGrade = data.CurrentJobGrade;
				existPromoteAndTransfer.CurrentDepartment = data.CurrentDepartment;
				existPromoteAndTransfer.CurrentWorkLocation = data.CurrentWorkLocation;
				//CR222
				existPromoteAndTransfer.PersonnelArea = data.PersonnelArea;
				existPromoteAndTransfer.PersonnelAreaText = data.PersonnelAreaText;
				existPromoteAndTransfer.EmployeeGroup = data.EmployeeGroup;
				existPromoteAndTransfer.EmployeeGroupDescription = data.EmployeeGroupDescription;
				existPromoteAndTransfer.EmployeeSubgroup = data.EmployeeSubgroup;
				existPromoteAndTransfer.EmployeeSubgroupDescription = data.EmployeeSubgroupDescription;
				existPromoteAndTransfer.PayScaleArea = data.PayScaleArea;
				//========================================================================
				existPromoteAndTransfer.ReportToId = data.ReportToId;
				existPromoteAndTransfer.Documents = data.Documents;
				existPromoteAndTransfer.PositionId = data.PositionId;
				existPromoteAndTransfer.PositionName = data.PositionName;
				existPromoteAndTransfer.CurrentJobGradeValue = data.CurrentJobGradeValue;
				existPromoteAndTransfer.ReportToFullName = data.ReportToFullName;
				existPromoteAndTransfer.ReportToSAPCode = data.ReportToSAPCode;
				existPromoteAndTransfer.Documents = data.Documents;
				existPromoteAndTransfer.IsSameDepartment = await isSameDepartment(existPromoteAndTransfer);
				existPromoteAndTransfer.ActingPositionId = data.ActingPositionId;
                //===== CR11.2 =====
                existPromoteAndTransfer.CurrentJobGradeGradeId = data.CurrentJobGradeGradeId;
                await _uow.CommitAsync();
			}
			return new ResultDTO { Object = Mapper.Map<PromoteAndTransferViewModel>(existPromoteAndTransfer) };
		}
		private async Task<string> ConcatenationDeptName(Department deptment, string dataPrint)
		{
			string result = string.Empty;
			result = dataPrint;
			try
			{
				var jobGrade = await _uow.GetRepository<JobGrade>(true).GetSingleAsync(x => x.Id == deptment.JobGradeId);
				var allJobGrade = await _uow.GetRepository<JobGrade>(true).GetAllAsync();
				
				var JD5Value = allJobGrade?
					.FirstOrDefault(x => (x.Title ?? string.Empty).ToUpper() == "G5")
					?.Grade ?? 5;
				var JD4Value = allJobGrade?
					.FirstOrDefault(x => (x.Title ?? string.Empty).ToUpper() == "G4")
					?.Grade ?? 4;
				var JD8Value = allJobGrade?
					.FirstOrDefault(x => (x.Title ?? string.Empty).ToUpper() == "G8")
					?.Grade ?? 8;

				
				// Current department is Store
				if (deptment.IsStore)
				{
					if (jobGrade != null)
					{
						var currParentDeptId = deptment.ParentId;
						//Current department < G5
						// if (jobGrade.Grade < 5)
						if (jobGrade.Grade < JD5Value)
						{
							for (int i = 1; i < 3; i++)
							{
								var parentDept = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.Id == currParentDeptId);
								if (parentDept != null)
								{
									var parentJobGrade = await _uow.GetRepository<JobGrade>(true).GetSingleAsync(x => x.Id == parentDept.JobGradeId);
									if (parentJobGrade != null)
									{
										if (parentJobGrade.Grade <= JD4Value)
										{
											result = parentDept.Name + " _ " + result;
										}
										else
											break;
									}
								}
								currParentDeptId = parentDept.ParentId;
							}
						}
					}
				}
				else // Current department is HQ
				{
					if (jobGrade != null)
					{
						//Current department < G5
						if (jobGrade.Grade < JD5Value)
						{
							var currParentDeptId = deptment.ParentId;
							for (int i = 1; i < 3; i++)
							{
								var parentDept = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.Id == currParentDeptId);
								if (parentDept != null)
								{
									var parentJobGrade = await _uow.GetRepository<JobGrade>(true).GetSingleAsync(x => x.Id == parentDept.JobGradeId);
									if (parentJobGrade != null)
									{
										if (parentJobGrade.Grade < JD8Value)
										{
											result = parentDept.Name + " _ " + result;
										}
										else
											break;
									}
									currParentDeptId = parentDept.ParentId;
								}
							}
						}
						else //Current department >= G5
						{
							var parentDept = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.Id == deptment.ParentId);
							if (parentDept != null)
							{
								var parentJobGrade = await _uow.GetRepository<JobGrade>(true).GetSingleAsync(x => x.Id == parentDept.JobGradeId);
								if (parentJobGrade != null)
								{
									if (parentJobGrade.Grade < JD8Value)
									{
										result = parentDept.Name + " _ " + result;
									}
								}
							}
						}
					}
				}
				return result;

			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
		private async Task<List<string>> GetListRemoveStoreName()
		{
			List<string> resultList = new List<string>();
			try
			{
				QueryArgs arg = new QueryArgs
				{
					Page = 1,
					Limit = 50,
					Predicate = string.Empty,
					PredicateParameters = new object[] { },
					Order = "Modified desc"
				};
				var totalKeyWords = await _uow.GetRepository<MaintainPromoteAndTranferPrint>().CountAsync(arg.Predicate, arg.PredicateParameters);

				decimal totalPages = (decimal)totalKeyWords / (decimal)arg.Limit;

				int pages = (int)Math.Ceiling(totalPages);

				for (int i = 0; i < pages; i++)
				{
					var removingValues = await _uow.GetRepository<MaintainPromoteAndTranferPrint>().FindByAsync<PromoteAndTranferPrintViewModel>(arg.Order, i + 1, arg.Limit, arg.Predicate, arg.PredicateParameters);
					if (removingValues != null)
					{
						foreach (var item in removingValues)
						{
							resultList.Add(item.RemovingValue);
						}
					}
				}

				return resultList;
			}
			catch (Exception ex)
			{
				return new List<string>();

			}
		}
		public async Task<byte[]> PrintForm(Guid Id)
		{
			byte[] result = null;
			List<string> listPrintPositionWithActing = new List<string> { "ProAndTran", "Pro" };
			var record = await _uow.GetRepository<PromoteAndTransfer>().FindByIdAsync(Id);
			if (record != null)
			{
				var dataToPrint = Mapper.Map<PromoteTransferPrintFormViewModel>(record);
				var tbPros = await GetWorkFlowHistories(Id, ObjectToPrintFromType.PromoteAndTransfer, dataToPrint);
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				dataToPrint.NewSalaryBenefit = record.NewSalaryOrBenefit.ToString("#,###", cul.NumberFormat);
				//CR224================
				Regex rgx = new Regex(@"\s?.(G\d).");
				//Current position name
				var currDept = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.Id == record.CurrentDepartmentId);
				if (currDept != null)
				{
					dataToPrint.CurrentPositionName = (listPrintPositionWithActing.Contains(record.TypeCode) ? "Acting " : "") + currDept.PositionName;
				}
				var rgxPositionName = rgx.Replace(dataToPrint.CurrentPositionName, "");
				dataToPrint.CurrentPositionName = rgxPositionName.Trim();

				//Current work location
				var RTHCurrDeptId = await _uow.GetRepository<RequestToHire>(true).GetSingleAsync(x => x.DeptDivisionId == record.CurrentDepartmentId);
				if (RTHCurrDeptId != null)
				{
					var Curr_WorkingLocationAddress = await _uow.GetRepository<WorkingAddressRecruitment>(true).GetSingleAsync(x => x.Id == RTHCurrDeptId.WorkingAddressRecruitmentId);
					if (Curr_WorkingLocationAddress != null)
					{
						dataToPrint.CurrentWorkLocationName = dataToPrint.CurrentWorkLocationName + " - " + Curr_WorkingLocationAddress.Address;
					}
				}
				// New work location
				var RTHNewDeptId = await _uow.GetRepository<RequestToHire>(true).GetSingleAsync(x => x.DeptDivisionId == record.NewDeptOrLineId);
				if (RTHNewDeptId != null)
				{
					var New_WorkingLocationAddress = await _uow.GetRepository<WorkingAddressRecruitment>(true).GetSingleAsync(x => x.Id == RTHNewDeptId.WorkingAddressRecruitmentId);
					if (New_WorkingLocationAddress != null)
					{
						dataToPrint.NewWorkLocationName = dataToPrint.NewWorkLocationName + " - " + New_WorkingLocationAddress.Address;
					}
				}
				//Phòng ban/ Ngành hàng – Bộ phận/ Nhóm hiện tại

				if (currDept != null)
				{
					string stringConCat = await ConcatenationDeptName(currDept, dataToPrint.CurrentDepartmentName);
					if (!string.IsNullOrEmpty(stringConCat))
					{
						dataToPrint.CurrentDepartmentName = stringConCat;
					}
				}
				/*var removeGCurrDept = rgx.Replace(dataToPrint.CurrentDepartmentName, "");
				dataToPrint.CurrentDepartmentName = removeGCurrDept.Trim();*/
				//Replace store name  
				List<string> listStoreName = await GetListRemoveStoreName();
				string strStoreName = string.Empty;
				if (listStoreName != null && listStoreName.Count > 0)
				{
					foreach (var item in listStoreName)
					{
						if (string.IsNullOrEmpty(strStoreName))
						{
							strStoreName = item;
						}
						else
							strStoreName = strStoreName + "|" + item;
					}
				}

				Regex regex = new Regex($@"(\s?|-\s?|-)({strStoreName})");
				if (!string.IsNullOrEmpty(strStoreName))
				{
					var removeStoreNameCurrDept = regex.Replace(dataToPrint.CurrentDepartmentName, "");
					dataToPrint.CurrentDepartmentName = removeStoreNameCurrDept.Trim();
				}

				//Phòng ban/ Ngành hàng – Bộ phận/ Nhóm mới
				var newDept = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.Id == record.NewDeptOrLineId);
				if (newDept != null)
				{
					string stringConCat = await ConcatenationDeptName(newDept, dataToPrint.NewDepartmentName);
					if (!string.IsNullOrEmpty(stringConCat))
					{
						dataToPrint.NewDepartmentName = stringConCat;
					}
				}
				/*var removeGNewDept = rgx.Replace(dataToPrint.NewDepartmentName, "");
				dataToPrint.NewDepartmentName = removeGNewDept.Trim();*/
				if (!string.IsNullOrEmpty(strStoreName))
				{
					var removeStoreNameNewDept = regex.Replace(dataToPrint.NewDepartmentName, "");
					dataToPrint.NewDepartmentName = removeStoreNameNewDept.Trim();
				}

				// Report to = name + position name
				// fix bug AMOAEON-654
				//var reportToUserMapping = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.UserId == record.ReportToId);
				var reportToUserMapping = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.UserId == record.ReportToId && x.IsHeadCount);
				// end AMOAEON-654


				if (!(reportToUserMapping is null))
				{
					var positionName = reportToUserMapping.Department.PositionName is null ? "" : reportToUserMapping.Department.PositionName;
					dataToPrint.ReportToUser = $"{dataToPrint.ReportToUser} - {positionName}";
				}
				//=====================

				#region Task #10671 get title instead grade 
				//if (record.CurrentDepartmentId.HasValue)
				//{
				//	var currentDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == record.CurrentDepartmentId.Value);
				//	if (!(currentDepartment is null) && !(currentDepartment.JobGrade is null))
				//		dataToPrint.CurrentJobGradeName = !string.IsNullOrEmpty(currentDepartment.JobGrade.Title) ? currentDepartment.JobGrade.Title : "";
				//}
				if (record.NewJobGradeId.HasValue)
				{
					var newJobgrade = await _uow.GetRepository<JobGrade>().GetSingleAsync(x => x.Id == record.NewJobGradeId.Value);
					if (!(newJobgrade is null))
						dataToPrint.NewJobGradeName = !string.IsNullOrEmpty(newJobgrade.Title) ? newJobgrade.Title : "";
				}
				#endregion


				var properties = typeof(PromoteTransferPrintFormViewModel).GetProperties();
				var pros = new Dictionary<string, string>();
				foreach (var property in properties)
				{
					var value = Convert.ToString(property.GetValue(dataToPrint));
					pros[property.Name] = SecurityElement.Escape(value);
				}
				result = WordAutomation.ExportPDF("PromoteTransfer.docx", pros, tbPros);
			}
			return result;
		}
		public async Task<List<Dictionary<string, string>>> GetWorkFlowHistories(Guid Id, ObjectToPrintFromType type, object dataToPrint = null)
		{
			var wfStatus = await _workflowBO.GetWorkflowStatusByItemId(Id);
			var workflowStatus = (WorkflowStatusViewModel)wfStatus.Object;
			var tbPros = new List<Dictionary<string, string>>();
			if (workflowStatus != null && workflowStatus.WorkflowInstances != null)
			{
				var wfInstanceList = workflowStatus.WorkflowInstances.FirstOrDefault();
				if (wfInstanceList != null && wfInstanceList.Histories != null)
				{
					// sua tam in PromoteTransfer
					if (wfInstanceList.WorkflowName == "Promotion for Store G1,G2 (Other Department) - Request form Company with Emp Confirmation"
						|| wfInstanceList.WorkflowName == "Promotion for Store G1,G2 (Other Department) - Request from Company with Emp Confirmation"
						|| wfInstanceList.WorkflowName == "Promotion for Store G3,G4 (Other Department) - Request from Company with Emp"
						|| wfInstanceList.WorkflowName == "Promotion for HQ G3,G4 (Other Department) - Request from Company with Emp Confirmation"
						|| wfInstanceList.WorkflowName == "Promotion for HQ G1,G2(Other Department) - Request from Company with Emp Confirmation")
					{
						await UpdateAdditionInfo_temp(wfInstanceList, type, dataToPrint);
					}
					else
					{
						await UpdateAdditionInfo(wfInstanceList, type, dataToPrint);
					}
					foreach (var item in wfInstanceList.Histories)
					{
						if (item != null && item.IsStepCompleted)
						{
							var rowPros = new Dictionary<string, string>();
							rowPros["AssignedDate"] = SecurityElement.Escape(item.Modified.ToString("dd/MM/yyyy"));
							rowPros["AssignedBy"] = SecurityElement.Escape(item.ApproverFullName);
							rowPros["Outcome"] = SecurityElement.Escape(item.Outcome);
							rowPros["Comment"] = SecurityElement.Escape(item.Comment);
							tbPros.Add(rowPros);
						}
					}
				}
				else
				{
					CreateEmptyTbPros(tbPros);
				}
			}
			else
			{
				CreateEmptyTbPros(tbPros);
			}
			return tbPros;
		}
		private void CreateEmptyTbPros(List<Dictionary<string, string>> tbProsTemp)
		{
			var rowPros = new Dictionary<string, string>();
			rowPros["AssignedDate"] = "";
			rowPros["AssignedBy"] = "";
			rowPros["Outcome"] = "";
			rowPros["Comment"] = "";
			tbProsTemp.Add(rowPros);
		}
		private async Task UpdateAdditionInfo(WorkflowInstanceViewModel workflowInstanceView, ObjectToPrintFromType type, object dataToPrint = null)
		{
			var histories = workflowInstanceView.Histories.ToList();
			if (type == ObjectToPrintFromType.PromoteAndTransfer)
			{
				var allJobGrades = await _uow.GetRepository<JobGrade>().GetAllAsync<JobGradeViewModel>();
				var jd5 = allJobGrades.FirstOrDefault(x => x.Title.ToUpper().Equals("G5"));
				var jd5Value = jd5?.Grade ?? 5;
				var jdMaxGrade = allJobGrades.Max(x => x.Grade);
				var promoteAndTransferInstance = (PromoteTransferPrintFormViewModel)dataToPrint;
				// cheat code phieu PRO-000000028-2023
				bool isPromote28_2023 = !string.IsNullOrEmpty(workflowInstanceView.ItemReferenceNumber) && workflowInstanceView.ItemReferenceNumber.Equals("PRO-000000028-2023", StringComparison.OrdinalIgnoreCase) ? true : false;
				int i = 0;
				foreach (var item in workflowInstanceView.WorkflowData.Steps)
				{
					var currentHistory = histories.ElementAt(i);
					if (currentHistory.IsStepCompleted)
					{
						var newJobGradeValue = 0;
						if (promoteAndTransferInstance.NewJobGradeId.HasValue)
						{
							newJobGradeValue = allJobGrades.FirstOrDefault(x => x.Id == promoteAndTransferInstance.NewJobGradeId.Value).Grade;
						}
						if (item.StepNumber == 1)
						{
							promoteAndTransferInstance.LineManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
							promoteAndTransferInstance.LManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
						}
						else if (item.StepNumber == 2)
						{
							promoteAndTransferInstance.StoreManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
							promoteAndTransferInstance.SManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
						}
						else if (item.StepNumber == 3)
						{
							// Update : Nếu NewJobGrade là G1, G2 hoặc NewJobGrade là G5 và IsStore của New Department = true và
							// IsSameDepartment = false
							// => thì không hiện gía trị
							// if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == 5 && promoteAndTransferInstance.IsStoreNewDepartment && !promoteAndTransferInstance.IsSameDepartment))
							if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == jd5Value && promoteAndTransferInstance.IsStoreNewDepartment && !promoteAndTransferInstance.IsSameDepartment))
							{

							}
							else
							{
								promoteAndTransferInstance.SGMOperation = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.SGMOperationSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
							}
							if (!promoteAndTransferInstance.IsSameDepartment)
							{
								// if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == 5 && promoteAndTransferInstance.IsStoreNewDepartment))
								if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == jd5Value && promoteAndTransferInstance.IsStoreNewDepartment))
								{
									promoteAndTransferInstance.NewLineManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									promoteAndTransferInstance.NewLManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								}
							}
							if (isPromote28_2023)
							{
								promoteAndTransferInstance.LineManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.LManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
							}
						}
						else if (item.StepNumber == 4)
						{
							if (!promoteAndTransferInstance.IsSameDepartment)
							{
								if (string.IsNullOrEmpty(promoteAndTransferInstance.NewLineManager))
								{
									promoteAndTransferInstance.NewLineManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									promoteAndTransferInstance.NewLManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								}
								// if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == 5 && promoteAndTransferInstance.IsStoreNewDepartment))
								if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == jd5Value && promoteAndTransferInstance.IsStoreNewDepartment))
								{
									promoteAndTransferInstance.NewStoreManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									promoteAndTransferInstance.NewSManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								}
								if (newJobGradeValue == 5 && promoteAndTransferInstance.IsStoreNewDepartment)
								{
									promoteAndTransferInstance.NewSGMOperation = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									promoteAndTransferInstance.NewSGMOperationSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								}
							}
							if (isPromote28_2023)
							{
								promoteAndTransferInstance.StoreManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.SManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								promoteAndTransferInstance.NewSGMOperation = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.NewSGMOperationSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								promoteAndTransferInstance.SGMOperation = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.SGMOperationSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								promoteAndTransferInstance.NewStoreManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.NewSManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
							}
						}
						else if (item.StepNumber == 5)
						{
							if (string.IsNullOrEmpty(promoteAndTransferInstance.NewStoreManager) && !promoteAndTransferInstance.IsSameDepartment)
							{
								promoteAndTransferInstance.NewStoreManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.NewSManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
							}
							if (isPromote28_2023)
							{
								promoteAndTransferInstance.NewLineManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.NewLManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
							}
						}
						else if (item.StepNumber == 6)
						{
							if (!promoteAndTransferInstance.IsSameDepartment && (string.IsNullOrEmpty(promoteAndTransferInstance.NewSGMOperation) && !((newJobGradeValue == 1 || newJobGradeValue == 2))))
							{
								promoteAndTransferInstance.NewSGMOperation = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.NewSGMOperationSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
							}

						}
					}
					i++;
				}

				//cheat code cho PRO-000000085-2022
				if (workflowInstanceView.WorkflowName == "Promotion for Store G5")
				{
					var currentSGMOperation = histories.ElementAt(1);
					promoteAndTransferInstance.SGMOperation = !string.IsNullOrEmpty(currentSGMOperation.ApproverFullName) ? currentSGMOperation.ApproverFullName + " Signed" : "";
					promoteAndTransferInstance.SGMOperationSignedDate = currentSGMOperation.Modified.ToLocalTime().ToString("dd/MM/yyyy");
				}

				var generalDirectorStep = 0;
				var hrManagerStep = 0;
				var allDepartments = await _uow.GetRepository<Department>().GetAllAsync<DepartmentViewModel>();
				histories.ForEach(async x =>
				{
					if (generalDirectorStep == 0 || hrManagerStep == 0)
					{
						var departmentInCurrentStep = allDepartments.Where(y => x.AssignedToDepartmentId != null && y.Id == x.AssignedToDepartmentId.Value).FirstOrDefault();

						if (departmentInCurrentStep != null)
						{
							// if (departmentInCurrentStep.IsHr && departmentInCurrentStep.JobGradeGrade == 5)
							if (departmentInCurrentStep.IsHr && departmentInCurrentStep.JobGradeGrade == jd5Value)
							{
								hrManagerStep = x.StepNumber;
							}
							// if (departmentInCurrentStep.JobGradeGrade == 10)
							if (departmentInCurrentStep.JobGradeGrade == jdMaxGrade)
							{
								generalDirectorStep = x.StepNumber;
							}
						}
					}
				});
				//var hrManagerStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.IsHRHQ && x.JobGrade == "G5").FirstOrDefault();
				if (hrManagerStep > 0)
				{
					if (histories.Count > (hrManagerStep - 1))
					{
						var hrManagerHistory = histories.ElementAt(hrManagerStep - 1);
						if (hrManagerHistory.IsStepCompleted)
						{
							promoteAndTransferInstance.HRManager = !string.IsNullOrEmpty(hrManagerHistory.ApproverFullName) ? hrManagerHistory.ApproverFullName + " Signed" : "";
							promoteAndTransferInstance.HRManagerSignedDate = hrManagerHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
						}
					}
				}

				if (isPromote28_2023)
				{
					var hrManagerHistory = histories.ElementAt(7);
					if (hrManagerHistory.IsStepCompleted)
					{
						promoteAndTransferInstance.HRManager = !string.IsNullOrEmpty(hrManagerHistory.ApproverFullName) ? hrManagerHistory.ApproverFullName + " Signed" : "";
						promoteAndTransferInstance.HRManagerSignedDate = hrManagerHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
					}
				}

				//var generalDirectorStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.IsHRHQ && x.JobGrade == "G9").FirstOrDefault();
				if (generalDirectorStep > 0)
				{
					if (histories.Count > (generalDirectorStep - 1))
					{
						var generalDirectorHistory = histories.ElementAt(generalDirectorStep - 1);
						if (generalDirectorHistory.IsStepCompleted)
						{
							promoteAndTransferInstance.GeneralDirector = !string.IsNullOrEmpty(generalDirectorHistory.ApproverFullName) ? generalDirectorHistory.ApproverFullName + " Signed" : "";
							promoteAndTransferInstance.GDirectorSignedDate = generalDirectorHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
						}

					}

				}
				dataToPrint = promoteAndTransferInstance;
			}
			else if (type == ObjectToPrintFromType.Acting)
			{
				var actingInstance = (ActingPrintFormViewModel)dataToPrint;
				//var hRManagerStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepNumber == 6).FirstOrDefault();
				var hRManagerStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepName == "HR Manager Approval").FirstOrDefault();
				if (hRManagerStep != null)
				{
					if (histories.Count > (hRManagerStep.StepNumber - 1))
					{
						var hRManagerHistory = histories.ElementAt(hRManagerStep.StepNumber - 1);
						if (hRManagerHistory.IsStepCompleted)
						{
							actingInstance.HRManager = hRManagerHistory.ApproverFullName;
						}

					}

				}
				//CR210
				//var firstAppraisedStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepNumber == workflowInstanceView.WorkflowData.Steps.Count - 1).FirstOrDefault();
				var firstAppraisedStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepName == "Appraiser 1").FirstOrDefault();
				if (firstAppraisedStep != null)
				{
					if (histories.Count > (firstAppraisedStep.StepNumber - 1))
					{
						var firstAppraisedStepHistory = histories.ElementAt(firstAppraisedStep.StepNumber - 1);
						if (firstAppraisedStepHistory.IsStepCompleted)
						{
							actingInstance.FirstAppraiserConfirmation = firstAppraisedStepHistory.ApproverFullName;
							//CR210===================
							actingInstance.FirstAppraiserComment = firstAppraisedStepHistory.Comment;
							//========================

						}

					}

				}
				//var secondAppraisedStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepNumber == workflowInstanceView.WorkflowData.Steps.Count).FirstOrDefault();
				var secondAppraisedStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepName == "Appraiser 2").FirstOrDefault();
				if (secondAppraisedStep != null)
				{
					if (histories.Count > (secondAppraisedStep.StepNumber - 1))
					{
						var secondAppraisedStepHistory = histories.ElementAt(secondAppraisedStep.StepNumber - 1);
						if (secondAppraisedStepHistory.IsStepCompleted)
						{
							actingInstance.SecondAppraiserConfirmation = secondAppraisedStepHistory.ApproverFullName;
							//CR210===================
							actingInstance.SecondAppraiserComment = secondAppraisedStepHistory.Comment;
							//========================
						}
					}

				}
				dataToPrint = actingInstance;
			}
			else if (type == ObjectToPrintFromType.RequestToHire)
			{
				var stepBudget = "Budget Checker";
				var stepAcknow = "Acknowledgement";
				var stepRecruiter = "Recruiter Assignment";
				var requestToHireToPrintForm = (RequestToHireForPrintViewModel)dataToPrint;
				int i = 0;
				foreach (var item in workflowInstanceView.WorkflowData.Steps)
				{
					if (i < histories.Count)
					{
						var currentHistory = histories.ElementAt(i);
						if (currentHistory.IsStepCompleted)
						{
							//Khiem - fix 451
							if (item.StepName.ToUpper() == stepBudget.ToUpper() || item.StepName.ToUpper() == stepAcknow.ToUpper() || item.StepName.ToUpper() == stepRecruiter.ToUpper())
							{
								i++;
								continue;
							}
							if (item.StepNumber == 1)
							{
								requestToHireToPrintForm.ApproverStep1 = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.ApproverStep1NotSigned = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName : "";
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{
										Department assignedDepartment = await GetDepartmentById(currentHistory.AssignedToDepartmentId.Value);
										if (assignedDepartment == null)
										{
											assignedDepartment = await GetDepartmentNotIsDeletedById(currentHistory.AssignedToDepartmentId.Value);
										}
										if (assignedDepartment != null)
                                        {
											// HR-844 Fix lỗi print file khi department bị xóa
											requestToHireToPrintForm.DepartmentOfOfApproverStep1 = assignedDepartment.Name;
											requestToHireToPrintForm.PositionOfApproverStep1 = department.PositionName;
											requestToHireToPrintForm.ApproverStep1Position = department.PositionName;
										}
									}
								}

								requestToHireToPrintForm.CompletedDateStep1 = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								requestToHireToPrintForm.StepSigned1Date = requestToHireToPrintForm.CompletedDateStep1;
							}
							if (item.StepNumber == 2)
							{
								requestToHireToPrintForm.ApproverStep2 = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.StepSigned2Date = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{

										requestToHireToPrintForm.ApproverStep2Position = department.PositionName;
									}
								}
							}
							else if (item.StepNumber == 3 && requestToHireToPrintForm.JobGrade != 1 && requestToHireToPrintForm.JobGrade != 2)
							{
								requestToHireToPrintForm.ApproverStep3 = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.StepSigned3Date = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{

										requestToHireToPrintForm.ApproverStep3Position = department.PositionName;
									}
								}

							}
							else if (item.StepNumber == 4 && requestToHireToPrintForm.JobGrade != 1 && requestToHireToPrintForm.JobGrade != 2)
							{
								requestToHireToPrintForm.ApproverStep4 = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.StepSigned4Date = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{

										requestToHireToPrintForm.ApproverStep4Position = department.PositionName;
									}
								}
							}
							else if (item.StepNumber == 5)
							{
								requestToHireToPrintForm.ApproverStep5 = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.StepSigned5Date = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{

										requestToHireToPrintForm.ApproverStep5Position = department.PositionName;
										/*if (requestToHireToPrintForm.JobGrade == "G1" || requestToHireToPrintForm.JobGrade == "G2")
                                        {
                                            requestToHireToPrintForm.HrManager = requestToHireToPrintForm.ApproverStep5;
                                            requestToHireToPrintForm.HRSignedDate = requestToHireToPrintForm.StepSigned5Date;
                                            requestToHireToPrintForm.HRManagerPosition = department.PositionName;

                                        }*/
									}

								}


							}
							else if (item.StepNumber == 7)
							{
								if (requestToHireToPrintForm.JobGrade == 4 && requestToHireToPrintForm.WorkingTimeCode == "STORE" || requestToHireToPrintForm.JobGrade == 5)
								{
									requestToHireToPrintForm.HrManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									requestToHireToPrintForm.HRSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
									if (currentHistory.ApproverId.HasValue)
									{
										var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
										if (department != null)
										{
											requestToHireToPrintForm.HRManagerPosition = department.PositionName;
										}
									}
								}

							}
							else if (item.StepNumber == 8)
							{
								if (requestToHireToPrintForm.JobGrade == 4 && requestToHireToPrintForm.WorkingTimeCode == "HQ")
								{
									requestToHireToPrintForm.HrManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									requestToHireToPrintForm.HRSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
									if (currentHistory.ApproverId.HasValue)
									{
										var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
										if (department != null)
										{
											requestToHireToPrintForm.HRManagerPosition = department.PositionName;
										}
									}
								}
							}
							else if (item.StepNumber == 9)
							{
								if (requestToHireToPrintForm.JobGrade == 3)
								{
									requestToHireToPrintForm.HrManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									requestToHireToPrintForm.HRSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
									if (currentHistory.ApproverId.HasValue)
									{
										var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
										if (department != null)
										{
											requestToHireToPrintForm.HRManagerPosition = department.PositionName;
										}
									}
								}
							}

							//Fix HR Manager follow by step name
							if (!string.IsNullOrEmpty(item.StepName) && item.StepName.Equals("HR Approval", StringComparison.OrdinalIgnoreCase))
							{
								requestToHireToPrintForm.HrManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.HRSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{
										requestToHireToPrintForm.HRManagerPosition = department.PositionName;
									}
								}
							}
						}
					}
					else
					{
						break;
					}
					i++;
				}
			}
		}

		private async Task UpdateAdditionInfo_temp(WorkflowInstanceViewModel workflowInstanceView, ObjectToPrintFromType type, object dataToPrint = null)
		{
			var wfName = workflowInstanceView.WorkflowName;

			// set up step 
			int AcknowledgementStep = 7;
			int HrCheckingStep = 6;

			//if (wfName == "Promotion for Store G3,G4 (Other Department) - Request from Company with Emp")
			//{
			//	AcknowledgementStep = 8;
			//	HrCheckingStep = 7;
			//}
			//else if (wfName == "Promotion for HQ G3,G4 (Other Department) - Request from Company with Emp Confirmation")
			//{
			//	AcknowledgementStep = 9;
			//	HrCheckingStep = 8;
			//}

			var histories = workflowInstanceView.Histories.ToList();
			if (type == ObjectToPrintFromType.PromoteAndTransfer)
			{
				var allJobGrades = await _uow.GetRepository<JobGrade>().GetAllAsync<JobGradeViewModel>();
				var promoteAndTransferInstance = (PromoteTransferPrintFormViewModel)dataToPrint;

				var jd5 = allJobGrades.FirstOrDefault(x => x.Title.ToUpper().Equals("G5"));
				var jd5Value = jd5?.Grade ?? 5;
				var jdMaxGrade = allJobGrades.Max(x => x.Grade);
				
				var wfStep = workflowInstanceView.WorkflowData.Steps;
				foreach (var currentHistory in histories)
				{
					WorkflowStepViewModel item = wfStep.Where(x => x.StepNumber == currentHistory.StepNumber).FirstOrDefault();
					if (item != null)
					{
						if (currentHistory.IsStepCompleted)
						{
							var newJobGradeValue = 0;
							if (promoteAndTransferInstance.NewJobGradeId.HasValue)
							{
								newJobGradeValue = allJobGrades.FirstOrDefault(x => x.Id == promoteAndTransferInstance.NewJobGradeId.Value).Grade;
							}
							if (item.StepNumber == 1)
							{
								promoteAndTransferInstance.LineManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.LManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
							}
							else if (item.StepNumber == 3)
							{
								promoteAndTransferInstance.StoreManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								promoteAndTransferInstance.SManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
							}
							else if (item.StepNumber == HrCheckingStep - 2)
							{
								// Update : Nếu NewJobGrade là G1, G2 hoặc NewJobGrade là G5 và IsStore của New Department = true và
								// IsSameDepartment = false
								// => thì không hiện gía trị
								//if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == 5 && promoteAndTransferInstance.IsStoreNewDepartment && !promoteAndTransferInstance.IsSameDepartment))
								if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == jd5Value && promoteAndTransferInstance.IsStoreNewDepartment && !promoteAndTransferInstance.IsSameDepartment))
								{

								}
								else
								{
									promoteAndTransferInstance.SGMOperation = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									promoteAndTransferInstance.SGMOperationSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								}
								if (!promoteAndTransferInstance.IsSameDepartment)
								{
									// if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == 5 && promoteAndTransferInstance.IsStoreNewDepartment))
									if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == jd5Value && promoteAndTransferInstance.IsStoreNewDepartment))
									{  
										promoteAndTransferInstance.NewLineManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
										promoteAndTransferInstance.NewLManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
									}
								}


							}
							else if (item.StepNumber == HrCheckingStep - 1)
							{
								if (!promoteAndTransferInstance.IsSameDepartment)
								{
									if (string.IsNullOrEmpty(promoteAndTransferInstance.NewLineManager))
									{
										promoteAndTransferInstance.NewLineManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
										promoteAndTransferInstance.NewLManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
									}
									// if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == 5 && promoteAndTransferInstance.IsStoreNewDepartment))
									if ((newJobGradeValue == 1 || newJobGradeValue == 2) || (newJobGradeValue == jd5Value && promoteAndTransferInstance.IsStoreNewDepartment))
									{
										//fix issue khong in neu khong phải là store
										if (promoteAndTransferInstance.IsStoreNewDepartment) {
											promoteAndTransferInstance.NewStoreManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
											promoteAndTransferInstance.NewSManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
										}
										//end

										//promoteAndTransferInstance.NewStoreManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									    //promoteAndTransferInstance.NewSManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");

									}
									if (newJobGradeValue == 5 && promoteAndTransferInstance.IsStoreNewDepartment)
									{
										promoteAndTransferInstance.NewSGMOperation = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
										promoteAndTransferInstance.NewSGMOperationSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
									}
								}
							}
							else if (item.StepNumber == HrCheckingStep)
							{
								//if (string.IsNullOrEmpty(promoteAndTransferInstance.NewStoreManager) && !promoteAndTransferInstance.IsSameDepartment) 
								if (string.IsNullOrEmpty(promoteAndTransferInstance.NewStoreManager) && !promoteAndTransferInstance.IsSameDepartment && promoteAndTransferInstance.IsStoreNewDepartment)
								{
									promoteAndTransferInstance.NewStoreManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									promoteAndTransferInstance.NewSManagerSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								}
							}
							else if (item.StepNumber == AcknowledgementStep)
							{
								if (!promoteAndTransferInstance.IsSameDepartment && (string.IsNullOrEmpty(promoteAndTransferInstance.NewSGMOperation) && !((newJobGradeValue == 1 || newJobGradeValue == 2))))
								{
									promoteAndTransferInstance.NewSGMOperation = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									promoteAndTransferInstance.NewSGMOperationSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								}

							}
						}
					}
				}

				var generalDirectorStep = 0;
				var hrManagerStep = 0;
				var allDepartments = await _uow.GetRepository<Department>().GetAllAsync<DepartmentViewModel>();
				histories.ForEach(async x =>
				{
					if (generalDirectorStep == 0 || hrManagerStep == 0)
					{
						var departmentInCurrentStep = allDepartments.Where(y => x.AssignedToDepartmentId != null && y.Id == x.AssignedToDepartmentId.Value).FirstOrDefault();

						if (departmentInCurrentStep != null)
						{
							// if (departmentInCurrentStep.IsHr && departmentInCurrentStep.JobGradeGrade == 5)
							if (departmentInCurrentStep.IsHr && departmentInCurrentStep.JobGradeGrade == jd5Value)
							{
								hrManagerStep = x.StepNumber;
							}
							// if (departmentInCurrentStep.JobGradeGrade == 10)
							if (departmentInCurrentStep.JobGradeGrade == jdMaxGrade)
							{
								generalDirectorStep = x.StepNumber;
							}
						}
					}
				});
				//var hrManagerStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.IsHRHQ && x.JobGrade == "G5").FirstOrDefault();
				if (hrManagerStep > 0)
				{
					if (histories.Count >= (hrManagerStep - 1))
					{  
						var hrManagerHistory = histories.Count == (hrManagerStep - 1) ? histories.ElementAt(hrManagerStep - 2) : histories.ElementAt(hrManagerStep - 1);

						if (hrManagerHistory.IsStepCompleted)
						{
							promoteAndTransferInstance.HRManager = !string.IsNullOrEmpty(hrManagerHistory.ApproverFullName) ? hrManagerHistory.ApproverFullName + " Signed" : "";
							promoteAndTransferInstance.HRManagerSignedDate = hrManagerHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
						}
					}
				}

				//var generalDirectorStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.IsHRHQ && x.JobGrade == "G9").FirstOrDefault();
				if (generalDirectorStep > 0)
				{
					if (histories.Count > (generalDirectorStep - 1))
					{
						var generalDirectorHistory = histories.ElementAt(generalDirectorStep - 2);
						//var generalDirectorHistory = histories.ElementAt(generalDirectorStep); 
						if (generalDirectorHistory.IsStepCompleted)
						{
							promoteAndTransferInstance.GeneralDirector = !string.IsNullOrEmpty(generalDirectorHistory.ApproverFullName) ? generalDirectorHistory.ApproverFullName + " Signed" : "";
							promoteAndTransferInstance.GDirectorSignedDate = generalDirectorHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
						}

					}

				}
				dataToPrint = promoteAndTransferInstance;
			}
			else if (type == ObjectToPrintFromType.Acting)
			{
				var actingInstance = (ActingPrintFormViewModel)dataToPrint;
				//var hRManagerStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepNumber == 6).FirstOrDefault();
				var hRManagerStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepName == "HR Manager Approval").FirstOrDefault();
				if (hRManagerStep != null)
				{
					if (histories.Count > (hRManagerStep.StepNumber - 1))
					{
						var hRManagerHistory = histories.ElementAt(hRManagerStep.StepNumber - 1);
						if (hRManagerHistory.IsStepCompleted)
						{
							actingInstance.HRManager = hRManagerHistory.ApproverFullName;
						}

					}

				}
				//CR210
				//var firstAppraisedStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepNumber == workflowInstanceView.WorkflowData.Steps.Count - 1).FirstOrDefault();
				var firstAppraisedStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepName == "Appraiser 1").FirstOrDefault();
				if (firstAppraisedStep != null)
				{
					if (histories.Count > (firstAppraisedStep.StepNumber - 1))
					{
						var firstAppraisedStepHistory = histories.ElementAt(firstAppraisedStep.StepNumber - 1);
						if (firstAppraisedStepHistory.IsStepCompleted)
						{
							actingInstance.FirstAppraiserConfirmation = firstAppraisedStepHistory.ApproverFullName;
							//CR210===================
							actingInstance.FirstAppraiserComment = firstAppraisedStepHistory.Comment;
							//========================

						}

					}

				}
				//var secondAppraisedStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepNumber == workflowInstanceView.WorkflowData.Steps.Count).FirstOrDefault();
				var secondAppraisedStep = workflowInstanceView.WorkflowData.Steps.Where(x => x.StepName == "Appraiser 2").FirstOrDefault();
				if (secondAppraisedStep != null)
				{
					if (histories.Count > (secondAppraisedStep.StepNumber - 1))
					{
						var secondAppraisedStepHistory = histories.ElementAt(secondAppraisedStep.StepNumber - 1);
						if (secondAppraisedStepHistory.IsStepCompleted)
						{
							actingInstance.SecondAppraiserConfirmation = secondAppraisedStepHistory.ApproverFullName;
							//CR210===================
							actingInstance.SecondAppraiserComment = secondAppraisedStepHistory.Comment;
							//========================
						}
					}

				}
				dataToPrint = actingInstance;
			}
			else if (type == ObjectToPrintFromType.RequestToHire)
			{
				var stepBudget = "Budget Checker";
				var stepAcknow = "Acknowledgement";
				var stepRecruiter = "Recruiter Assignment";
				var requestToHireToPrintForm = (RequestToHireForPrintViewModel)dataToPrint;
				int i = 0;
				foreach (var item in workflowInstanceView.WorkflowData.Steps)
				{
					if (i < histories.Count)
					{
						var currentHistory = histories.ElementAt(i);
						if (currentHistory.IsStepCompleted)
						{
							//Khiem - fix 451
							if (item.StepName.ToUpper() == stepBudget.ToUpper() || item.StepName.ToUpper() == stepAcknow.ToUpper() || item.StepName.ToUpper() == stepRecruiter.ToUpper())
							{
								i++;
								continue;
							}
							if (item.StepNumber == 1)
							{
								requestToHireToPrintForm.ApproverStep1 = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.ApproverStep1NotSigned = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName : "";
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{
										Department assignedDepartment = await GetDepartmentById(currentHistory.AssignedToDepartmentId.Value);
										if (assignedDepartment == null)
										{
											assignedDepartment = await GetDepartmentNotIsDeletedById(currentHistory.AssignedToDepartmentId.Value);
										}
										if (assignedDepartment != null)
                                        {
											requestToHireToPrintForm.DepartmentOfOfApproverStep1 = assignedDepartment.Name;
											requestToHireToPrintForm.PositionOfApproverStep1 = department.PositionName;
											requestToHireToPrintForm.ApproverStep1Position = department.PositionName;
										} 
									}
								}

								requestToHireToPrintForm.CompletedDateStep1 = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								requestToHireToPrintForm.StepSigned1Date = requestToHireToPrintForm.CompletedDateStep1;
							}
							if (item.StepNumber == 2)
							{
								requestToHireToPrintForm.ApproverStep2 = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.StepSigned2Date = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{

										requestToHireToPrintForm.ApproverStep2Position = department.PositionName;
									}
								}
							}
							else if (item.StepNumber == 3 && requestToHireToPrintForm.JobGrade != 1 && requestToHireToPrintForm.JobGrade != 2)
							{
								requestToHireToPrintForm.ApproverStep3 = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.StepSigned3Date = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{

										requestToHireToPrintForm.ApproverStep3Position = department.PositionName;
									}
								}

							}
							else if (item.StepNumber == 4 && requestToHireToPrintForm.JobGrade != 1 && requestToHireToPrintForm.JobGrade != 2)
							{
								requestToHireToPrintForm.ApproverStep4 = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.StepSigned4Date = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{

										requestToHireToPrintForm.ApproverStep4Position = department.PositionName;
									}
								}
							}
							else if (item.StepNumber == 5)
							{
								requestToHireToPrintForm.ApproverStep5 = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.StepSigned5Date = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{

										requestToHireToPrintForm.ApproverStep5Position = department.PositionName;
										/*if (requestToHireToPrintForm.JobGrade == "G1" || requestToHireToPrintForm.JobGrade == "G2")
										{
											requestToHireToPrintForm.HrManager = requestToHireToPrintForm.ApproverStep5;
											requestToHireToPrintForm.HRSignedDate = requestToHireToPrintForm.StepSigned5Date;
											requestToHireToPrintForm.HRManagerPosition = department.PositionName;

										}*/
									}

								}


							}
							else if (item.StepNumber == 7)
							{
								if (requestToHireToPrintForm.JobGrade == 4 && requestToHireToPrintForm.WorkingTimeCode == "STORE" || requestToHireToPrintForm.JobGrade == 5)
								{
									requestToHireToPrintForm.HrManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									requestToHireToPrintForm.HRSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
									if (currentHistory.ApproverId.HasValue)
									{
										var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
										if (department != null)
										{
											requestToHireToPrintForm.HRManagerPosition = department.PositionName;
										}
									}
								}

							}
							else if (item.StepNumber == 8)
							{
								if (requestToHireToPrintForm.JobGrade == 4 && requestToHireToPrintForm.WorkingTimeCode == "HQ")
								{
									requestToHireToPrintForm.HrManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									requestToHireToPrintForm.HRSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
									if (currentHistory.ApproverId.HasValue)
									{
										var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
										if (department != null)
										{
											requestToHireToPrintForm.HRManagerPosition = department.PositionName;
										}
									}
								}
							}
							else if (item.StepNumber == 9)
							{
								if (requestToHireToPrintForm.JobGrade == 3)
								{
									requestToHireToPrintForm.HrManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
									requestToHireToPrintForm.HRSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
									if (currentHistory.ApproverId.HasValue)
									{
										var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
										if (department != null)
										{
											requestToHireToPrintForm.HRManagerPosition = department.PositionName;
										}
									}
								}
							}

							//Fix HR Manager follow by step name
							if (!string.IsNullOrEmpty(item.StepName) && item.StepName.Equals("HR Approval", StringComparison.OrdinalIgnoreCase))
							{
								requestToHireToPrintForm.HrManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
								requestToHireToPrintForm.HRSignedDate = currentHistory.Modified.ToLocalTime().ToString("dd/MM/yyyy");
								if (currentHistory.ApproverId.HasValue)
								{
									var department = await GetPositionOfAppoverById(currentHistory.ApproverId.Value);
									if (department != null)
									{
										requestToHireToPrintForm.HRManagerPosition = department.PositionName;
									}
								}
							}
						}
					}
					else
					{
						break;
					}
					i++;
				}
			}
		}

		private async Task<DepartmentViewModel> GetPositionOfAppoverById(Guid Id)
		{
			var departmentMapping = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync<UserDepartmentMappingViewModel>(x => x.UserId == Id && x.IsHeadCount);
			if (departmentMapping != null)
			{
				var department = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(departmentMapping.DepartmentId.Value);
				return department;
			}
			return null;
		}
		private async Task<Department> GetDepartmentById(Guid Id)
		{
			var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == Id);
			return department;
		}
		private async Task<Department> GetDepartmentNotIsDeletedById(Guid Id)
		{
			var department = await _uow.GetRepository<Department>().GetSingleAsyncIsNotDeleted(x => x.Id == Id);
			return department;
		}
	}
}
