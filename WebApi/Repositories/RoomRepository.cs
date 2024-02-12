using WebApi.Data;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly DatabaseContext _context;
        public RoomRepository(DatabaseContext context)
        {
            _context = context;
        }
        public Task<Room> CreateAsync(Guid BuilidingId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Room>> GetSimpleInfoAsyn()
        {
            throw new NotImplementedException();
        }
    }
}
