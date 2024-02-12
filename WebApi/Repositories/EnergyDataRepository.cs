using WebApi.Data;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class EnergyDataRepository : IEnergyDataRepository
    {
        private readonly DatabaseContext _context;
        public EnergyDataRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<EnergyData> CreateData(EspDataDto data)
        {
            EnergyData energyData = new EnergyData
            {
                Id = Guid.NewGuid(),
                MeterId = data.MeterId,
                Date = data.DateTime,
                AccumulatedValue = data.AccumulatedValue
            };

            await _context.EnergyData.AddAsync(energyData);

            // check if the save was successful and return the object if it was
            if(await _context.SaveChangesAsync() > 0)
            {
                return energyData;
            }

            // return error
            throw new Exception("Failed to save data");


        }
    }
}
