using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Trip.Api.Extensions;
using Trip.Api.Models;

namespace Trip.Api.Services
{
    public class SkyscannerService: ISkyscannerService
    {
        private string BaseApiUrl;
        private string ApiKey;
        public SkyscannerService(IConfiguration configuration)
        {
            BaseApiUrl = configuration.GetSection("Skyscanner").GetSection("ApiUrl").Value;
            ApiKey = configuration.GetSection("Skyscanner").GetSection("ApiKey").Value;
        }

        public async Task<CalendarDataResponse> GetCalendarData(FlightDataRequest request)
        {
            var start = Convert.ToDateTime(request.StartDate);
            List<DateTime> dates = new List<DateTime>();
            while (start <= Convert.ToDateTime(request.EndDate))
            {
                dates.Add(start);
                start = start.AddMonths(1);
            }

            List<CalendarDataApiResponse> responses = new List<CalendarDataApiResponse>();

            foreach (var date in dates)
            {
                var extraUrl = $"apiservices/browsegrid/v1.0/EE/EUR/et-EE/{request.OriginAirport}/{request.DestinationAirport}/{date:yyyy-MM}/{date:yyyy-MM}?apiKey={ApiKey}";
                var calendarDataApiResponse = await CallApi<CalendarDataApiResponse>(HttpMethod.Get, extraUrl);
                responses.Add(calendarDataApiResponse);
                extraUrl = $"apiservices/browsegrid/v1.0/EE/EUR/et-EE/{request.OriginAirport}/{request.DestinationAirport}/{date:yyyy-MM}/{date.AddMonths(1):yyyy-MM}?apiKey={ApiKey}";
                calendarDataApiResponse = await CallApi<CalendarDataApiResponse>(HttpMethod.Get, extraUrl);
                responses.Add(calendarDataApiResponse);
            }

            return responses.CalendarDataApiToCalendarData();
        }
        public async Task<float> GetLivePrice(FlightDataRequest request)
        {
            try
            {
                var priceHttpResponse = await GetHttpResponse(HttpMethod.Post, "apiservices/pricing/v1.0", new FormUrlEncodedContent(MakeRequestContentDictionary(request)));
                var bookingDataResponse = await CallApi<BookingDataApiResponse>(HttpMethod.Get, priceHttpResponse.Headers.Location.AbsolutePath + $"?apikey={ApiKey}&sortTupe=price?sortOrder=asc");

                while (bookingDataResponse.Status.Equals("UpdatesPending"))
                {
                    bookingDataResponse  = await CallApi<BookingDataApiResponse>(HttpMethod.Get, priceHttpResponse.Headers.Location.AbsolutePath + $"?apikey={ApiKey}&sortTupe=price?sortOrder=asc");
                }

                var cheapestIniniary = bookingDataResponse.Itineraries.OrderBy(o => o.PricingOptions[0].Price)
                                                                      .First();

                var cheapestBookingRequestContent = new StringContent(cheapestIniniary.BookingDetailsLink.Body, Encoding.UTF8, "application/x-www-form-urlencoded");

                var bookingHttpResponse = await GetHttpResponse(HttpMethod.Put, cheapestIniniary.BookingDetailsLink.Uri + $"?apikey={ApiKey}", cheapestBookingRequestContent);
                var bookingOptionsResponse = await CallApi<BookingOptionsDataResponse>(HttpMethod.Get, bookingHttpResponse.Headers.Location.AbsolutePath + $"?apikey={ApiKey}");
                
                return bookingOptionsResponse.BookingOptions.First().BookingItems.First().Price;
            }
            catch (Exception)
            {
                throw;
            }         
        }

        private async Task<T> CallApi<T>(HttpMethod method = null, string extraUrl = "")
        {
            var client = new HttpClient();
            var requestMessage = new HttpRequestMessage(method ?? HttpMethod.Get, BaseApiUrl.TrimEnd('/') + "/" + extraUrl.TrimStart('/'));

            if (!requestMessage.Headers.Contains("Accept"))
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await client.SendAsync(requestMessage))
            {
                try
                {
                    var repsonesContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(repsonesContent);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        private async Task<HttpResponseMessage> GetHttpResponse(HttpMethod method = null, string extraUrl = "", HttpContent content = null)
        {
            try
            {
                 var client = new HttpClient();
                 var requestMessage = new HttpRequestMessage(method ?? HttpMethod.Get, BaseApiUrl.TrimEnd('/') + "/" + extraUrl);

                 if (!requestMessage.Headers.Contains("Accept"))
                    requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                 if (content != null)
                     requestMessage.Content = content;

                 return await client.SendAsync(requestMessage);
            }
            catch (Exception)
            {
                throw;
            }        
        }

        private Dictionary<string, string> MakeRequestContentDictionary(FlightDataRequest request)
        {
            var contentDictionary = new Dictionary<string, string>();
            contentDictionary.Add("Content-Type", "application/x-www-form-urlencoded");
            contentDictionary.Add("cabinclass", "Economy");
            contentDictionary.Add("country", "EE");
            contentDictionary.Add("currency", "EUR");
            contentDictionary.Add("locale", "et-EE");
            contentDictionary.Add("locationSchema", "iata");
            contentDictionary.Add("originplace", request.OriginAirport);
            contentDictionary.Add("destinationplace", request.DestinationAirport);
            contentDictionary.Add("outbounddate", request.StartDate);
            contentDictionary.Add("inbounddate", request.EndDate);
            contentDictionary.Add("adults", "1");
            contentDictionary.Add("children", "0");
            contentDictionary.Add("infants", "0");
            contentDictionary.Add("apikey", ApiKey);

            return contentDictionary;
        }
    }

}