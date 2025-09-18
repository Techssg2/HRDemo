using CreatePayloadCompleted.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatePayloadCompleted.src.ModelEntity
{
    [TableAttribute(TableName = "OvertimeApplications")]
    public class CreatePayloadCompletedEntity : BaseEntity
    {
        [Field(FieldName = "ReferenceNumber", Type = SqlDbType.NVarChar)]
        public string ReferenceNumber { get; set; }
        [Field(FieldName = "Status", Type = SqlDbType.NVarChar)]
        public string Status { get; set; }
        public override void Fill(SqlDataReader sqlDataReader)
        {
            base.Fill(sqlDataReader);
            this.ReferenceNumber = this.GetData<string>(sqlDataReader, "ReferenceNumber");
            this.Status = this.GetData<string>(sqlDataReader, "Status");
        }
        public override void FillOut(SqlCommand sqlCommand)
        {
            base.FillOut(sqlCommand);
        }
    }

    public class CreatePayloadArgs
    {
        public List<string> ReferenceNumbers { get; set; }
    }
}
