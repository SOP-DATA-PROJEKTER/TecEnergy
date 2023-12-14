using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.Database.Repositories;
public class EnergyDataRepository : IEnergyDataRepository
{
    private readonly DatabaseContext _context;
    public EnergyDataRepository(DatabaseContext context) => _context = context;

    public async Task<IEnumerable<EnergyData>> GetAllAsync()
    {
        return await _context.EnergyData.ToListAsync();
    }

    public async Task<EnergyData> GetByIdAsync(Guid id)
    {
        return await _context.EnergyData.FindAsync(id);
    }

    //For the simulation to get the highest datetime and read the accumulatedvalue to set the appsettings configuration
    public async Task<EnergyData> GetLatestEnergyDataAsync(Guid energyMeterId)
    {
        return await _context.EnergyData.OrderBy(x => x.AccumulatedValue).Where(x => x.EnergyMeterID == energyMeterId).LastOrDefaultAsync();
        //return await _context.EnergyData.OrderBy(x => x.AccumulatedValue).LastOrDefaultAsync();
    }

    public async Task AddAsync(EnergyData energyData)
    {
        energyData.DateTime = DateTimeOffset.UtcNow.UtcDateTime;
        await _context.EnergyData.AddAsync(energyData);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EnergyData energyData)
    {
        _context.Entry(energyData).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var energyData = await _context.EnergyData.FindAsync(id);
        if (energyData is not null)
        {
            _context.EnergyData.Remove(energyData);
            await _context.SaveChangesAsync();
        }
    }
}
