using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class RoomOrganization: IEntity
    {
        public RoomOrganization()
        {
            RoomUserMappings = new HashSet<RoomUserMapping>();
        }
        public Guid Id { get; set; }
        public Guid RoomTypeId { get; set; }
        public string RoomTypeCode { get; set; }
        public string RoomTypeName { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }      
        public Guid BusinessTripApplicationId { get; set; }
        public virtual BusinessTripApplication BusinessTripApplication { get; set; }
        public virtual RoomType RoomType { get; set; }
        public ICollection<RoomUserMapping> RoomUserMappings { get; set; }
        public int TripGroup { get; set; }
    }
}
