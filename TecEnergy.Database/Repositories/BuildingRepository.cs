using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.Database.Repositories;
public class BuildingRepository : IBuildingRepository
{
    private readonly DatabaseContext _context;

    public BuildingRepository(DatabaseContext context) => _context = context;

    public async Task<IEnumerable<Building>> GetAllAsync()
    {
        return await _context.Buildings.ToListAsync();
    }

    public async Task<Building> GetByIdAsync(Guid? id)
    {
        return await _context.Buildings.FindAsync(id);
    }

    public async Task<Building> GetByIdWithRoomsAsync(Guid? id)
    {
        return await _context.Buildings.Include(r => r.Rooms).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Building building)
    {
        _context.Buildings.Add(building);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Building building)
    {
        _context.Entry(building).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var building = await _context.Buildings.FindAsync(id);
        if (building is not null)
        {
            _context.Buildings.Remove(building);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> BuildingExistsAsync(Guid buildingId)
    {
        return await _context.Buildings.AnyAsync(b => b.Id == buildingId);
    }

    public async Task<IEnumerable<Building>> SearchAsync(string searchInput)
    {
        return await _context.Buildings
            .Where(x => x.BuildingName.Contains(searchInput) || x.Address.Contains(searchInput))
            .ToListAsync();
    }
}
