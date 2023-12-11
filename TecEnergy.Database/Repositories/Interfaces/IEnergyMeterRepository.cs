using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;

namespace TecEnergy.Database.Repositories.Interfaces;

public interface IEnergyMeterRepository
{
    Task<IEnumerable<EnergyMeter>> GetAllAsync();
    Task<EnergyMeter> GetByIdAsync(Guid id);
    Task AddAsync(EnergyMeter building);
    Task UpdateAsync(EnergyMeter building);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<EnergyMeter>> SearchAsync(string searchInput);
    Task<EnergyMeter> GetByIdDatetimeAsync(Guid id, DateTime? startDate, DateTime? endTime);
}