using AEON.Integrations.Gotadi.Common;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.QueryParams.AirTicket
{
    public class SearchQueryParams : BaseQueryParams
    {
        [AliasAs("origin_code")]
        public string OriginCode { get; set; }

        [AliasAs("destination_code")]
        public string DestinationCode { get; set; }

        [AliasAs("departure_date")]
        public string DepartureDate { get; set; }

        [AliasAs("returnture_date")]
        public string ReturntureDate { get; set; }

        [AliasAs("cabin_class")]
        public string CabinClass { get; set; }

        [AliasAs("route_type")]
        public string RouteType { get; set; }

        [AliasAs("aduts_qtt")]
        public int AdutsQtt { get; set; }

        [AliasAs("children_qtt")]
        public int ChildrenQtt { get; set; }

        [AliasAs("infants_qtt")]
        public int InfantsQtt { get; set; }

        [AliasAs("include-equivfare")]
        public bool IncludeQquivfare { get; set; }

        [AliasAs("page")]
        public int Page { get; set; }

        [AliasAs("size")]
        public int Size { get; set; }

        [AliasAs("suppliers")]
        public string Suppliers { get; set; }

        public override string Key
        {
            get
            {
                var combined = $"{OriginCode}{DestinationCode}{DepartureDate}{ReturntureDate}{CabinClass}{RouteType}{AdutsQtt}{ChildrenQtt}{InfantsQtt}{Page}{Size}{Time}";
                return GenerateKey(combined);
            }
        }

    }
}
