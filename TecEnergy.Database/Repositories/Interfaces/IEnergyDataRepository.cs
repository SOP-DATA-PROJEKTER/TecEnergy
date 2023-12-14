using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecEnergy.Database.Models.DataModels;

namespace TecEnergy.Database.Repositories.Interfaces;
public interface IEnergyDataRepository
{
    Task<IEnumerable<EnergyData>> GetAllAsync();
    Task<EnergyData> GetByIdAsync(Guid id);
    Task AddAsync(EnergyData building);
    Task UpdateAsync(EnergyData building);
    Task DeleteAsync(Guid id);
    Task<EnergyData> GetLatestEnergyDataAsync(Guid energyMeterId);
}
