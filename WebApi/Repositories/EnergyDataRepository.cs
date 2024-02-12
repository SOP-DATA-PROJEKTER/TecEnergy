using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class EnergyDataRepository : IEnergyDataRepository
    {
        public Task<EnergyData> CreateData(EspDataDto data)
        {
            throw new NotImplementedException();
        }
    }
}
