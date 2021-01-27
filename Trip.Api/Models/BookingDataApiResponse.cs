using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trip.Api.Models
{

    public class BookingDataApiResponse
    {
        public string Status { get; set; }
        public List<Itinerary> Itineraries { get; set; }
    }


    public class Itinerary
    {
        public string OutboundLegId { get; set; }
        public string InboundLegId { get; set; }
        public List<Pricingoption> PricingOptions { get; set; }
        public Bookingdetailslink BookingDetailsLink { get; set; }
    }

    public class Bookingdetailslink
    {
        public string Uri { get; set; }
        public string Body { get; set; }
    }

    public class Pricingoption
    {
        public float Price { get; set; }
        public string DeeplinkUrl { get; set; }
    }
}
