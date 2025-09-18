using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.ExternalItem;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Aeon.HR.ViewModels;
using Aeon.HR.Infrastructure.Enums;
using Newtonsoft.Json;
using Aeon.HR.ViewModels.Args;

namespace Aeon.HR.BusinessObjects.Handlers.ExternalBO
{
	public class OvertimeBO : ExternalExcution
	{
		//: base(log, uow, "overtimeInfo.json") { }
		private IMasterDataB0 _masterData;
		private ILogger _log;
		public OvertimeBO(ILogger log, IUnitOfWork uow, OvertimeApplication overtime, ITrackingBO trackingBO, IMasterDataB0 masterbo) : base(log, uow, "overtimeInfo.json", overtime, trackingBO)
		{
			_masterData = masterbo;
			_log = log;
		}
		public override void ConvertToPayload()
		{

		}

		public override Task<object> GetData(string predicate, string[] param)
		{
			throw new NotImplementedException();
		}

		public override async Task SubmitData(bool allowSentoSAP)
		{
			var model = (OvertimeApplication)_integrationEntity;

			ItemId = model.Id;
			if (model != null)
			{
				List<TrackingRequest> trackingRequests = await PrepareTrackingRequest(model);
				if (trackingRequests != null && trackingRequests.Count > 0)
				{
					foreach (var item in trackingRequests)
					{
						await base.SubmitAPIWithTracking(item, allowSentoSAP);
					}
				}
			}
			else
			{
				_log.LogInformation("ERROR Overtime Submit Payload - No Any payload");
			}
		}

