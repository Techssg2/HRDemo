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
    [TableAttribute(TableName = "UserDepartmentMappings")]
    public class UserDepartmentMappingEntity : BaseEntity
    {
        [FieldAttribute(FieldName = "UserId", Type = SqlDbType.UniqueIdentifier)]
        public Guid UserId { get; set; }

        [FieldAttribute(FieldName = "DepartmentId", Type = SqlDbType.UniqueIdentifier)]
        public Guid DepartmentId { get; set; }

        [FieldAttribute(FieldName = "Role", Type = SqlDbType.Int)]
        public int Role { get; set; }

        [FieldAttribute(FieldName = "IsHeadCount", Type = SqlDbType.Bit)]
        public bool IsHeadCount { get; set; }

        [FieldAttribute(FieldName = "Created", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset Created { get; set; }

        [FieldAttribute(FieldName = "CreatedBy", Type = SqlDbType.NVarChar)]
        public string CreatedBy { get; set; }

        [FieldAttribute(FieldName = "Modified", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset Modified { get; set; }

        [FieldAttribute(FieldName = "ModifiedBy", Type = SqlDbType.NVarChar)]
        public string ModifiedBy { get; set; }

        // Thuộc tính mở rộng không có trong bảng
        // ExcludeUpdate = true để đảm bảo rằng thuộc tính này không được bao gồm trong các truy vấn INSERT/UPDATE
        [FieldAttribute(FieldName = "UserFullName", Type = SqlDbType.NVarChar, ExcludeUpdate = true)]
        public string UserFullName { get; set; }

        [FieldAttribute(FieldName = "UserEmail", Type = SqlDbType.NVarChar, ExcludeUpdate = true)]
        public string UserEmail { get; set; }

        public override void Fill(SqlDataReader sqlDataReader)
        {
            base.Fill(sqlDataReader);
            this.UserId = this.GetData<Guid>(sqlDataReader, "UserId");
            this.DepartmentId = this.GetData<Guid>(sqlDataReader, "DepartmentId");
            this.Role = this.GetData<int>(sqlDataReader, "Role");
            this.IsHeadCount = this.GetData<bool>(sqlDataReader, "IsHeadCount");
            this.Created = this.GetData<DateTimeOffset>(sqlDataReader, "Created");
            this.CreatedBy = this.GetData<string>(sqlDataReader, "CreatedBy");
            this.Modified = this.GetData<DateTimeOffset>(sqlDataReader, "Modified");
            this.ModifiedBy = this.GetData<string>(sqlDataReader, "ModifiedBy");

            // Kiểm tra xem cột có tồn tại trong kết quả truy vấn không trước khi đọc
            if (sqlDataReader.HasColumn("UserFullName"))
                this.UserFullName = this.GetData<string>(sqlDataReader, "UserFullName");

            if (sqlDataReader.HasColumn("UserEmail"))
                this.UserEmail = this.GetData<string>(sqlDataReader, "UserEmail");
        }

        public override void FillOut(SqlCommand sqlCommand)
        {
            base.FillOut(sqlCommand);

            sqlCommand.Parameters.Add(new SqlParameter("UserId", SqlDbType.UniqueIdentifier) { Value = this.UserId });
            sqlCommand.Parameters.Add(new SqlParameter("DepartmentId", SqlDbType.UniqueIdentifier) { Value = this.DepartmentId });
            sqlCommand.Parameters.Add(new SqlParameter("Role", SqlDbType.Int) { Value = this.Role });
            sqlCommand.Parameters.Add(new SqlParameter("IsHeadCount", SqlDbType.Bit) { Value = this.IsHeadCount });
            sqlCommand.Parameters.Add(new SqlParameter("Created", SqlDbType.DateTimeOffset) { Value = this.Created });
            sqlCommand.Parameters.Add(new SqlParameter("CreatedBy", SqlDbType.NVarChar) { Value = this.CreatedBy ?? (object)DBNull.Value });
            sqlCommand.Parameters.Add(new SqlParameter("Modified", SqlDbType.DateTimeOffset) { Value = this.Modified });
            sqlCommand.Parameters.Add(new SqlParameter("ModifiedBy", SqlDbType.NVarChar) { Value = this.ModifiedBy ?? (object)DBNull.Value });
            // Không thêm tham số cho các trường mở rộng ExcludeUpdate = true
        }
    }
}