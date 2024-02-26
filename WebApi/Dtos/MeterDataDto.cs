namespace WebApi.Dtos
{
    public class MeterDataDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double RealTime { get; set; }
        public long Accumulated { get; set; }
        public bool IsConnected { get; set; }
    }
}
