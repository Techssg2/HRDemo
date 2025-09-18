using JobSendMail1STResignation.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignation.src.ModelEntity
{
    [TableAttribute(TableName = "Departments")]
    public class DepartmentEntity : BaseEntity
    {
        [FieldAttribute(FieldName = "Code", Type = SqlDbType.NVarChar)]
        public string Code { get; set; }

        [FieldAttribute(FieldName = "Name", Type = SqlDbType.NVarChar)]
        public string Name { get; set; }

        [FieldAttribute(FieldName = "JobGradeId", Type = SqlDbType.UniqueIdentifier)]
        public Guid JobGradeId { get; set; }

        [FieldAttribute(FieldName = "ParentId", Type = SqlDbType.UniqueIdentifier)]
        public Guid? ParentId { get; set; }

        [FieldAttribute(FieldName = "IsStore", Type = SqlDbType.Bit)]
        public bool IsStore { get; set; }

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
            this.Code = this.GetData<string>(sqlDataReader, "Code");
            this.Name = this.GetData<string>(sqlDataReader, "Name");
            this.JobGradeId = this.GetData<Guid>(sqlDataReader, "JobGradeId");
            this.ParentId = this.GetData<Guid?>(sqlDataReader, "ParentId");
            this.IsStore = this.GetData<bool>(sqlDataReader, "IsStore");
            this.Created = this.GetData<DateTimeOffset>(sqlDataReader, "Created");
            this.CreatedBy = this.GetData<string>(sqlDataReader, "CreatedBy");
            this.Modified = this.GetData<DateTimeOffset>(sqlDataReader, "Modified");
            this.ModifiedBy = this.GetData<string>(sqlDataReader, "ModifiedBy");
        }
    }
}