		public async Task<List<TrackingRequest>> PrepareTrackingRequest(OvertimeApplication model)
		{
			List<TrackingRequest> returnValue = new List<TrackingRequest>();
			try
			{

				_log.LogInformation("Overtime Submit Payload - Start generate payload");
				if (model != null)
				{

					var items = new List<ISAPEntity>();
					var departmentCode = string.IsNullOrEmpty(model.DivisionCode) ? model.DeptCode : model.DivisionCode;
					var currentDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => x.Code == departmentCode);
					var isHQ = currentDepartment != null && currentDepartment.Any() && !currentDepartment.FirstOrDefault().IsStore;
					var overTimeItems = await _uow.GetRepository<OvertimeApplicationDetail>().FindByAsync(x => x.OvertimeApplicationId == model.Id);
					ResultDTO ShiftCodeData = await _masterData.GetMasterDataValues(new MasterDataArgs() { Name = "ShiftCode", ParentCode = string.Empty });
					ArrayResultDTO ShiftCodeArray = ShiftCodeData.Object as ArrayResultDTO;
					List<MasterExternalDataViewModel> ShiftCode_MasterData = ShiftCodeArray.Data as List<MasterExternalDataViewModel>;
					ShiftCodeDetailCollection shiftCodeCollection = new ShiftCodeDetailCollection(ShiftCode_MasterData);

					Func<OvertimeApplicationDetailViewModel, OvertimeInfo, OvertimeInfo> CreateTrackingRequestInfo = (OvertimeApplicationDetailViewModel item, OvertimeInfo data) =>
					{
						// var data = Mapper.Map<OvertimeInfo>(item);
						data.Status = "";
						var tempActualHours = 0.0;

						if (data.ActualHoursTo == "000000")
						{
							tempActualHours = (DateTime.Parse("00:00").AddDays(1) - DateTime.Parse(item.ActualHoursFrom)).TotalMinutes / 60;

						}
						else if (DateTime.Parse(item.ActualHoursTo) < DateTime.Parse(item.ActualHoursFrom))
						{
							tempActualHours = (DateTime.Parse("00:00").AddDays(1).AddHours(DateTime.Parse(item.ActualHoursTo).Hour).AddMinutes(DateTime.Parse(item.ActualHoursTo).Minute) - DateTime.Parse(item.ActualHoursFrom)).TotalMinutes / 60;
						}
						else
						{
							tempActualHours = ((DateTime.Parse(item.ActualHoursTo) - DateTime.Parse(item.ActualHoursFrom)).TotalMinutes / 60);
						}

						if (isHQ && tempActualHours > 4)
						{
							tempActualHours--;
						}

						data.ActualHours = tempActualHours.ToString();
						data.EmployeeCode = model.Type == OverTimeType.ManagerApplyForEmployee ? item.SAPCode : model.UserSAPCode;
						data.RequestFrom = model.CreatedBy;
						if (AppSettingsHelper.ApplyNewShiftExchange_Payload)
						{
							data.Reason = model.Type == OverTimeType.EmployeeSeftService ? $"{item.ReasonName} {item.DetailReason}" : $"{model.ReasonName} {model.ContentOfOtherReason}";
						}
						if (item.DateOffInLieu)
						{
							if (tempActualHours < 4)
							{
								data.DateOffInLieuL = "0";
								data.DateOffInLieuL_1_2 = "0";
							}
							else if (tempActualHours >= 4 && tempActualHours < 7)
							{
								data.DateOffInLieuL = "0";
								data.DateOffInLieuL_1_2 = "1";
							}
							else if (tempActualHours >= 7)
							{
								data.DateOffInLieuL = "1";
								data.DateOffInLieuL_1_2 = "0";
							}
						}
						else
						{
							data.DateOffInLieuL = "0";
							data.DateOffInLieuL_1_2 = "0";
						}

						// Convert ActualHoursFrom and ActualHoursFrom to SAP time format HHmmss
						data.ActualHoursFrom = item.ActualHoursFrom.Replace(":", string.Empty) + "00";
						data.ActualHoursTo = item.ActualHoursTo.Replace(":", string.Empty) + "00";
						return data;
					};


					if (overTimeItems.Count() > 0)
					{
						foreach (var otDetailsItem in overTimeItems)
						{
							try
							{
								var data = Mapper.Map<OvertimeInfo>(otDetailsItem);
								OvertimeApplicationDetailViewModel otDetails = Mapper.Map<OvertimeApplicationDetailViewModel>(otDetailsItem);
								if (otDetails.IsNoOT)
								{
									continue;
								}
								DateTime otDate = otDetails.Date.Date.ToLocalTime();
								bool isHoliday = (otDate != DateTime.MinValue && otDate.IsPublicHoliday(_uow));
								User user = null;
								if (model.Type == OverTimeType.EmployeeSeftService)
								{
									user = model.UserSAPCode.GetUserByUserSAP(_uow);
								}
								else
								{
									user = otDetails.SAPCode.GetUserByUserSAP(_uow);
								}
								bool doesWorkOnThisHoliday = false;
								TargetDateDetail targetDateDetail = user.GetActualTarget1_ByDate(_uow, otDate, true);
								if (targetDateDetail is null)
								{
									_log.LogInformation("Overtime Submit Payload - User SAPCode " + user.SAPCode + " do not has Target plan on " + otDate.ToLocalTime().ToString("dd-MM-yyyy"));
								}
								if (isHoliday)
								{
									string shiftCode = targetDateDetail.value;

                                    if (shiftCode.StartsWith("V", StringComparison.OrdinalIgnoreCase) || shiftCode.Equals("NPL", StringComparison.OrdinalIgnoreCase))
									{
										doesWorkOnThisHoliday = true;
									}
								}

								if (isHoliday && doesWorkOnThisHoliday)
								{
									//Holiday with working shift code (Start with V - V818,V819...)
									ShiftCodeDetail shiftCodeDetail = shiftCodeCollection.GetDetailsByCode(targetDateDetail.value);
									DateTime workingFrom = shiftCodeDetail.GetStartDateTime(otDate);
									DateTime workingTo = shiftCodeDetail.GetEndDateTime(otDate);
									DateTime actualHoursFrom = otDate.SetTime(otDetails.ActualHoursFrom.Replace(":", string.Empty) + "00");
									DateTime actualHoursTo = otDate.SetTime(otDetails.ActualHoursTo.Replace(":", string.Empty) + "00");

									//if (workingFrom > workingTo)
									if (workingFrom.Subtract(workingTo).TotalHours > 0)
									{
										// if start working time > end working time ==> Subtract 1 day
										workingTo = workingTo.AddDays(1);
									}
									//if (actualHoursFrom > workingTo)
									//if (actualHoursFrom.Subtract(workingTo).TotalHours > 0)
									if (actualHoursFrom.Subtract(actualHoursTo).TotalHours > 0)
									{
										// if start actualHoursFrom > end working time ==> Subtract 1 day
										actualHoursTo = actualHoursTo.AddDays(1);
									}

									//OT time not cover the working time
									//==> Add 1 OT payload for actualHoursFrom and actualHoursTo
									//==> SAP will return issue OT time conflict with working time
									//==> This is work as design
									if ((workingFrom < actualHoursFrom && actualHoursFrom < workingTo) || (workingFrom < actualHoursTo && actualHoursTo < workingTo))
									{

										otDetails.ActualHoursFrom = actualHoursFrom.ToString("HH:mm");
										otDetails.ActualHoursTo = actualHoursTo.ToString("HH:mm");
										items.Add(CreateTrackingRequestInfo(otDetails, data));
										_log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - OT Date " + otDetails.Date.ToLocalTime().ToString("dd-MM-yyyy") + " - Ref Number " + model.ReferenceNumber);
									}
									//DOFL
									else if (otDetails.DateOffInLieu)
									{
										_log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - ActualHoursFrom " + actualHoursFrom + " - ActualHoursTo " + actualHoursTo + " - workingFrom " + workingFrom + " - workingTo " + workingTo);

										//If actual start OT Time less than Start working time
										//==> Add 1 OT payload for first part with DOFL
										if (actualHoursFrom < workingFrom)
										{
											OvertimeInfo newData = data.ConvertTo<OvertimeInfo>();
											otDetails.ActualHoursFrom = actualHoursFrom.ToString("HH:mm");
                                            if (actualHoursTo < workingFrom)  // truong hop ket thuc OT trươc giờ bắt đầu ca
                                            {
                                                otDetails.ActualHoursTo = actualHoursTo.ToString("HH:mm");

                                            }
                                            else
                                            {
                                                otDetails.ActualHoursTo = workingFrom.ToString("HH:mm");

                                            }

											var payload = CreateTrackingRequestInfo(otDetails, newData);
                                            payload.DateOffInLieuL = "0";
                                            payload.DateOffInLieuL_1_2 = "0";
                                            items.Add(payload);

											_log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - OT Date " + otDetails.Date.ToLocalTime().ToString("dd-MM-yyyy") + " - Ref Number " + model.ReferenceNumber);
										}

										//If OT cover the working time
										//==> Add 1 OT payload for the working time with DOFL
										if (actualHoursFrom <= workingFrom && workingTo <= actualHoursTo)
										{
											OvertimeInfo newData = data.ConvertTo<OvertimeInfo>();
											otDetails.ActualHoursFrom = workingFrom.ToString("HH:mm");
											otDetails.ActualHoursTo = workingTo.ToString("HH:mm");
											items.Add(CreateTrackingRequestInfo(otDetails, newData));
											_log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - OT Date " + otDetails.Date.ToLocalTime().ToString("dd-MM-yyyy") + " - Ref Number " + model.ReferenceNumber);
										}

										//If Actual end OT Time greater than End working time
										//==> Add 1 OT payload for end part with DOFL
										if (workingTo < actualHoursTo)
										{
											OvertimeInfo newData = data.ConvertTo<OvertimeInfo>();
                                            if (workingTo < actualHoursFrom)  // trường hop bat dau OT sau gio ket thuc ca
                                            {
                                                otDetails.ActualHoursFrom = actualHoursFrom.ToString("HH:mm");
                                            }
                                            else
                                            {
                                                otDetails.ActualHoursFrom = workingTo.ToString("HH:mm");
                                            }
                                            otDetails.ActualHoursTo = actualHoursTo.ToString("HH:mm");

                                            var payload = CreateTrackingRequestInfo(otDetails, newData);
                                            payload.DateOffInLieuL = "0";
                                            payload.DateOffInLieuL_1_2 = "0";
                                            items.Add(payload);

                                            _log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - OT Date " + otDetails.Date.ToLocalTime().ToString("dd-MM-yyyy") + " - Ref Number " + model.ReferenceNumber);
										}
									}
									//No DOFL
									else
									{
										_log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - ActualHoursFrom " + actualHoursFrom + " - ActualHoursTo " + actualHoursTo + " - workingFrom " + workingFrom + " - workingTo " + workingTo);


										//If actual start OT Time less than Start working time
										//==> Add 1 OT payload for first part with DOFL
										if (actualHoursFrom < workingFrom)
										{
											OvertimeInfo newData = data.ConvertTo<OvertimeInfo>();
											otDetails.ActualHoursFrom = actualHoursFrom.ToString("HH:mm");

											if (actualHoursTo < workingFrom)  // truong hop ket thuc OT trươc giờ bắt đầu ca
											{
												otDetails.ActualHoursTo = actualHoursTo.ToString("HH:mm");

											}
											else
											{
												otDetails.ActualHoursTo = workingFrom.ToString("HH:mm");

											}

											items.Add(CreateTrackingRequestInfo(otDetails, newData));
											_log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - OT Date " + otDetails.Date.ToLocalTime().ToString("dd-MM-yyyy") + " - Ref Number " + model.ReferenceNumber);
										}

										//If Actual end OT Time greater than End working time
										//==> Add 1 OT payload for end part with DOFL
										if (workingTo < actualHoursTo)
										{
											OvertimeInfo newData = data.ConvertTo<OvertimeInfo>();

											if (workingTo < actualHoursFrom)  // trường hop bat dau OT sau gio ket thuc ca
											{
												otDetails.ActualHoursFrom = actualHoursFrom.ToString("HH:mm");
											}
											else
											{
												otDetails.ActualHoursFrom = workingTo.ToString("HH:mm");
											}
											otDetails.ActualHoursTo = actualHoursTo.ToString("HH:mm");
											items.Add(CreateTrackingRequestInfo(otDetails, newData));
											_log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - OT Date " + otDetails.Date.ToLocalTime().ToString("dd-MM-yyyy") + " - Ref Number " + model.ReferenceNumber);
										}



										//if (actualHoursFrom <= workingFrom || actualHoursTo >= workingTo)
										//{
										//	//If actual start OT Time less than Start working time
										//	//==> Add 1 OT payload for first part with DOFL
										//	if (actualHoursFrom < workingFrom)
										//	{
										//		OvertimeInfo newData = data.ConvertTo<OvertimeInfo>();
										//		otDetails.ActualHoursFrom = actualHoursFrom.ToString("HH:mm");
										//		otDetails.ActualHoursTo = workingFrom.ToString("HH:mm");
										//		items.Add(CreateTrackingRequestInfo(otDetails, newData));
										//		_log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - OT Date " + otDetails.Date.ToLocalTime().ToString("dd-MM-yyyy") + " - Ref Number " + model.ReferenceNumber);
										//	}

										//	//If Actual end OT Time greater than End working time
										//	//==> Add 1 OT payload for end part with DOFL
										//	if (workingTo < actualHoursTo)
										//	{
										//		OvertimeInfo newData = data.ConvertTo<OvertimeInfo>();
										//		otDetails.ActualHoursFrom = workingTo.ToString("HH:mm");
										//		otDetails.ActualHoursTo = actualHoursTo.ToString("HH:mm");
										//		items.Add(CreateTrackingRequestInfo(otDetails, newData));
										//		_log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - OT Date " + otDetails.Date.ToLocalTime().ToString("dd-MM-yyyy") + " - Ref Number " + model.ReferenceNumber);
										//	}
										//}
										//else
										//{
										//	OvertimeInfo newData = data.ConvertTo<OvertimeInfo>();
										//	otDetails.ActualHoursFrom = actualHoursFrom.ToString("HH:mm");
										//	otDetails.ActualHoursTo = actualHoursTo.ToString("HH:mm");
										//	items.Add(CreateTrackingRequestInfo(otDetails, newData));
										//} 

									}
								}
								else
								{
									//Normal working day
									//And Holiday with PHL shift code
									string tempActualHoursFrom = otDetails.ActualHoursFrom;
									string tempActualHoursTo = otDetails.ActualHoursTo;
									if (!string.IsNullOrEmpty(otDetails.CalculatedActualHoursFrom))
									{
										tempActualHoursFrom = otDetails.CalculatedActualHoursFrom;
									}
									if (!string.IsNullOrEmpty(otDetails.CalculatedActualHoursTo))
									{
										tempActualHoursTo = otDetails.CalculatedActualHoursTo;
									}
									otDetails.ActualHoursFrom = tempActualHoursFrom;
									otDetails.ActualHoursTo = tempActualHoursTo;

									items.Add(CreateTrackingRequestInfo(otDetails, data));
									_log.LogInformation("Overtime Submit Payload: Add Payload for UserSapCode " + user.SAPCode + " - OT Date " + otDetails.Date.ToLocalTime().ToString("dd-MM-yyyy") + " - Ref Number " + model.ReferenceNumber);
								}
								//await SubmitAPI();
							}
							catch (Exception ex)
							{
								_log.LogInformation("ERROR Overtime Submit Payload - Message: " + ex.Message + " - Stacktrace" + ex.StackTrace);
							}
						}
					}
					else
					{
						_log.LogInformation("ERROR Overtime Submit Payload - OT model is null");
					}

					if (items.Count > 0)
					{
						returnValue = await AddTrackingRequests(items, "Employee");

					}
					else
					{
						_log.LogInformation("ERROR Overtime Submit Payload - No Any payload");
					}
				}

			}
			catch
			{

			}
			return returnValue;
		}
	}
}