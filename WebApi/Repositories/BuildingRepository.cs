using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly DatabaseContext _context;
        public BuildingRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<Building> CreateBuildingAsync(string name)
        {
            Building building = new Building
            {
                Id = Guid.NewGuid(),
                Name = name,
            };
            await _context.Buildings.AddAsync(building);

            if(await _context.SaveChangesAsync() > 0)
            {
                return building;
            }
            throw new Exception("Failed to save building");
            
        }

        public async Task<List<Building>> GetAllBuildingsAsync()
        {
            return await _context.Buildings.ToListAsync() ?? throw new Exception("No buildings found");
        }

        public async Task<Building> GetBuildingAsync(Guid id)
        {

            // if result is null, throw an exception else return the result
            return await _context.Buildings.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("Building not found");
        }
    }
}
