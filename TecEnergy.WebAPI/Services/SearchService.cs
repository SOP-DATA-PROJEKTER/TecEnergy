using Microsoft.OpenApi.Services;
using System.ComponentModel;
using TecEnergy.Database.Repositories;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.WebAPI.Services;

public class SearchService
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IEnergyMeterRepository _energyMeterRepository;

    public SearchService(
        IBuildingRepository buildingRepository,
        IRoomRepository roomRepository,
        IEnergyMeterRepository energyMeterRepository)
    {
        _buildingRepository = buildingRepository;
        _roomRepository = roomRepository;
        _energyMeterRepository = energyMeterRepository;
    }

    public async Task<List<SearchResult>> PerformSearch(string searchInput)
    {
        var buildingResults = await _buildingRepository.SearchAsync(searchInput);
        var roomResults = await _roomRepository.SearchAsync(searchInput);
        var energyMeterResults = await _energyMeterRepository.SearchAsync(searchInput);

        var combinedResults = new List<SearchResult>();
        combinedResults.AddRange((IEnumerable<SearchResult>)buildingResults);
        combinedResults.AddRange((IEnumerable<SearchResult>)roomResults);
        combinedResults.AddRange((IEnumerable<SearchResult>)energyMeterResults);

        return combinedResults;
    }
}
