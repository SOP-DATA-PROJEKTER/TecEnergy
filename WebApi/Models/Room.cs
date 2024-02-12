namespace WebApi.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public Building Building { get; set; }
        public string Name { get; set; }
    }
}
