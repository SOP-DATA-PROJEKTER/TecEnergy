using WebApi.Dtos;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IEnergyDataRepository
    {
        Task<EnergyData> CreateData(EspDataDto data);
    }
}
