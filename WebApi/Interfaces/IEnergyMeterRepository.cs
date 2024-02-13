using WebApi.Dtos;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IEnergyMeterRepository
    {
        Task<EnergyMeter> CreateAsync(SimpleInfoDto data);
        Task<EnergyMeter> GetEnergyMeter(Guid id);
        Task<ICollection<DateValueDto>> GetDailyAsync(Guid meterId, DateTime date);
        Task<ICollection<DateValueDto>> GetMonthlyAsync(Guid meterId, DateTime date);
        Task<ICollection<DateValueDto>> GetYearlyAsync(Guid meterId, DateTime date);
        Task<bool> RoomExists(Guid roomId);
        Task<bool> EnergyMeterExists(SimpleInfoDto data);


    }
}
