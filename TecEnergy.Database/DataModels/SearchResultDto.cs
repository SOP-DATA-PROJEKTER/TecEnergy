using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecEnergy.Database.DataModels;
public class SearchResultDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ModelType ModelType { get; set; }
}

public enum ModelType
{
    Building,
    Room,
    EnergyMeter
} 
