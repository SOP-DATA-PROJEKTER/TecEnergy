using WebApi.Dtos;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IEnergyMeterRepository
    {
        Task<EnergyMeter> CreateAsync(Guid roomId);
        Task<ICollection<SimpleGraphDto>> GetDailyAsync(Guid id, DateOnly date);
        Task<ICollection<SimpleGraphDto>> GetMonthlyAsync(Guid id, DateOnly date);
        Task<ICollection<SimpleGraphDto>> GetYearlyAsync(Guid id, DateOnly date);


    }
}
