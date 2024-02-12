using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dtos;
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
        public async Task<Room> CreateAsync(Guid BuilidingId)
        {
            Room room = new Room
            {
                Id = Guid.NewGuid(),
                BuildingId = BuilidingId
            };

            await _context.Rooms.AddAsync(room);
            if (await _context.SaveChangesAsync() > 0)
            {
                return room;
            }
            throw new Exception("Failed to save data");

        }

        public async Task<Room> GetRoomByIdAsync(Guid id)
        {
            return await _context.Rooms
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("No Data Found");
        }

        public async Task<ICollection<SimpleInfoDto>> GetSimpleInfoAsync()
        {
            // this only works for 1 room beware!
            
            return await _context.Rooms
                .Include(x => x.EnergyMeters)
                .Select(x => new SimpleInfoDto
                {
                    roomId = x.Id,
                    meterIds = x.EnergyMeters.Select(y => y.Id).ToList(),
                    roomName = x.Name
                })
                .ToListAsync() ?? throw new Exception("No Data Found");

        }
    }
}
