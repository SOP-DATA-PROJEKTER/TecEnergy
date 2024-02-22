namespace WebApi.Models
{
    public class EnergyData
    {
        public Guid Id { get; set; }
        public Guid EnergyMeterId { get; set; }
        public EnergyMeter EnergyMeter { get; set; }
        public DateTime DateTime { get; set; }
        public long AccumulatedValue { get; set; }
    }
}
