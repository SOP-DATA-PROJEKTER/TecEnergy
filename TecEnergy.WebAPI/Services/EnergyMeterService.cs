using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;
using TecEnergy.Database.Repositories.Interfaces;
using TecEnergy.WebAPI.Helpers;
using TecEnergy.WebAPI.Mapping;

namespace TecEnergy.WebAPI.Services;

public class EnergyMeterService
{
    private readonly IEnergyMeterRepository _repository;
    public EnergyMeterService(IEnergyMeterRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EnergyMeter>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        return result;
    }
    public async Task<EnergyMeter> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        return result;
    }
    public async Task<EnergyDto> GetByIdDatetimeAsync(Guid id, DateTime startDateTime, DateTime endDateTime)
    {
        var result = await _repository.GetByIdWithDataAsync(id);

        var energyDataDateTimeFilter = result.EnergyDatas
            .Where(x => x.DateTime > startDateTime && x.DateTime < endDateTime)
            .ToList();

        var hoursInDouble = CalculationHelper.CalculateHoursToDouble(startDateTime, endDateTime);
        var realtime = CalculationHelper.GetKilowattsInHours(energyDataDateTimeFilter.Count, hoursInDouble);

        var accumulatedStartDateTime = result.EnergyDatas.Min(x => x.DateTime);
        var accumulatedEndDateTime = result.EnergyDatas.Max(x => x.DateTime);
        var hoursInDoubleAcc = CalculationHelper.CalculateHoursToDouble(accumulatedStartDateTime, accumulatedEndDateTime);

        var accumulated = CalculationHelper.GetKilowattsInHours(result.EnergyDatas.Count, hoursInDoubleAcc);

        var energyDto = EnergyMeterMappings.EnergyMeterToEnergyDto(result, realtime, accumulated);

        return energyDto;
    }
    public async Task<EnergyMeter> GetByIdWithDataAsync(Guid id)
    {
        var result = await _repository.GetByIdWithDataAsync(id);
        return result;
    }



    public async Task CreateAsync(EnergyMeter energyMeter)
    {
        await _repository.AddAsync(energyMeter);
    }

    public async Task UpdateAsync(Guid id, EnergyMeter energyMeter)
    {
        await _repository.UpdateAsync(energyMeter);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
