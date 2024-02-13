using WebApi.Dtos;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room> CreateAsync(string name);
        Task<ICollection<SimpleInfoDto>> GetSimpleInfoAsync();
        Task<Room> GetRoomByIdAsync(Guid id);
        Task<bool> GetRoomByNameAsync(string name);
        Task<RoomDataDto> GetRoomDataAsync(Guid roomId);
    }
}
