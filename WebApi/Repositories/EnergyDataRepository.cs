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


        public async Task<bool> CreateData(List<EspDataDto> dataList)
        {
            foreach(var data in dataList)
            {
                var energyData = new EnergyData
                {
                    EnergyMeterId = data.MeterId,
                    AccumulatedValue = data.AccumulatedValue,
                    DateTime = DateTime.Parse(data.DateTime),
                };

                await _context.EnergyData.AddAsync(energyData);
            }

            return await _context.SaveChangesAsync() > 0;


        }
    }
}
