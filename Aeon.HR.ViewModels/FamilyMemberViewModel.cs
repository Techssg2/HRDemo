using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class FamilyMemberViewModel
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public string RelationShip { get; set; }
        public string RelationShipName { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Occupation { get; set; }
        public string PlaceOfOccupation { get; set; }
        public string ContactNumber { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}