using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trip.Api.Models
{
    public class CalendarDataResponse
    {
        [JsonProperty("activeDate")]
        public string ActiveDate { get; set; }
        [JsonProperty("data")]
        public Dictionary<string, Data> Data { get; set; }

        public void SetActiveDate()
        {
            ActiveDate = Data.Aggregate((l, r) => l.Value.Price < r.Value.Price ? l : r).Key;
        }
    }
    public class Data
    {
        [JsonProperty("price")]
        public int Price { get; set; }
        [JsonProperty("dates")]
        public Dictionary<string, int> Dates { get; set; } 

        public void SetPrice()
        {
            Price = Dates.Aggregate((l, r) => l.Value < r.Value ? l : r).Value;
        }
    }
}
