using System;

namespace Aeon.HR.ViewModels
{
    public class StopQuantityInfoViewModel
    {
        public DateTime ArrivalDateTime { get; set; }
        public string City { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string Duration { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
    }
}
