using Newtonsoft.Json;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.Hotel
{
    public class FilterOptions
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("prices")]
        public List<Price> Prices { get; set; }

        [JsonProperty("guestRatings")]
        public List<GuestRating> GuestRatings { get; set; }

        [JsonProperty("propertyRatings")]
        public List<PropertyRating> PropertyRatings { get; set; }

        [JsonProperty("propertyAmenities")]
        public List<PropertyAmenity> PropertyAmenities { get; set; }

        [JsonProperty("propertyCategories")]
        public List<PropertyCategory> PropertyCategories { get; set; }

        [JsonProperty("roomAmenities")]
        public List<RoomAmenity> RoomAmenities { get; set; }

        [JsonProperty("roomViews")]
        public List<RoomView> RoomViews { get; set; }

        [JsonProperty("themes")]
        public List<Theme> Themes { get; set; }

        [JsonProperty("mealPlans")]
        public List<MealPlan> MealPlans { get; set; }
    }
}
