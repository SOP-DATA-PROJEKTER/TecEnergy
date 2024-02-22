using WebApi.Dtos;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IEnergyMeterRepository
    {
        Task<EnergyMeter> CreateAsync(SimpleInfoDto data);
        Task<EnergyMeter> GetEnergyMeter(Guid id);
        Task<bool> RoomExists(Guid roomId);
        Task<bool> EnergyMeterExists(SimpleInfoDto data);


    }
}
