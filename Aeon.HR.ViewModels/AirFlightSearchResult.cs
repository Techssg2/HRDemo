using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class AirFlightSearchResult
    {
        public AirFlightSearchResult()
        {
            GroupPricedItineraries = new List<GroupPricedItineraryViewModel>();
            Page = new AirFlightSearchPageInfo();
            Errors = new List<object>();
        }
        public string ReturnSearchId { get; set; }
        public string DepartureSearchId { get; set; }
        public int Duration { get; set; }
        public List<object> Errors { get; set; }
        public List<GroupPricedItineraryViewModel> GroupPricedItineraries { get; set; }
        public string Infos { get; set; }
        public AirFlightSearchPageInfo Page { get; set; }
        public string SearchId { get; set; }
        public bool Success { get; set; }
        public string TextMessage { get; set; }
    }

    public class AirFlightSearchPageInfo
    {
        public int NextPageNumber { get; set; }
        public int Offset { get; set; }
        public int PageNumber { get; set; }
        public int PreviousPageNumber { get; set; }
        public int TotalElements { get; set; }
        public int TotalPage { get; set; }
    }
}
