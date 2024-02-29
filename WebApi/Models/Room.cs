namespace WebApi.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<EnergyMeter>? EnergyMeters { get; set; }
    }
}
