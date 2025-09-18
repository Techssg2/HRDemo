using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class AdultFare
    {
        [JsonProperty("passengerTypeQuantities")]
        public PassengerTypeQuantities PassengerTypeQuantities { get; set; }

        [JsonProperty("fareBasisCodes")]
        public object FareBasisCodes { get; set; }

        [JsonProperty("passengerFare")]
        public PassengerFare PassengerFare { get; set; }
    }
}
