namespace WebApi.Dtos
{
    public class MeterDataDto
    {
        public Guid meterId { get; set; }
        public DateTime dateTime { get; set; }
        public long accumulatedValue { get; set; }
    }
}
