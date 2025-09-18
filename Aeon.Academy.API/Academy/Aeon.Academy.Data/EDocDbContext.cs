using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data
{
    public class EDocDbContext : DbContext
    {
        public EDocDbContext() : base("HRDbContext")
        {
            Database.SetInitializer<EDocDbContext>(null);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<WorkflowTemplate> WorkflowTemplate { get; set; }
        public DbSet<UserDepartmentMapping> UserDepartmentMappings { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ReferenceNumber> ReferenceNumbers { get; set; }
        public DbSet<JobGrade> JobGrades { get; set; }
    }
}
