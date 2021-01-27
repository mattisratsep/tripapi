using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trip.Api.Models
{

    public class BookingOptionsDataResponse
    {
        public List<Bookingoption> BookingOptions { get; set; }
    }
    public class Bookingoption
    {
        public List<Bookingitem> BookingItems { get; set; }
    }

    public class Bookingitem
    {
        public float Price { get; set; }
        public float AlternativePrice { get; set; }
        public string AlternativeCurrency { get; set; }
        public string Deeplink { get; set; }
    }

}
