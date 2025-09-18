using Aeon.HR.BusinessObjects.DataHandlers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.Data;
using Newtonsoft.Json;
using System.Reflection;
using Aeon.HR.ViewModels;
using Aeon.HR.Infrastructure.Enums;
using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.BTA;
using AutoMapper;
using System.Security.Cryptography;
using Aeon.HR.ViewModels.CustomSection;
using System.Configuration;
using System.Text;

namespace Aeon.HR.BusinessObjects
{
	#region BTAHelper
	public static class BTAHelper
	{
		public static BTAViewModel GetAsBtaViewModel(this Guid btaId, IUnitOfWork uow)
		{
			BTAViewModel returnValue = null;
			try
			{
				returnValue = uow.GetRepository<BusinessTripApplication>().GetSingle<BTAViewModel>(x => x.Id == btaId);
			}
			catch
			{
			}
			return returnValue;
		}

        public static double? GetBTAApprovedDay(Guid businessTripApplicationId, IUnitOfWork uow)
        {
            try
            {
                var bta = uow.GetRepository<BusinessTripApplication>().FindById(businessTripApplicationId);
                if (bta != null)
                {
                    var btadetails = uow.GetRepository<BusinessTripApplicationDetail>().GetSingle(x => x.BusinessTripApplicationId == businessTripApplicationId, "FromDate asc");
                    if (btadetails != null)
                    {
                        var wfTasks = uow.GetRepository<WorkflowTask>(true).GetSingle(x => x.ItemId == businessTripApplicationId
                                                                                 && x.Status.ToLower().Equals("waiting for booking flight"), "created desc");
                        if (wfTasks != null)
                        {
                            DateTimeOffset FromConverted = btadetails.FromDate.Value.ToOffset(wfTasks.Created.Offset);
                            double? countDay = ((TimeSpan)(FromConverted.Date - wfTasks.Created.Date)).Days;
                            return (countDay > 0 ? countDay : null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        public static BTAOverBudgetViewModel GetAsBtaOverBudgetViewModel(this Guid btaId, IUnitOfWork uow)
		{
			BTAOverBudgetViewModel returnValue = null;
			try
			{
				returnValue = uow.GetRepository<BusinessTripOverBudget>().GetSingle<BTAOverBudgetViewModel>(x => x.Id == btaId);
			}
			catch
			{
			}
			return returnValue;
		}
		public static List<BusinessTripOverBudgetDetailViewModel> GetAsBtaOverBudgetDetailsViewModel(this BTAOverBudgetViewModel btaItem, IUnitOfWork uow)
		{
			List<BusinessTripOverBudgetDetailViewModel> returnValue = new List<BusinessTripOverBudgetDetailViewModel>();
			try
			{
				returnValue = uow.GetRepository<BusinessTripOverBudgetsDetail>().FindBy<BusinessTripOverBudgetDetailViewModel>(i => i.BusinessTripOverBudgetId == btaItem.Id).ToList();

			}
			catch
			{
				returnValue = new List<BusinessTripOverBudgetDetailViewModel>();
			}
			return returnValue;
		}
		public static decimal GetMaxBudgetOverLimit(this BusinessTripOverBudgetDetailViewModel btaDetail, IUnitOfWork uow)
		{
			decimal returnValue = 0;
			try
			{
				if (btaDetail != null && btaDetail != null)
				{
					returnValue = btaDetail.User.GetMaxBudgetLimit(btaDetail.PartitionId, uow);
				}
			}
			catch
			{
			}
			return returnValue;
		}
		public static List<BtaDetailViewModel> GetAsBtaDetailsViewModel(this BTAViewModel btaItem, IUnitOfWork uow)
		{
			List<BtaDetailViewModel> returnValue = new List<BtaDetailViewModel>();
			try
			{
				returnValue = uow.GetRepository<BusinessTripApplicationDetail>().FindBy<BtaDetailViewModel>(i => i.BusinessTripApplicationId == btaItem.Id).ToList();

			}
			catch
			{
				returnValue = new List<BtaDetailViewModel>();
			}
			return returnValue;
		}




		public static decimal GetMaxBudgetLimit(this BtaDetailViewModel btaDetail, IUnitOfWork uow)
		{
			decimal returnValue = 0;
			try
			{
				if (btaDetail != null && btaDetail != null)
				{
					returnValue = btaDetail.User.GetMaxBudgetLimit(btaDetail.PartitionId, uow);
				}
			}
			catch
			{
			}
			return returnValue;
		}
	}
	#endregion

	#region BTADetailsHelper
	public static class BTADetailsHelper
	{
		public static List<FlightDetailViewModel> GetFlightDetails(this BtaDetailViewModel btaDetailViewModel, IUnitOfWork uow)
		{
			List<FlightDetailViewModel> returnValue = null;
			try
			{
				if (btaDetailViewModel != null && uow != null)
				{
					returnValue = uow.GetRepository<FlightDetail>().FindBy<FlightDetailViewModel>(x => x.BusinessTripApplicationDetailId == btaDetailViewModel.Id).ToList();
				}
			}
			catch (Exception ex)
			{
			}
			return returnValue;
		}
		public static List<FlightDetailViewModel> GetFlightOverBudgetDetails(this BusinessTripOverBudgetDetailViewModel btaDetailViewModel, IUnitOfWork uow)
		{
			List<FlightDetailViewModel> returnValue = null;
			try
			{
				if (btaDetailViewModel != null && uow != null)
				{
					returnValue = uow.GetRepository<FlightDetail>().FindBy<FlightDetailViewModel>(x => x.BusinessTripApplicationDetailId == btaDetailViewModel.Id).ToList();
				}
			}
			catch (Exception ex)
			{
			}
			return returnValue;
		}
		public static bool SavePassengerInformation(this BusinessTripApplicationDetail btaDetail, IUnitOfWork uow)
		{
			bool returnValue = false;
			try
			{
				if (btaDetail != null && uow != null)
				{
					PassengerInformation passengerInformation = btaDetail.GetPassengerInformation(uow);
					if (passengerInformation is null)
					{
						passengerInformation = new PassengerInformation();
					}
					passengerInformation = btaDetail.TransformValues(passengerInformation, new List<string> { "Id" });
					if (passengerInformation.Id == Guid.Empty)
					{
						uow.GetRepository<PassengerInformation>().Add(passengerInformation);
					}
					else
					{
						uow.GetRepository<PassengerInformation>().Update(passengerInformation);
					}
					uow.Commit();
					returnValue = true;
				}
			}
			catch
			{
			}
			return returnValue;
		}
		public static PassengerInformation GetPassengerInformation(this BusinessTripApplicationDetail btaDetail, IUnitOfWork uow)
		{
			PassengerInformation returnValue = null;
			try
			{
				if (btaDetail != null && uow != null)
				{
					returnValue = uow.GetRepository<PassengerInformation>().GetSingle(x => x.SAPCode == btaDetail.SAPCode);
				}
			}
			catch
			{
			}
			return returnValue;
		}

	}
	#endregion

	#region BTAPassengerHelper
	public static class BTAPassengerHelper
	{
		public static bool Update(this BTAPassengerViewModel btaPassengerInfo, IUnitOfWork uow)
		{
			bool returnValue = false;
			try
			{
				if (btaPassengerInfo != null && uow != null)
				{
					#region Update BusinessTripApplicationDetail
					BusinessTripApplicationDetail btaDetailItem = uow.GetRepository<BusinessTripApplicationDetail>(true).FindById(btaPassengerInfo.Id);
					if (btaDetailItem != null)
					{
						btaDetailItem.TripGroup = btaPassengerInfo.TripGroup;
						btaDetailItem.IsOverBudget = btaPassengerInfo.IsOverBudget;
						btaDetailItem.MaxBudgetAmount = btaPassengerInfo.MaxBudgetAmount;
						btaDetailItem.Comments = btaPassengerInfo.Comments;
						uow.GetRepository<BusinessTripApplicationDetail>().Update(btaDetailItem);

						if (btaDetailItem.IsOverBudget)
						{
							btaDetailItem.BusinessTripApplication.IsOverBudget = true;
							uow.GetRepository<BusinessTripApplication>().Update(btaDetailItem.BusinessTripApplication);
						}
					}
					#endregion

					#region Update FlightInfo
					List<FlightDetail> flightDetailItems = uow.GetRepository<FlightDetail>(true).FindBy(x => x.BusinessTripApplicationDetailId == btaDetailItem.Id).ToList();
					if (btaPassengerInfo.FlightDetails != null)
					{

						List<FlightDetailViewModel> flightDetailInfos = btaPassengerInfo.FlightDetails;
						List<Guid> deleteFlightDetailIds = flightDetailItems.Select(x => x.Id).ToList().Except(flightDetailInfos.Where(x => x.Id != Guid.Empty).Select(x => x.Id).ToList()).ToList();

						flightDetailInfos.Select(x => x.SaveAsFlightDetail(btaDetailItem.Id, uow)).ToList();
						flightDetailItems.Where(x => x != null && deleteFlightDetailIds.Contains(x.Id)).Select(x => x.DeleteFlightDetail(uow)).ToList();
					}
					else
					{
						if (flightDetailItems != null && flightDetailItems.Count > 0)
						{
							uow.GetRepository<FlightDetail>().Delete(flightDetailItems);
						}
					}
					#endregion
					uow.Commit();
					returnValue = true;
				}
			}
			catch (Exception ex)
			{
			}
			return returnValue;
		}
	}
	#endregion

	#region BTADetailsHelper
	public static class FlightDetailHelper
	{
		public static bool SaveAsFlightDetail(this FlightDetailViewModel flightDetailViewModel, Guid btaDetailId, IUnitOfWork uow)
		{
			bool returnValue = false;
			try
			{
				if (flightDetailViewModel.Id == Guid.Empty)
				{
					FlightDetail flightDetailItem = new FlightDetail();
					flightDetailItem = Mapper.Map<FlightDetail>(flightDetailViewModel);
					flightDetailItem.BusinessTripApplicationDetailId = btaDetailId;
					uow.GetRepository<FlightDetail>().Add(flightDetailItem);
				}
				else
				{
					FlightDetail flightDetailItem = uow.GetRepository<FlightDetail>().GetSingle(x => x.Id == flightDetailViewModel.Id);
					flightDetailItem = Mapper.Map<FlightDetail>(flightDetailViewModel);
					flightDetailItem.BusinessTripApplicationDetailId = btaDetailId;
					uow.GetRepository<FlightDetail>().Update(flightDetailItem);
				}

				uow.Commit();
				returnValue = true;
			}
			catch (Exception ex)
			{
				returnValue = false;
			}
			return returnValue;
		}

		public static bool DeleteFlightDetail(this FlightDetail flightDetailViewModel, IUnitOfWork uow)
		{
			bool returnValue = false;
			try
			{
				if (flightDetailViewModel != null && uow != null)
				{
					FlightDetail flightDetailItem = uow.GetRepository<FlightDetail>().GetSingle(x => x.Id == flightDetailViewModel.Id);
					if (flightDetailItem != null)
					{
						uow.GetRepository<FlightDetail>().Delete(flightDetailItem);
					}
					uow.Commit();
					returnValue = true;
				}
			}
			catch
			{
				returnValue = false;
			}
			return returnValue;
		}
	}
	#endregion

	#region BTABookingFlightHelper
	public static class BTABookingFlightHelper
	{
		public static async Task<bool> Update(this BookingFlightViewModel btaBookingInfo, IUnitOfWork uow, ILogger logger)
		{
			bool returnValue = false;
			try
			{
				logger.LogInformation("BookingId: " + btaBookingInfo.Id);
				//if (btaBookingInfo.Id == Guid.Empty)
				//{
				//Get Flight Details
				FlightDetail flightDetail = await uow.GetRepository<FlightDetail>().GetSingleAsync(x =>
				x.GroupId == btaBookingInfo.GroupId &&
				x.DirectFlight == btaBookingInfo.DirectFlight &&
				x.BusinessTripApplicationDetailId == btaBookingInfo.BTADetailId);
				if (flightDetail != null)
				{
					BookingFlight bookingFlightItem = new BookingFlight();
					bookingFlightItem = Mapper.Map<BookingFlight>(btaBookingInfo);
					bookingFlightItem.FlightDetailId = flightDetail.Id;

					logger.LogInformation("Before Update Booking Flight");
					uow.GetRepository<BookingFlight>().Add(bookingFlightItem);
					logger.LogInformation("After Update Booking Flight");
					//Update data Flight Details
					BusinessTripApplicationDetail btaDetails = await uow.GetRepository<BusinessTripApplicationDetail>().GetSingleAsync(x => x.Id == flightDetail.BusinessTripApplicationDetailId);
					if (null != btaDetails)
					{
						if (btaBookingInfo.DirectFlight)
						{
							btaDetails.FlightNumberCode = flightDetail.FlightNo;
							btaDetails.FlightNumberName = flightDetail.AirlineName + " " + flightDetail.OriginLocationCode + " - " + flightDetail.DestinationLocationCode + " " + flightDetail.DepartureDateTime.ToString("HH:mm") + " - " + flightDetail.ArrivalDateTime.ToString("HH:mm");
						}
						else
						{
							btaDetails.ComebackFlightNumberCode = flightDetail.FlightNo;
							btaDetails.ComebackFlightNumberName = flightDetail.AirlineName + " " + flightDetail.OriginLocationCode + " - " + flightDetail.DestinationLocationCode + " " + flightDetail.DepartureDateTime.ToString("HH:mm") + " - " + flightDetail.ArrivalDateTime.ToString("HH:mm");
						}
						uow.GetRepository<BusinessTripApplicationDetail>().Update(btaDetails);
					}

				}
				await uow.CommitAsync();
				returnValue = true;
			}
			catch (Exception ex)
			{
				logger.LogError("Update Booking Flight: " + ex.Message + " Stact Trace:" + ex.StackTrace);
			}
			return returnValue;
		}
	}
	#endregion

	#region CommitBookingHelper

	public static class CommitBookingHelper
	{
		public static CommitBooking GenerateKey(this CommitBooking commitBooking)
		{
			try
			{
				GotadiSettingsSection GOTADI_CONFIG = (GotadiSettingsSection)ConfigurationManager.GetSection("airTicketSettings");
				RSACryptoServiceProvider rsa = RSAHelper.GetCryptoServiceByPublicKey(Convert.FromBase64String(RSAHelper.GetPublicKey(GOTADI_CONFIG.Certificates.Gotadi_Public_Key)));
				commitBooking.key = rsa.Encrypt(commitBooking.secret_key, false).GetAsString();
			}
			catch
			{

			}
			return commitBooking;
		}
		public static string GenerateOriginalData(this CommitBooking commitBooking)
		{
			try
			{
				GotadiSettingsSection GOTADI_CONFIG = (GotadiSettingsSection)ConfigurationManager.GetSection("airTicketSettings");
				commitBooking.access_code = GOTADI_CONFIG.Header.AccessCode;
				RSACryptoServiceProvider rsa = RSAHelper.GetCryptoServiceByPrivateKey(Convert.FromBase64String(RSAHelper.GetPrivateKey(GOTADI_CONFIG.Certificates.AEON_BTA_Private_Key)));
				string signatureString = rsa.SignData(Encoding.ASCII.GetBytes(commitBooking.signatureData), CryptoConfig.MapNameToOID("SHA256")).GetAsString();
				return $"{commitBooking.signatureData}|{signatureString}";
			}
			catch
			{

			}
			return string.Empty;
		}
		public static CommitBooking GenerateData(this CommitBooking commitBooking)
		{
			try
			{
				string originalData = commitBooking.GenerateOriginalData();
				commitBooking.data = originalData.EncryptBySecretkey(commitBooking.secret_key);
			}
			catch
			{

			}
			return commitBooking;
		}
    }
	#endregion
}
