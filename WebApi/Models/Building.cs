namespace WebApi.Models
{
    public class Building
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid>? RoomId { get; set; }
        public List<Room>? Rooms { get; set; }
    }
}
