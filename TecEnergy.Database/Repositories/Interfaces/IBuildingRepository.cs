using TecEnergy.Database.DataModels;

namespace TecEnergy.Database.Repositories.Interfaces;

public interface IBuildingRepository
{
    Task<IEnumerable<Building>> GetAllAsync();
    Task<Building> GetByIdAsync(Guid? id);
    Task AddAsync(Building building);
    Task UpdateAsync(Building building);
    Task DeleteAsync(Guid id);
}