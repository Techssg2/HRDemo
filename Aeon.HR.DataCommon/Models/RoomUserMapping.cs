using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class RoomUserMapping: IEntity
    {
        public Guid Id { get; set; }
        public Guid? RoomOrganizationId { get; set; }
        public Guid? BusinessTripApplicationDetailId { get; set; }
        public Guid? ChangeCancelBusinessTripApplicationDetailId { get; set; }
        public Guid? UserId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public bool IsChange { get; set; }
        public virtual RoomOrganization RoomOrganization { get; set; }
        public virtual BusinessTripApplicationDetail BusinessTripApplicationDetail { get; set; }
        public virtual ChangeCancelBusinessTripDetail ChangeCancelBusinessTripApplicationDetail { get; set; }
        public virtual User User { get; set; }
    }
}
