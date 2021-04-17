using Common.Interfaces;
using Newtonsoft.Json;

namespace Common.Models
{
    public class IPDetails : IIPDetails
    {
        public string City { get; set; }

        [JsonProperty(PropertyName = "country_name")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "continent_name")]
        public string Continent { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
