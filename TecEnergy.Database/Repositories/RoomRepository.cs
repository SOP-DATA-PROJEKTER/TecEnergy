using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.Database.Repositories;
public class RoomRepository : IRoomRepository
{
    private readonly DatabaseContext _context;

    public RoomRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _context.Rooms.ToListAsync();
    }

    public async Task<Room> GetByIdAsync(Guid id)
    {
        return await _context.Rooms.FindAsync(id);
    }
    public async Task<Room> GetByIdWithEnergyMetersAsync(Guid id)
    {
        return await _context.Rooms.Include(x => x.EnergyMeters).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Room room)
    {
        _context.Entry(room).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room is not null)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Room>> SearchAsync(string searchInput)
    {
        return await _context.Rooms
            .Where(x => x.RoomName.Contains(searchInput) || x.RoomComment.Contains(searchInput))
            .ToListAsync();
    }
}
