using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Interfaces;

namespace WebApi.Repositories
{
    public class GraphRepository: IGraphRepository
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
                .Where(x => x.EnergyMeterId == meterId && x.DateTime.Month == date.Month)
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

            return result.GroupBy(x => x.DateTime.Month)
                .OrderBy(x => x.Key)
                .Select(x => new DateValueDto
                {
                    Date = new DateOnly(date.Year, x.Key, 1),
                    AccumulatedValue = x.Max(x => x.AccumulatedValue)
                }).ToList();
        }


        // TODO: remove the date from parameter
        public async Task<ICollection<DateValueDto>> GetYearlyAsync(Guid meterId)
        {
            // get all data for the given id as a list of years and the data for the whole year. starting at year 2023.

            var result = await _context.DailyAccumulations
                .Where(x => x.EnergyMeterId == meterId && x.DateTime.Year >= 2023)
                .ToListAsync() ?? throw new Exception("No data found");

            return result.GroupBy(x => x.DateTime.Year)
                .OrderBy(x => x.Key)
                .Select(x => new DateValueDto
                {
                    Date = new DateOnly(x.Key, 1, 1),
                    AccumulatedValue = x.Max(x => x.AccumulatedValue)
                }).ToList();
        }


    }
}
