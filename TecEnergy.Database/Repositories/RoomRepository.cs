using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
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

    public async Task<Room> GetFirstRoomAsync()
    {
        return await _context.Rooms.FirstAsync();
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _context.Rooms.ToListAsync();
    }

    public async Task<Room> GetByIdAsync(Guid id)
    {
        return await _context.Rooms.FindAsync(id);
    }

    public async Task<Building> GetBuildingByIdAsync(Guid buildingId)
    {
        return await _context.Buildings.Include(x => x.Rooms).FirstOrDefaultAsync(x => x.Id == buildingId);
    }

    public async Task<Room> GetByIdWithEnergyMetersAsync(Guid id)
    {
        return await _context.Rooms.Include(x => x.EnergyMeters).ThenInclude(x => x.EnergyDatas).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Room> GetByIdWithEnergyMetersFirstAndLastAsync(Guid id, DateTime? startDateTime, DateTime? endDateTime)
    {
        return await _context.Rooms.Include(x => x.EnergyMeters)
            .ThenInclude(x => x.EnergyDatas.Where(x => x.DateTime >= startDateTime && x.DateTime <= endDateTime))
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
    }

    public async Task<EnergyData> GetLatestEnergyDataAsync(Guid energyMeterId)
    {
        return await _context.EnergyData.OrderBy(x => x.AccumulatedValue).Where(x => x.EnergyMeterID == energyMeterId).LastOrDefaultAsync();
        //return await _context.EnergyData.OrderBy(x => x.AccumulatedValue).LastOrDefaultAsync();
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

    async Task<ICollection<DailyAccumulated>> IRoomRepository.GetDailyAccumulationAsync(Guid roomId, DateTime startTime, DateTime? endTime)
    {
        return await _context.DailyAccumulated
            .Where(x => x.RoomId == roomId && x.DateTime >= startTime && x.DateTime <= endTime)
            .ToListAsync();

    }
}
