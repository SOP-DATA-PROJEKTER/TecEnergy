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
        public Task<Building> CreateBuildingAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Building> GetBuildingAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
