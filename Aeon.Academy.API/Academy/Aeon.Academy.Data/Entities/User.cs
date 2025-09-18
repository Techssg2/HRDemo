using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeon.Academy.Data.Entities
{
    [Table("Users")]
    public class User : BaseEntity
    {
        public string LoginName { get; set; }
        public string SapCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public int Type { get; set; }

    }
}
