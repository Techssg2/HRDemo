using Aeon.HR.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data
{
    public class DbInitializer : System.Data.Entity.CreateDatabaseIfNotExists<HRDbContext>
    {
        protected override void Seed(HRDbContext context)
        {
            //var sqlContent = ReflectionHelper.ReadResourceFile<DbInitializer>("Aeon.HR.Data.sql_initializer.sql");
            //context.Database.ExecuteSqlCommand(sqlContent);

        }
    }
}