using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;
using TecEnergy.Database.Repositories.Interfaces;

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

    //public async Task<EnergyDto> GetByIdDatetimeAsync(Guid id, DateTime startDate, DateTime endTime)
    //{
    //    var result = await _repository.GetByIdDatetimeAsync(id, startDate, endTime);
        
    //    //return result;
    //}

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
