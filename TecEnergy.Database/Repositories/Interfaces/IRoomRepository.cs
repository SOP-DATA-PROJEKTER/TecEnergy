using TecEnergy.Database.DataModels;

namespace TecEnergy.Database.Repositories.Interfaces;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> GetAllAsync();
    Task<Room> GetByIdAsync(Guid id);
    Task<Room> GetByIdWithEnergyMetersAsync(Guid id);
    Task AddAsync(Room room);
    Task UpdateAsync(Room room);
    Task DeleteAsync(Guid id);
}