using System.Runtime.CompilerServices;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.WebAPI.Services;

public class RoomService
{
    private readonly IRoomRepository _repository;
    public RoomService(IRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        return result;
    }

    public async Task<Room> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        return result;
    }

    public async Task AddAsync(Room room)
    {
        await _repository.AddAsync(room);
    }

    public async Task UpdateAsync(Guid id, Room room)
    {
        await _repository.UpdateAsync(room);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Room>> SearchAsync(string searchInput)
    {
        var result = await _repository.SearchAsync(searchInput);
        return result;
    }
}
