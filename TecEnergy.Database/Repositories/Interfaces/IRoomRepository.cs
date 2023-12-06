using TecEnergy.Database.Models.DataModels;

namespace TecEnergy.Database.Repositories.Interfaces;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> GetAllAsync();
    Task<Room> GetByIdAsync(Guid id);
    Task<Room> GetByIdWithEnergyMetersAsync(Guid id);
    Task AddAsync(Room room);
    Task UpdateAsync(Room room);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Room>> SearchAsync(string searchInput);
}