namespace WebApi.Dtos
{
    public class SimpleInfoDto
    {
        public Guid roomId { get; set; }
        public List<Guid> meterIds{ get; set; }
        public string roomName { get; set; }
    }
}
