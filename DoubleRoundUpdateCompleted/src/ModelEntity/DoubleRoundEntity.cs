using OvertimeUpdateCompleted.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvertimeUpdateCompleted.src.ModelEntity
{
    [TableAttribute(TableName = "OvertimeApplications")]
    public class DoubleRoundEntity : BaseEntity
    {
        [Field(FieldName = "ReferenceNumber", Type = SqlDbType.NVarChar)]
        public string ReferenceNumber { get; set; }
        [Field(FieldName = "Status", Type = SqlDbType.NVarChar)]
        public string Status { get; set; }

        [FieldAttribute(FieldName = "InstanceId", ExcludeUpdate = true, Type = SqlDbType.UniqueIdentifier)]
        public Guid InstanceId { get; set; }
        public override void Fill(SqlDataReader sqlDataReader)
        {
            base.Fill(sqlDataReader);
            this.ReferenceNumber = this.GetData<string>(sqlDataReader, "ReferenceNumber");
            this.Status = this.GetData<string>(sqlDataReader, "Status");
            this.InstanceId = this.GetData<Guid>(sqlDataReader, "InstanceId");
        }
        public override void FillOut(SqlCommand sqlCommand)
        {
            base.FillOut(sqlCommand);
        }
    }
}
