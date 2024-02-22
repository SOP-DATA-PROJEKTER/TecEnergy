namespace WebApi.Dtos
{
    public class RoomDataDto
    {
        // main meter
        public MeterDataDto MainMeter { get; set; }
        // sub meters
        public List<MeterDataDto> SubMeters { get; set; }
    }
}
