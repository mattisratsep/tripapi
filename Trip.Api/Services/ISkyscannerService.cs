using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trip.Api.Models;

namespace Trip.Api.Services
{
    public interface ISkyscannerService
    {
      Task<CalendarDataResponse> GetCalendarData(FlightDataRequest request);
      Task<float> GetLivePrice(FlightDataRequest request);
    }
}
