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
        var result = await _repository.GetByIdDatetimeAsync(id, startDateTime, endDateTime);
        var energyDataAmount = result.EnergyDatas.Count();

        var hoursInDouble = CalculationHelper.CalculateHoursToDouble(startDateTime, endDateTime);
        var realtime = CalculationHelper.GetRealTimeKilowattsInHours(energyDataAmount, hoursInDouble);
        //Func<DateTime, double> powerFunction = time => 50.0 * (time - DateTime.Now).TotalSeconds + 20.0;
        //var accumulated = CalculationHelper.GetAccumulatedKilowattsInHours(powerFunction, startDateTime, endDateTime);
        var energyDateAccumulated = GetByIdWithDataAsync(id).Result;
        var energyDateAccumulatedCount = energyDateAccumulated.EnergyDatas.Count();
        var accumulatedStartDateTime = energyDateAccumulated.EnergyDatas.OrderBy(x => x.AccumulatedValue).First().DateTime;
        var accumulatedEndDateTime = energyDateAccumulated.EnergyDatas.OrderBy(x => x.AccumulatedValue).Last().DateTime;
        var hoursInDoubleAcc = CalculationHelper.CalculateHoursToDouble(accumulatedStartDateTime, accumulatedEndDateTime);
        var accumulated = CalculationHelper.GetRealTimeKilowattsInHours(energyDateAccumulatedCount, hoursInDoubleAcc);


        var energyDto = EnergyMeterMappings.EnergyMeterToEnergyDto(result, realtime, accumulated);
        return energyDto;
        //return result;
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
