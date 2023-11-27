using Microsoft.OpenApi.Services;
using System.ComponentModel;
using TecEnergy.Database.DataModels;
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

    public async Task<List<SearchResultDto>> PerformSearch(string searchInput)
    {
        var combinedResults = new List<SearchResultDto>();

        //mapping building results
        combinedResults.AddRange(await MapToSearchResultsAsync(_buildingRepository.SearchAsync(searchInput), x => new SearchResultDto
        {
            Id = x.Id,
            Name = x.BuildingName,
            ModelType = ModelType.Building
        }));

        //mapping room results
        combinedResults.AddRange(await MapToSearchResultsAsync(_roomRepository.SearchAsync(searchInput), x => new SearchResultDto
        {
            Id = x.Id,
            Name = x.RoomName,
            ModelType = ModelType.Room
        }));

        //mapping energy meter results
        combinedResults.AddRange(await MapToSearchResultsAsync(_energyMeterRepository.SearchAsync(searchInput), x => new SearchResultDto
        {
            Id = x.Id,
            Name = x.MeasurementPointName,
            ModelType = ModelType.EnergyMeter
        }));

        return combinedResults;
    }

    //mapping searchresults to SearchResultDto
    private async Task<List<SearchResultDto>> MapToSearchResultsAsync<T>(Task<IEnumerable<T>> sourceTask, Func<T, SearchResultDto> mapFunction)
    {
        var source = await sourceTask;
        //if source has elements select else return null list 
        return source.Any() ? source.Select(mapFunction).ToList() : new List<SearchResultDto>();
    }
}
