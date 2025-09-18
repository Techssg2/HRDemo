using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class SimpleUserDTO
    {
        public Guid Id { get; set; }
        public Guid? RoomOrganizationId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? BusinessTripApplicationDetailId { get; set; }
        public Guid? ChangeCancelBusinessTripApplicationDetailId { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }    
        public string HotelName { get; set; }
        public string HotelCode { get; set; }        
        public bool IsChange { get; set; }
    }
}
