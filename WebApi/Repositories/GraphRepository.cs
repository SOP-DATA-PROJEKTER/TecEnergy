using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Repositories
{
    public class GraphRepository : IGraphRepository
    {

        private readonly DatabaseContext _context;


        public GraphRepository(DatabaseContext context)
        {
            _context = context;
        }


        public async Task<ICollection<DateValueDto>> GetDailyAsync(Guid meterId, DateTime date)
        {
            // get the data for the whole month as a list containing daily data
            // eg. get the data for the 1st of Jan and put it in a list, then get the data for the 2nd of Jan and put it in the list and so on untill the end of the month
            // then return the list
            // but only for the given id



            var result = await _context.DailyAccumulations
                .Where(x => x.EnergyMeterId == meterId && x.DateTime.Month == date.Month && x.DateTime.Year == date.Year)
                .OrderBy(x => x.DateTime)
                .ToListAsync() ?? throw new Exception("No data found");

            // map result to SimpleGraphDto
            List<DateValueDto> data = new();
            foreach (var item in result)
            {
                DateValueDto dto = new()
                {
                    Date = DateOnly.FromDateTime(item.DateTime),
                    AccumulatedValue = item.AccumulatedValue
                };
                data.Add(dto);
            }

            return data;

        }


        public async Task<ICollection<DateValueDto>> GetMonthlyAsync(Guid meterId, DateTime date)
        {
            // Get data for the whole year as a list containing monthly data
            // eg. get the data for Jan and put it in a list, then get the data for Feb and put it in the list and so on untill the end of the year
            // then return the list
            // but only for the given id
            // 
            // since the results are in the format of daily data, we need to sum the values before mapping them to the SimpleGraphDto

            var result = await _context.DailyAccumulations
                .Where(x => x.EnergyMeterId == meterId && x.DateTime.Year == date.Year)
                .ToListAsync() ?? throw new Exception("No data found");

            // select the latest date in each month and take the accumulated value of that date

            // x.key is the month, since we groupBy month
            return result.GroupBy(x => x.DateTime.Month)
                .OrderBy(x => x.Key)
                .Select(x => new DateValueDto
                {
                    Date = new DateOnly(date.Year, x.Key, 1),
                    AccumulatedValue = x.Max(x => x.AccumulatedValue)
                }).ToList();
        }


        public async Task<ICollection<DateValueDto>> GetYearlyAsync(Guid meterId)
        {
            // get all data for the given id as a list of years and the data for the whole year. starting at year 2023.

            // change to get the first entry's year and the compare to that year instead of 2023

            // get first entry's year
            var firstEntry = await _context.DailyAccumulations
                .Where(x => x.EnergyMeterId == meterId)
                .OrderByDescending(x => x.DateTime)
                .FirstOrDefaultAsync() ?? throw new Exception("No data found");

            var result = await _context.DailyAccumulations
                .Where(x => x.EnergyMeterId == meterId && x.DateTime.Year >= 2020)
                .ToListAsync() ?? throw new Exception("No data found");

            return result.GroupBy(x => x.DateTime.Year)
                .OrderBy(x => x.Key)
                .Select(x => new DateValueDto
                {
                    Date = new DateOnly(x.Key, 1, 1),
                    AccumulatedValue = x.Max(x => x.AccumulatedValue)
                }).ToList();
        }


        public async Task<ICollection<DateValueDto>> GetDailyAsyncFromRoomId(Guid roomId, DateTime date)
        {
            // get all meterIds from the given roomId
            // then get the data for each meterId
            // if there are multiple meters, sum the values for each date
            // then return the list


            var meters = EnergyMetersFromRoomId(roomId);
            List<DateValueDto> data = new();

            foreach(var meter in meters.Result)
            {
                var result = GetDailyAsync(meter.Id, date).Result;

                foreach(var item in result)
                {
                    var existing = data.FirstOrDefault(x => x.Date == item.Date);
                    if(existing != null)
                    {
                        existing.AccumulatedValue += item.AccumulatedValue;
                    }
                    else
                    {
                        data.Add(item);
                    }
                }
            }

            // if there is no data, throw an exception
            if(data.Count == 0)
            {
                throw new Exception("No data found");
            }

            return data;
        }


        public async Task<ICollection<DateValueDto>> GetMonthlyAsyncFromRoomId(Guid roomId, DateTime date)
        {
            // get all meterIds from the given roomId
            // then get the data for each meterId
            // if there are multiple meters, sum the values for each date
            // then return the list


            var meters = EnergyMetersFromRoomId(roomId);
            List<DateValueDto> data = new();

            foreach (var meter in meters.Result)
            {
                var result = GetMonthlyAsync(meter.Id, date).Result;

                foreach (var item in result)
                {
                    var existing = data.FirstOrDefault(x => x.Date.Month == item.Date.Month);
                    if (existing != null)
                    {
                        existing.AccumulatedValue += item.AccumulatedValue;
                    }
                    else
                    {
                        data.Add(item);
                    }
                }
            }

            // if there is no data, throw an exception
            if (data.Count == 0)
            {
                throw new Exception("No data found");
            }

            return data;
        }


        public async Task<ICollection<DateValueDto>> GetYearlyAsyncFromRoomId(Guid roomId)
        {
            // get all meterIds from the given roomId
            // then get the data for each meterId
            // if there are multiple meters, sum the values for each date
            // then return the list


            var meters = EnergyMetersFromRoomId(roomId);
            List<DateValueDto> data = new();

            foreach (var meter in meters.Result)
            {
                var result = GetYearlyAsync(meter.Id).Result;

                foreach (var item in result)
                {
                    var existing = data.FirstOrDefault(x => x.Date.Year == item.Date.Year);
                    if (existing != null)
                    {
                        existing.AccumulatedValue += item.AccumulatedValue;
                    }
                    else
                    {
                        data.Add(item);
                    }
                }
            }

            // if there is no data, throw an exception
            if (data.Count == 0)
            {
                throw new Exception("No data found");
            }

            return data;
        }


        public async Task<bool> IsRoomId(Guid id)
        {
            return await _context.Rooms.AnyAsync(x => x.Id == id);
        }


        private async Task<ICollection<EnergyMeter>> EnergyMetersFromRoomId(Guid roomId)
        {
            return await _context.EnergyMeters.Where(x => x.RoomId == roomId).ToListAsync();
        }

    }
}
