using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using JobResignationV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResignationV2.src.ModelEntity
{
    [JobResignationV2.src.SQLExcute.TableAttribute(TableName = "UserDepartmentMappings")]
    public class UserDepartmentMappingEntity : BaseEntity
    {
        public Guid? DepartmentId { get; set; }
        public Guid? UserId { get; set; }
        public Group Role { get; set; } 
        public bool IsHeadCount { get; set; }
 
        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
        public string Note { get; set; }
        public bool? Authorizated { get; set; }
        public bool IsFromIT { get; set; }

        public override void Fill(SqlDataReader sqlDataReader)
        {
            base.Fill(sqlDataReader);
            this.UserId = this.GetData<Guid>(sqlDataReader, "UserId");
        }
        public override void FillOut(SqlCommand sqlCommand)
        {
            base.FillOut(sqlCommand);
        }
    }
}
