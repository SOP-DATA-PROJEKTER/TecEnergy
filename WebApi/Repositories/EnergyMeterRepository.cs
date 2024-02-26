using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;
namespace WebApi.Repositories
{
    public class EnergyMeterRepository : IEnergyMeterRepository
    {
        private readonly DatabaseContext _context;


        public EnergyMeterRepository(DatabaseContext context)
        {
            _context = context;
        }


        public async Task<EnergyMeter> CreateAsync(SimpleInfoDto data)
        {
            EnergyMeter energyMeter = new EnergyMeter
            {
                Id = Guid.NewGuid(),
                RoomId = data.Id,
                Name = data.Name,
                IsConnected = true
            };

            await _context.EnergyMeters.AddAsync(energyMeter);
            if(await _context.SaveChangesAsync() > 0)
            {
                return energyMeter;
            }

            throw new Exception("Failed to save data");
        }


        public Task<bool> EnergyMeterExists(SimpleInfoDto data)
        {
            return _context.EnergyMeters
                .AnyAsync(x => x.RoomId == data.Id && x.Name == data.Name);
        }


        public async Task<EnergyMeter> GetEnergyMeter(Guid id)
        {
            return await _context.EnergyMeters
                .Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("No data found");
        }


        public Task<bool> RoomExists(Guid roomId)
        {
            return _context.Rooms.AnyAsync(x => x.Id == roomId);
        }


    }
}
