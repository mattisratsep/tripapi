using System;
using System.Collections.Generic;
using System.Linq;
using Trip.Api.Models;

namespace Trip.Api.Extensions
{
    public static class CalendarDataExtension
    {
        public static CalendarDataResponse CalendarDataApiToCalendarData(this List<CalendarDataApiResponse> apiResponses)
        {
            CalendarDataResponse resp = new CalendarDataResponse();
            resp.Data = new Dictionary<string, Data>();

            foreach (var apiResponse in apiResponses)
            {
                var allDates = apiResponse.Dates.First();
                for (int i = 1; i < allDates.Count; i++)
                {
                    Data data = new Data();
                    data.Dates = new Dictionary<string, int>();
                    for (int j = 1; j < apiResponse.Dates.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(apiResponse.Dates[j][i]?.MinPrice))
                        {
                            data.Dates.Add(apiResponse.Dates[j][0]?.DateString, Convert.ToInt32(apiResponse.Dates[j][i]?.MinPrice));

                        }
                    }
                    if (data.Dates.Any())
                    {
                        if (resp.Data.ContainsKey(allDates[i].DateString))
                        {
                            resp.Data[allDates[i].DateString].Dates = resp.Data[allDates[i].DateString].Dates.Concat(data.Dates)
                                                                                                             .ToDictionary(y => y.Key, x => x.Value);            
                        }
                        else
                        {
                            resp.Data.Add(allDates[i].DateString, data);
                        }
                    }
                }
            }

            foreach (var item in resp.Data)
            {
                item.Value.SetPrice();
            }

            resp.SetActiveDate();

            return resp;
        }
    }
}
