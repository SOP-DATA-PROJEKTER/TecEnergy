using TecEnergy.Database.DataModels;

namespace TecEnergy.Database.Repositories.Interfaces;

public interface IEnergyMeterRepository
{
    Task<IEnumerable<EnergyMeter>> GetAllAsync();
    Task<EnergyMeter> GetByIdAsync(Guid id);
    Task AddAsync(EnergyMeter building);
    Task UpdateAsync(EnergyMeter building);
    Task DeleteAsync(Guid id);
}