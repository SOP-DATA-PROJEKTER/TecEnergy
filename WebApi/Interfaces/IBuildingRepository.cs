using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IBuildingRepository
    {
        Task<Building> GetBuildingAsync(Guid id);
        Task<Building> CreateBuildingAsync(string name);
        Task<List<Building>> GetAllBuildingsAsync();
        //Task<Building> DeleteBuildingAsync(Guid id);
        //Task<Building> UpdateBuildingAsync(Building building);
    }
}
