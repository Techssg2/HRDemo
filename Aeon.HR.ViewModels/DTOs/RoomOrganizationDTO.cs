using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class RoomOrganizationDTO
    {
        public RoomOrganizationDTO()
        {
            Users = new HashSet<SimpleUserDTO>();
        }
        public Guid? Id { get; set; }
        public Guid RoomTypeId { get; set; }
        public string RoomTypeCode { get; set; }
        public string RoomTypeName { get; set; }
        public ICollection<SimpleUserDTO> Users { get; set; } // json
        public int TripGroup { get; set; }
    }
}
