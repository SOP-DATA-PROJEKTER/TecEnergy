using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecEnergy.Database.DataModels;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.Database.Repositories;
public class EnergyMeterRepository : IEnergyMeterRepository
{
    private readonly DatabaseContext _context;

    public EnergyMeterRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EnergyMeter>> GetAllAsync()
    {
        return await _context.EnergyMeters.ToListAsync();
    }

    public async Task<EnergyMeter> GetByIdAsync(Guid id)
    {
        return await _context.EnergyMeters.FindAsync(id);
    }

    public async Task AddAsync(EnergyMeter energyMeter)
    {
        _context.EnergyMeters.Add(energyMeter);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EnergyMeter energyMeter)
    {
        //_context.Entry(energyMeter).State = EntityState.Modified;
        _context.Update(energyMeter);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var energyMeter = await _context.EnergyMeters.FindAsync(id);
        if (energyMeter is not null)
        {
            _context.EnergyMeters.Remove(energyMeter);
            await _context.SaveChangesAsync();
        }
    }
}
