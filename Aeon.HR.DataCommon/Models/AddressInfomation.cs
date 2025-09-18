using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class AddressInfomation : IEntity
    {
        public Guid Id { get ; set ; }       
        public Guid ApplicantId { get; set; }

        public string AddressType { get; set; }
        public string Address { get; set; }
        public string WardCode { get; set; }
        public string DistrictCode { get; set; }
        public string ProvinceCode { get; set; }
        public string CountryCode { get; set; }
        public DateTimeOffset Created { get ; set ; }
        public DateTimeOffset Modified { get ; set ; }

        public virtual Applicant Applicant { get; set; }
    }
}
