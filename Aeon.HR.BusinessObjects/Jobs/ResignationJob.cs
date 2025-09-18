using Aeon.HR.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System.Transactions;
using System.Web.Security;
using Aeon.HR.BusinessObjects.Interfaces;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class ResignationJob
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly ISettingBO _settingUser;
        public ResignationJob(ILogger logger, IUnitOfWork unitOfWork, ISettingBO settingUser)
        {
            _logger = logger;
            _uow = unitOfWork;
            _settingUser = settingUser;
        }
        public async Task<bool> InactiveUserOnResignationDate()
        {
            // Inactive USER
            // Remove 
            //var datetimeNow = DateTime.Now.Date;
            //var last3Days = DateTime.Now.AddDays(-3).Date;
            /*var completedResignations = _uow.GetRepository<ResignationApplication>(true).FindBy<ResignationApplicationViewModel>
            (x => x.Status == "Completed" && x.SuggestionForLastWorkingDay.HasValue && x.OfficialResignationDate <= datetimeNow
            && x.OfficialResignationDate >= last3Days).ToList();*/

            //fix - 487
            var rangeToDate = DateTime.Now.AddDays(-13).Date;
            var rangeFromDate = DateTime.Now.AddDays(-15).Date;

            /*var completedResignations = _uow.GetRepository<ResignationApplication>(true).FindBy<ResignationApplicationViewModel>
              (x => x.Status == "Completed" && x.SuggestionForLastWorkingDay.HasValue && x.OfficialResignationDate <= rangeToDate
              && x.OfficialResignationDate >= rangeFromDate).ToList();*/

            var completedResignations = _uow.GetRepository<ResignationApplication>(true).FindBy<ResignationApplicationViewModel>
              (x => x.Status == "Completed"
              && x.OfficialResignationDate <= rangeToDate).ToList();
            if (completedResignations.Any())
            {
                try
                {
                   /* using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {*/
                        foreach (var item in completedResignations)
                        {
                            try
                            {
                                if (item != null && !string.IsNullOrEmpty(item.UserSAPCode))
                                {
                                    //_logger.LogInformation($"Process inactive User SAP code: " + item.UserSAPCode);
                                    if (!string.IsNullOrEmpty(item.UserSAPCode) && DateTime.Compare(item.OfficialResignationDate.AddDays(14).ToLocalTime().DateTime.Date, DateTime.Now.Date) <= 0)
                                    {
                                        var currentUser = _uow.GetRepository<User>(true).GetSingle(x => x.SAPCode == item.UserSAPCode && x.IsActivated);
                                        if (currentUser != null && currentUser.StartDate.Value.Date < item.OfficialResignationDate.ToLocalTime().DateTime.Date)
                                        {
                                            var mappingUserDepartments = _uow.GetRepository<UserDepartmentMapping>(true).ITFindBy(x => x.UserId == currentUser.Id).ToList();
                                            if (mappingUserDepartments.Any())
                                            {
                                                _uow.GetRepository<UserDepartmentMapping>().Delete(mappingUserDepartments);
                                                //_logger.LogInformation($"Removed UserDepartmentMapping");
                                            }
                                            if (currentUser.IsActivated)
                                            {
                                                currentUser.IsActivated = false;
                                                //
                                                #region Rem code lock membership account
                                                //var isSuccess = false;
                                                //var isSuccess = await _settingUser.ChangeStatus(currentUser.Id, false);

                                                //if (currentUser != null)
                                                //{
                                                //    _logger.LogInformation($"Start get Membership account " + currentUser.LoginName);
                                                //    var user = Membership.GetUser(currentUser.LoginName);
                                                //    _logger.LogInformation($"Get Membership account complete");
                                                //    if (user != null)
                                                //    {
                                                //        try
                                                //        {
                                                //            user.IsApproved = false;
                                                //            _logger.LogInformation($"Start to update Membership account " + currentUser.LoginName);
                                                //            Membership.UpdateUser(user);
                                                //            _logger.LogInformation($"pdate Membership account complete");

                                                //            isSuccess = true;
                                                //        }
                                                //        catch (Exception ex)
                                                //        {
                                                //            _logger.LogError($"Error at Inactive Membership " + item.UserSAPCode + ". " + ex.Message + ". " + ex.StackTrace);
                                                //        }
                                                //    }
                                                //}

                                                //if (isSuccess)
                                                //{
                                                //    _logger.LogInformation($"Changed status to inactive");
                                                //}
                                                #endregion
                                            }
                                            try
                                            {
                                                _uow.Commit();
                                                //_logger.LogInformation($"Inactive user " + currentUser.SAPCode + " successfully.");
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.LogError($"Cannot commit" + ex.Message + ex.StackTrace);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.LogError($"Error at Inactive User null or SAPCode empty " + item.Id);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error at Inactive User " + item.UserSAPCode + ex.Message + ". " + ex.StackTrace);
                            }
                        }
                        //_uow.CommitAsync().GetAwaiter().GetResult();
                      /*  transactionScope.Complete();
                    }*/
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error at InactiveUserOnResignationDate: " + ex.Message + ". " + ex.StackTrace);
                    return false;
                }
            }
            return true;
        }
    }
}
