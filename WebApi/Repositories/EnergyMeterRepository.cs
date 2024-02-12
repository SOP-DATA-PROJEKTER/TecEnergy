using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;
namespace WebApi.Repositories
{
    public class EnergyMeterRepository : IEnergyMeterRepository
    {
        public Task<EnergyMeter> CreateAsync(EnergyMeter energyMeter)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<SimpleGraphDto>> GetDailyAsync(Guid id, DateOnly date)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<SimpleGraphDto>> GetMonthlyAsync(Guid id, DateOnly date)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<SimpleGraphDto>> GetYearlyAsync(Guid id, DateOnly date)
        {
            throw new NotImplementedException();
        }
    }
}
