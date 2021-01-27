using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trip.Api.Models
{
    public class CalendarDataApiResponse
    {
        public List<List<CalendarDateApi>> Dates { get; set; }
    }
    public class CalendarDateApi
    {
        public string DateString { get; set; }
        public string MinPrice { get; set; }
    }
}