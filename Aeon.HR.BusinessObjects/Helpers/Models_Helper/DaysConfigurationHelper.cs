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

namespace Aeon.HR.BusinessObjects
{
    #region DateTime Helper
    public class DaysConfigurationHelper
    {
        #region variables
        int _createdNewPeriodDate = -1;
        int _newSalaryPeriod = -1;
        int _salaryPeriodFrom = -1;
        int _salaryPeriodTo = -1;
        int _deadlineOfSubmittingCABApplication = -1;
        int _deadlineOfSubmittingCABHQ = -1;
        int _deadlineOfSubmittingCABStore = -1;
        #endregion

        public DaysConfigurationHelper(IUnitOfWork unitOfWork )
        {
            try
            {
                if(!(unitOfWork is null))
                {
                    this.uow = unitOfWork;
                    PrepareInfo();
                }
            }
            catch
            {
            }
        }

        #region Private Methods
        private void PrepareInfo()
        {
            try
            {
                if (!(uow is null))
                {
                    List<DaysConfiguration> daysConfigurationItems = uow.GetRepository<DaysConfiguration>(true).FindBy(x => !x.IsDeleted).ToList();
                    for (int i = 0; i < daysConfigurationItems.Count; i++)
                    {
                        DaysConfiguration currentItem = daysConfigurationItems[i];
                        switch (currentItem.Name)
                        {
                            case "DeadlineOfSubmittingCABApplication":
                                {
                                    this._deadlineOfSubmittingCABApplication = currentItem.Value;
                                    break;
                                }
                            case "DeadlineOfSubmittingCABHQ":
                                {
                                    this._deadlineOfSubmittingCABHQ = currentItem.Value;
                                    break;
                                }
                            case "DeadlineOfSubmittingCABStore":
                                {
                                    this._deadlineOfSubmittingCABStore = currentItem.Value;
                                    break;
                                }
                            case "SalaryPeriodFrom":
                                {
                                    this._salaryPeriodFrom = currentItem.Value;
                                    break;
                                }
                            case "SalaryPeriodTo":
                                {
                                    this._salaryPeriodTo = currentItem.Value;
                                    break;
                                }
                            case "NewSalaryPeriod":
                                {
                                    this._newSalaryPeriod = currentItem.Value;
                                    break;
                                }
                            case "CreatedNewPeriodDate":
                                {
                                    this._createdNewPeriodDate = currentItem.Value;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Services

        #endregion

        #region Properties
        public IUnitOfWork uow { get; set; }

        public int CreatedNewPeriodDate
        {
            get
            {
                return _createdNewPeriodDate;
            }
        }

        public int NewSalaryPeriod
        {
            get
            {
                return _newSalaryPeriod;
            }
        }

        public int SalaryPeriodFrom
        {
            get
            {
                return _salaryPeriodFrom;
            }
        }

        public int SalaryPeriodTo
        {
            get
            {
                return _salaryPeriodTo;
            }
        }

        public int DeadlineOfSubmittingCABApplication
        {
            get
            {
                return _deadlineOfSubmittingCABApplication;
            }
        }

        public int DeadlineOfSubmittingCABHQ
        {
            get
            {
                return _deadlineOfSubmittingCABHQ;
            }
        }

        public int DeadlineOfSubmittingCABStore
        {
            get
            {
                return _deadlineOfSubmittingCABStore;
            }
        }
        #endregion
    }
    #endregion
}
