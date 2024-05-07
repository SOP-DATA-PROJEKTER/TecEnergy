namespace WebApi.Dtos
{
    public class EspDataDto
    {
        public Guid MeterId { get; set; }
        public string DateTime { get; set; }
        public long AccumulatedValue { get; set; }
    }
}
