using System.ComponentModel.DataAnnotations;

namespace Trip.Api.Models
{
    public class FlightDataRequest
    {
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string EndDate { get; set; }
        [Required]
        public string OriginAirport { get; set; }
        [Required]
        public string DestinationAirport { get; set; }
    }
}
