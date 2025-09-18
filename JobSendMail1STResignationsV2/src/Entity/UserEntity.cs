using JobSendMail1STResignationsV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignationsV2.src.Entity
{
    [TableAttribute(TableName = "Users")]
    public class UserEntity : BaseEntity
    {
        [Field(FieldName = "Id", Type = SqlDbType.UniqueIdentifier)]
        public Guid Id { get; set; }

        [Field(FieldName = "FullName", Type = SqlDbType.NVarChar)]
        public string FullName { get; set; }

        [Field(FieldName = "Email", Type = SqlDbType.NVarChar)]
        public string Email { get; set; }

        [Field(FieldName = "LoginName", Type = SqlDbType.NVarChar)]
        public string LoginName { get; set; }

        [Field(FieldName = "SAPCode", Type = SqlDbType.NVarChar)]
        public string SAPCode { get; set; }

        [Field(FieldName = "Type", Type = SqlDbType.Int)]
        public int Type { get; set; }

        [Field(FieldName = "Role", Type = SqlDbType.Int)]
        public int Role { get; set; }

        [Field(FieldName = "IsActivated", Type = SqlDbType.Bit)]
        public bool IsActivated { get; set; }

        [Field(FieldName = "IsDeleted", Type = SqlDbType.Bit)]
        public bool IsDeleted { get; set; }

        [Field(FieldName = "ProfilePictureId", Type = SqlDbType.UniqueIdentifier)]
        public Guid? ProfilePictureId { get; set; }

        [Field(FieldName = "StartDate", Type = SqlDbType.DateTime)]
        public DateTime? StartDate { get; set; }

        [Field(FieldName = "QuotaDataJson", Type = SqlDbType.NVarChar)]
        public string QuotaDataJson { get; set; }

        [Field(FieldName = "RedundantPRD", Type = SqlDbType.NVarChar)]
        public string RedundantPRD { get; set; }

        [Field(FieldName = "Gender", Type = SqlDbType.Int)]
        public int Gender { get; set; }

        [Field(FieldName = "CheckAuthorizationUSB", Type = SqlDbType.Bit)]
        public bool CheckAuthorizationUSB { get; set; }

        [Field(FieldName = "IsTargetPlan", Type = SqlDbType.Bit)]
        public bool IsTargetPlan { get; set; }

        [Field(FieldName = "IsNotTargetPlan", Type = SqlDbType.Bit)]
        public bool IsNotTargetPlan { get; set; }

        [Field(FieldName = "HasTrackingLog", Type = SqlDbType.Bit)]
        public bool HasTrackingLog { get; set; }

        [Field(FieldName = "IsFromIT", Type = SqlDbType.Bit)]
        public bool IsFromIT { get; set; }

        [Field(FieldName = "Created", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset Created { get; set; }

        [Field(FieldName = "CreatedBy", Type = SqlDbType.NVarChar)]
        public string CreatedBy { get; set; }

        [Field(FieldName = "CreatedById", Type = SqlDbType.UniqueIdentifier)]
        public Guid? CreatedById { get; set; }

        [Field(FieldName = "Modified", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset Modified { get; set; }

        [Field(FieldName = "ModifiedBy", Type = SqlDbType.NVarChar)]
        public string ModifiedBy { get; set; }

        [Field(FieldName = "ModifiedById", Type = SqlDbType.UniqueIdentifier)]
        public Guid? ModifiedById { get; set; }

        [Field(FieldName = "CreatedByFullName", Type = SqlDbType.NVarChar)]
        public string CreatedByFullName { get; set; }

        [Field(FieldName = "ModifiedByFullName", Type = SqlDbType.NVarChar)]
        public string ModifiedByFullName { get; set; }

        [Field(FieldName = "AppService", Type = SqlDbType.NVarChar)]
        public string AppService { get; set; }

        public override void Fill(SqlDataReader sqlDataReader)
        {
            base.Fill(sqlDataReader);
            this.Id = this.GetData<Guid>(sqlDataReader, "Id");
            this.FullName = this.GetData<string>(sqlDataReader, "FullName");
            this.Email = this.GetData<string>(sqlDataReader, "Email");
            this.LoginName = this.GetData<string>(sqlDataReader, "LoginName");
            this.SAPCode = this.GetData<string>(sqlDataReader, "SAPCode");
            this.Type = this.GetData<int>(sqlDataReader, "Type");
            this.IsActivated = this.GetData<bool>(sqlDataReader, "IsActivated");
            this.Created = this.GetData<DateTimeOffset>(sqlDataReader, "Created");
            this.CreatedBy = this.GetData<string>(sqlDataReader, "CreatedBy");
            this.Modified = this.GetData<DateTimeOffset>(sqlDataReader, "Modified");
            this.ModifiedBy = this.GetData<string>(sqlDataReader, "ModifiedBy");
            this.Role = this.GetData<int>(sqlDataReader, "Role");
            this.IsDeleted = this.GetData<bool>(sqlDataReader, "IsDeleted");
            this.ProfilePictureId = this.GetData<Guid?>(sqlDataReader, "ProfilePictureId");
            this.StartDate = this.GetData<DateTime?>(sqlDataReader, "StartDate");
            this.QuotaDataJson = this.GetData<string>(sqlDataReader, "QuotaDataJson");
            this.RedundantPRD = this.GetData<string>(sqlDataReader, "RedundantPRD");
            this.Gender = this.GetData<int>(sqlDataReader, "Gender");
            this.CheckAuthorizationUSB = this.GetData<bool>(sqlDataReader, "CheckAuthorizationUSB");
            this.IsTargetPlan = this.GetData<bool>(sqlDataReader, "IsTargetPlan");
            this.IsNotTargetPlan = this.GetData<bool>(sqlDataReader, "IsNotTargetPlan");
            this.HasTrackingLog = this.GetData<bool>(sqlDataReader, "HasTrackingLog");
            this.IsFromIT = this.GetData<bool>(sqlDataReader, "IsFromIT");
            this.CreatedById = this.GetData<Guid?>(sqlDataReader, "CreatedById");
            this.ModifiedById = this.GetData<Guid?>(sqlDataReader, "ModifiedById");
            this.CreatedByFullName = this.GetData<string>(sqlDataReader, "CreatedByFullName");
            this.ModifiedByFullName = this.GetData<string>(sqlDataReader, "ModifiedByFullName");
            this.AppService = this.GetData<string>(sqlDataReader, "AppService");
        }

        public override void FillOut(SqlCommand sqlCommand)
        {
            base.FillOut(sqlCommand);
        }
    }
}