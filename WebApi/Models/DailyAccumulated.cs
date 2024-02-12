namespace WebApi.Models
{
    public class DailyAccumulated
    {
        public Guid Id { get; set; }
        public Guid MeterId { get; set; }
        public EnergyMeter EnergyMeter { get; set; }
        public DateTime Date { get; set; }
        public long AccumulatedValue { get; set; }

    }
}
