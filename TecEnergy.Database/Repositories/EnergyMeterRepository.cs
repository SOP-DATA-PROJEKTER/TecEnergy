using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;
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


    public async Task<EnergyMeter> GetByIdDatetimeAsync(Guid id, DateTime startDate, DateTime endTime)
    {
        EnergyMeter energyMeter = new();
        var meter = await _context.EnergyMeters.Include(x => x.EnergyDatas).Where(x => x.Id == id).FirstOrDefaultAsync();
        var datemeter = meter.EnergyDatas.Where(x => x.DateTime > startDate && x.DateTime < endTime).ToList();
        energyMeter = meter;
        energyMeter.EnergyDatas = datemeter;
        return energyMeter;
    }
    public async Task<EnergyMeter> GetByIdWithDataAsync(Guid id)
    {
        return await _context.EnergyMeters
            .Include(x => x.EnergyDatas)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(EnergyMeter energyMeter)
    {
        await _context.EnergyMeters.AddAsync(energyMeter);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EnergyMeter energyMeter)
    {
        _context.Entry(energyMeter).State = EntityState.Modified;
        //_context.Update(energyMeter);
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
    public async Task<IEnumerable<EnergyMeter>> SearchAsync(string searchInput)
    {
        return await _context.EnergyMeters
            .Where(x => x.MeasurementPointName.Contains(searchInput) || x.MeasurementPointComment.Contains(searchInput))
            .ToListAsync();
    }
}
