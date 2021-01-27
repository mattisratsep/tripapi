using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trip.Api.Models;
using Trip.Api.Services;

namespace Trip.Api.Controllers
{
    public class SkyscannerController : ControllerBase
    {
        private ISkyscannerService _skyscannerService;
        public SkyscannerController(ISkyscannerService skyscannerService)
        {
            _skyscannerService = skyscannerService;
        }

        [HttpPost]
        [Route("calendar")]
        public async Task<IActionResult> GetCalendarData([FromBody] FlightDataRequest calendarDataRequest)
        {
            return Ok(await _skyscannerService.GetCalendarData(calendarDataRequest));
        }

        [HttpPost]
        [Route("live-price")]
        public async Task<IActionResult> GetBookingData([FromBody] FlightDataRequest calendarDataRequest)
        {
            return Ok(await _skyscannerService.GetLivePrice(calendarDataRequest));
        }
    }
}
