using WebApi.Dtos;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room> CreateAsync(Guid BuilidingId);
        Task<ICollection<SimpleInfoDto>> GetSimpleInfoAsync();
        Task<Room> GetRoomByIdAsync(Guid id);
    }
}
