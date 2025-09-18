using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    /// <summary>
    /// Map qua để đẩy position qua Mass
    /// </summary>
    public class PositionMassViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }   // Job Title
        public int? RequiredQuantity { get; set; }
        public int? AlertQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public int JobGrade { get; set; }
        public string MassLocationCode { get; set; }
        public string Description { get; set; }
        public string ReferenceRTH { get; set; }
    }
    public class PositionChangingStatus
    {
        public Guid Id { get; set; }
        public bool InActive { get; set; }
    }
}
