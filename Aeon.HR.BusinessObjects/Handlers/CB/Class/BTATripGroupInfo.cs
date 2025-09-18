using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.BTA;

namespace Aeon.HR.BusinessObjects.Handlers.CB
{
    public class BTATripGroupInfo
    {
        public BTATripGroupInfo()
        {
        }
        public BTATripGroupInfo(BtaDetailViewModel btaDetail, IUnitOfWork uow)
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
                    this.key = this.JobGrade + this.DepartureCode + this.ArrivalCode + this.FromDate + this.ToDate + this.MaxBudgetAmount + this.PartitionCode;
                    this.hasGroupInfo = false;
                }
            }
            catch
            {
            }
        }

        #region Private Methods
        private void InitValue(BtaDetailViewModel btaDetail, IUnitOfWork uow)
        {
            try
            {
                this.JobGrade = btaDetail.Department.JobGradeGrade;
                this.DepartureCode = btaDetail.DepartureCode;
                this.ArrivalCode = btaDetail.ArrivalCode;
                this.FromDate = btaDetail?.FromDate?.Date.GetAsDateString();
                this.ToDate = btaDetail?.ToDate?.Date.GetAsDateString();
                this.MaxBudgetAmount = btaDetail.GetMaxBudgetLimit(uow);
                this.PartitionCode = btaDetail.PartitionCode;
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
        public string PartitionCode { get; set; }
    }
}
