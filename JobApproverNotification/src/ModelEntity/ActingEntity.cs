using JobApproverNotification.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApproverNotification.src.ModelEntity
{
    [TableAttribute(TableName = "Actings")]
    public class ActingEntity : BaseEntity
    {
        [FieldAttribute(FieldName = "ReferenceNumber", Type = SqlDbType.NVarChar)]
        public string ReferenceNumber { get; set; }

        [FieldAttribute(FieldName = "Status", Type = SqlDbType.NVarChar)]
        public string Status { get; set; }

        [FieldAttribute(FieldName = "Period1To", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset? Period1To { get; set; }

        [FieldAttribute(FieldName = "Period2To", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset? Period2To { get; set; }

        [FieldAttribute(FieldName = "Period3To", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset? Period3To { get; set; }

        [FieldAttribute(FieldName = "Period4To", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset? Period4To { get; set; }

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
            this.ReferenceNumber = this.GetData<string>(sqlDataReader, "ReferenceNumber");
            this.Status = this.GetData<string>(sqlDataReader, "Status");
            this.Period1To = this.GetData<DateTimeOffset?>(sqlDataReader, "Period1To");
            this.Period2To = this.GetData<DateTimeOffset?>(sqlDataReader, "Period2To");
            this.Period3To = this.GetData<DateTimeOffset?>(sqlDataReader, "Period3To");
            this.Period4To = this.GetData<DateTimeOffset?>(sqlDataReader, "Period4To");
            this.Created = this.GetData<DateTimeOffset>(sqlDataReader, "Created");
            this.CreatedBy = this.GetData<string>(sqlDataReader, "CreatedBy");
            this.Modified = this.GetData<DateTimeOffset>(sqlDataReader, "Modified");
            this.ModifiedBy = this.GetData<string>(sqlDataReader, "ModifiedBy");
        }
    }
}
