using WebApi.Dtos;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IEnergyDataRepository
    {
        Task<bool> CreateData(List<EspDataDto> data);
    }
}
