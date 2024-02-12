namespace WebApi.Models
{
    public class EnergyMeter
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Room Room { get; set; }

    }
}
