using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IBuildingRepository
    {
        Task<Building> GetBuildingAsync(Guid id);
        Task<Building> CreateBuildingAsync();
        //Task<Building> DeleteBuildingAsync(Guid id);
        //Task<Building> UpdateBuildingAsync(Building building);
    }
}
