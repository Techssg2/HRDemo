using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using JobResignationV2.src.SQLExcute;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResignationV2.src.ModelEntity
{
    [JobResignationV2.src.SQLExcute.TableAttribute(TableName = "Users")]
    public class UserEntity : BaseEntity
    {
        [Field(FieldName = "FullName", Type = SqlDbType.NVarChar)]
        public string FullName { get; set; }
        public string LoginName { get; set; }
        public string SAPCode { get; set; }
        public string Email { get; set; }
        public Guid? ProfilePictureId { get; set; }
        public bool IsActivated { get; set; }
        public LoginType Type { get; set; }
        public UserRole Role { get; set; }
        public Gender Gender { get; set; }
        public bool CheckAuthorizationUSB { get; set; }
        public bool IsTargetPlan { get; set; }
        public bool IsNotTargetPlan { get; set; }
        public DateTime? StartDate { get; set; }
        public string QuotaDataJson { get; set; }
        public string RedundantPRD { get; set; } // PRD còn dư của tháng trước tháng hiện tại
        public bool? HasTrackingLog { get; set; }
        public bool IsFromIT { get; set; }
        public int CountLogin { get; set; }
        public override void Fill(SqlDataReader sqlDataReader)
        {
            base.Fill(sqlDataReader);
            this.FullName = this.GetData<string>(sqlDataReader, "FullName");
            this.SAPCode = this.GetData<string>(sqlDataReader, "SAPCode");
            this.QuotaDataJson = this.GetData<string>(sqlDataReader, "QuotaDataJson");
            this.IsActivated = this.GetData<bool>(sqlDataReader, "IsActivated");
            this.StartDate = this.GetData<DateTime>(sqlDataReader, "StartDate");
        }
        public override void FillOut(SqlCommand sqlCommand)
        {
            base.FillOut(sqlCommand);
        }
    }
}
