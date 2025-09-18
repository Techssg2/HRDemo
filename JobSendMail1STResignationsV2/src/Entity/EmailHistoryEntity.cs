using JobSendMail1STResignationsV2.src.Enums;
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
    [TableAttribute(TableName = "EmailHistory")]
    public class EmailHistoryEntity : BaseEntity
    {
        [FieldAttribute(FieldName = "ItemId", Type = SqlDbType.UniqueIdentifier)]
        public Guid ItemId { get; set; }

        [FieldAttribute(FieldName = "ReferenceNumber", Type = SqlDbType.NVarChar)]
        public string ReferenceNumber { get; set; }

        [FieldAttribute(FieldName = "ItemType", Type = SqlDbType.NVarChar)]
        public string ItemType { get; set; }

        [FieldAttribute(FieldName = "UserSent", Type = SqlDbType.NVarChar)]
        public string UserSent { get; set; }  /*UserInfo*/

        [FieldAttribute(FieldName = "DepartmentId", Type = SqlDbType.UniqueIdentifier)]
        public Guid? DepartmentId { get; set; }

        [FieldAttribute(FieldName = "DepartmentType", Type = SqlDbType.NVarChar)]
        public Group DepartmentType { get; set; }

        [FieldAttribute(FieldName = "IsSent", Type = SqlDbType.Bit)]
        public bool IsSent { get; set; }

        [FieldAttribute(FieldName = "Created", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset Created { get; set; }

        [FieldAttribute(FieldName = "CreatedBy", Type = SqlDbType.NVarChar)]
        public string CreatedBy { get; set; }

        [FieldAttribute(FieldName = "Modified", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset Modified { get; set; }

        [FieldAttribute(FieldName = "ModifiedBy", Type = SqlDbType.NVarChar)]
        public string ModifiedBy { get; set; }

        public override void Fill(SqlDataReader sqlDataReader)
        {
            base.Fill(sqlDataReader);
            this.ItemId = this.GetData<Guid>(sqlDataReader, "ItemId");
            this.ReferenceNumber = this.GetData<string>(sqlDataReader, "ReferenceNumber");
            this.ItemType = this.GetData<string>(sqlDataReader, "ItemType");
            this.UserSent = this.GetData<string>(sqlDataReader, "UserSent");
            this.DepartmentId = this.GetData<Guid?>(sqlDataReader, "DepartmentId");
            this.DepartmentType = this.GetData<Group>(sqlDataReader, "DepartmentType");
            this.IsSent = this.GetData<bool>(sqlDataReader, "IsSent");
            this.Created = this.GetData<DateTimeOffset>(sqlDataReader, "Created");
            this.CreatedBy = this.GetData<string>(sqlDataReader, "CreatedBy");
            this.Modified = this.GetData<DateTimeOffset>(sqlDataReader, "Modified");
            this.ModifiedBy = this.GetData<string>(sqlDataReader, "ModifiedBy");
        }

        public class UserInfo
        {
            public Guid UserId { get; set; }
            public string FullName { get; set; }
        }
    }
}