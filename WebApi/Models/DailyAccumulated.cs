namespace WebApi.Models
{
    public class DailyAccumulated
    {
        public Guid Id { get; set; }
        public Guid EnergyMeterId { get; set; }
        public EnergyMeter EnergyMeter { get; set; }
        public DateTime DateTime { get; set; }
        public long AccumulatedValue { get; set; }
    }
}
