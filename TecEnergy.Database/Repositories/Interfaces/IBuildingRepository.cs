using TecEnergy.Database.Models.DataModels;

namespace TecEnergy.Database.Repositories.Interfaces;

public interface IBuildingRepository
{
    Task<IEnumerable<Building>> GetAllAsync();
    Task<Building> GetByIdAsync(Guid? id);
    Task<Building> GetByIdWithRoomsAsync(Guid? id);
    Task AddAsync(Building building);
    Task UpdateAsync(Building building);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Building>> SearchAsync(string searchInput);
}