using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room> CreateAsync(Guid BuilidingId);
        Task<ICollection<Room>> GetSimpleInfoAsyn();
    }
}
