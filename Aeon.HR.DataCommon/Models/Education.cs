using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class Education: IEntity
    {
        public Guid Id { get ; set ; }
        public Guid ApplicantId { get; set; }

        public string Level { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Certificate { get; set; }
        public string SchoolName { get; set; }
        public DateTimeOffset Created { get ; set ; }
        public DateTimeOffset Modified { get ; set ; }
    }
}
