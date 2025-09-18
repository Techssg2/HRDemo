using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.BTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.CB.Class
{
    public class BTATripOverBudgetGroupInfo
    {
        public BTATripOverBudgetGroupInfo()
        {
        }
        public BTATripOverBudgetGroupInfo(BusinessTripOverBudgetDetailViewModel btaDetail, IUnitOfWork uow)
        {
            try
            {
                InitValue(btaDetail, uow);
                //IsOverBudget is true means this passenger had grouped
                if (btaDetail.TripGroup != 0 || btaDetail.IsOverBudget)
                {
                    this.key = btaDetail.TripGroup + string.Empty;
                    this.hasGroupInfo = true;
                }
                else
                {
                    this.key = this.JobGrade + this.DepartureCode + this.ArrivalCode + this.FromDate + this.ToDate + this.MaxBudgetAmount;
                    this.hasGroupInfo = false;
                }
            }
            catch
            {
            }
        }

        #region Private Methods
        private void InitValue(BusinessTripOverBudgetDetailViewModel btaDetail, IUnitOfWork uow)
        {
            try
            {
                this.JobGrade = btaDetail.Department.JobGradeGrade;
                this.DepartureCode = btaDetail.DepartureCode;
                this.ArrivalCode = btaDetail.ArrivalCode;
                this.FromDate = btaDetail?.FromDate?.Date.GetAsDateString();
                this.ToDate = btaDetail?.ToDate?.Date.GetAsDateString();
                this.MaxBudgetAmount = btaDetail.GetMaxBudgetOverLimit(uow);
            }
            catch
            {
            }
        }
        #endregion
        public int JobGrade { get; set; }
        public string DepartureCode { get; set; }
        public string ArrivalCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public decimal MaxBudgetAmount { get; set; }
        public string key { get; set; }
        public bool hasGroupInfo { get; set; }
    }
}
