using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;
namespace WebApi.Repositories
{
    public class EnergyMeterRepository : IEnergyMeterRepository
    {
        private readonly DatabaseContext _context;


        public EnergyMeterRepository(DatabaseContext context)
        {
            _context = context;
        }


        public async Task<EnergyMeter> CreateAsync(SimpleInfoDto data)
        {
            EnergyMeter energyMeter = new EnergyMeter
            {
                Id = Guid.NewGuid(),
                RoomId = data.Id,
                Name = data.Name,
                IsConnected = true
            };

            await _context.EnergyMeters.AddAsync(energyMeter);
            if(await _context.SaveChangesAsync() > 0)
            {
                return energyMeter;
            }

            throw new Exception("Failed to save data");
        }

        public Task<bool> EnergyMeterExists(SimpleInfoDto data)
        {
            return _context.EnergyMeters
                .AnyAsync(x => x.RoomId == data.Id && x.Name == data.Name);
        }

        public async Task<ICollection<DateValueDto>> GetDailyAsync(Guid meterId, DateTime date)
        {
            // get the data for the whole month as a list containing daily data
            // eg. get the data for the 1st of Jan and put it in a list, then get the data for the 2nd of Jan and put it in the list and so on untill the end of the month
            // then return the list
            // but only for the given id

            var result = await _context.DailyAccumulations
                .Where(x => x.EnergyMeterId == meterId && x.DateTime.Month == date.Month)
                .ToListAsync() ?? throw new Exception("No data found");

            // map result to SimpleGraphDto

            return result.Select(x => new DateValueDto
            {
                Date = x.DateTime,
                AccumulatedValue = x.AccumulatedValue
            }).ToList();
        }


        public async Task<EnergyMeter> GetEnergyMeter(Guid id)
        {
            return await _context.EnergyMeters
                .Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("No data found");  
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

            // sum the values for each month
            // makes groups of the data by month and sums the values of each group, meaning each month.

            return result.GroupBy(x => x.DateTime.Month)
                .Select(x => new DateValueDto
                {
                    Date = new DateTime(date.Year, x.Key, 1),
                    AccumulatedValue = x.Sum(x => x.AccumulatedValue)
                }).ToList();
        }


        public async Task<ICollection<DateValueDto>> GetYearlyAsync(Guid meterId, DateTime date)
        {
            // get all data for the given id as a list of years and the data for the whole year. starting at year 2023.
            
            var result = await _context.DailyAccumulations
                .Where(x => x.EnergyMeterId == meterId && x.DateTime.Year >= 2023)
                .ToListAsync() ?? throw new Exception("No data found");

            return result.GroupBy(x => x.DateTime.Year)
                .Select(x => new DateValueDto
                {
                    Date = new DateTime(x.Key, 1, 1),
                    AccumulatedValue = x.Sum(x => x.AccumulatedValue)
                }).ToList();
        }

        public Task<bool> RoomExists(Guid roomId)
        {
            return _context.Rooms.AnyAsync(x => x.Id == roomId);
        }
    }
}
