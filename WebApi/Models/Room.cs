namespace WebApi.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public Building Building { get; set; }
        public List<Guid>? MeterId { get; set; }
        public List<EnergyMeter>? EnergyMeters { get; set; }
        public string Name { get; set; }
    }
}
