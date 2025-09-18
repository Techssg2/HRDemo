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
    [TableAttribute(TableName = "ITUsers")]
    public class ITUserEntity : BaseEntity
    {
        [Field(FieldName = "Id", Type = SqlDbType.UniqueIdentifier)]
        public Guid Id { get; set; }

        [Field(FieldName = "EmailEdoc1", Type = SqlDbType.NVarChar)]
        public string EmailEdoc1 { get; set; }

        public override void Fill(SqlDataReader sqlDataReader)
        {
            base.Fill(sqlDataReader);
            this.EmailEdoc1 = this.GetData<string>(sqlDataReader, "EmailEdoc1");
        }

        public override void FillOut(SqlCommand sqlCommand)
        {
            base.FillOut(sqlCommand);
        }
    }
}
