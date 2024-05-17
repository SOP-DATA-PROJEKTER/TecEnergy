using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly DatabaseContext _context;
        private readonly double JoulesPerImpulse = 3600.0;


        public RoomRepository(DatabaseContext context)
        {
            _context = context;
        }


        public async Task<Room> CreateAsync(string name)
        {
            Room room = new Room
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            await _context.Rooms.AddAsync(room);
            if (await _context.SaveChangesAsync() > 0)
            {
                return room;
            }
            throw new Exception("Failed to save data");

        }


        public async Task<Room> GetRoomByIdAsync(Guid id)
        {
            return await _context.Rooms
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("No Data Found");
        }


        public async Task<bool> GetRoomByNameAsync(string name)
        {
            var result = await _context.Rooms
                .FirstOrDefaultAsync(x => x.Name == name);

            return result != null;
        }


        public async Task<ICollection<SimpleInfoDto>> GetSimpleInfoAsync()
        {
            return await _context.Rooms
                .Select(x => new SimpleInfoDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync() ?? throw new Exception("No Data Found");
        }


        public async Task<RoomDataDto> GetRoomDataAsync(Guid roomId)
        {
            var metersInRoom = await _context.EnergyMeters
                .Include(x => x.EnergyDatas)
                .Where(x => x.RoomId == roomId)
                .OrderBy(x => x.Name)
                .ToListAsync();

            if (metersInRoom == null || metersInRoom.Count == 0)
            {
                throw new Exception("No meters found");
            }

            DateTime end = DateTime.Now;
            DateTime start = end.AddSeconds(-60);

            await Console.Out.WriteLineAsync("start: " + start);
            await Console.Out.WriteLineAsync("end: " + end);

            List<MeterDataDto> subMeters = [];
            MeterDataDto mainMeter = new()
            {
                Id = Guid.NewGuid(), // change this since it is a fake guid that will not be used
                Name = "Main Meter",
                RealTime = 0,
                Accumulated = 0,
                IsConnected = true
            };

            foreach (var meter in metersInRoom)
            {
                var meterData = meter.EnergyDatas
                    .Where(x => x.DateTime >= start && x.DateTime <= end)
                    .OrderBy(x => x.DateTime)
                    .ToList();

                if (meterData.Count == 0)
                {
                    // If no data within the time window, use the latest available data
                    var latestMeterData = meter.EnergyDatas
                        .OrderByDescending(x => x.DateTime)
                        .FirstOrDefault();

                    if (latestMeterData != null)
                    {
                        subMeters.Add(new MeterDataDto
                        {
                            Id = meter.Id,
                            Name = meter.Name,
                            RealTime = 0,
                            Accumulated = latestMeterData.AccumulatedValue,
                            IsConnected = true
                        });

                        mainMeter.Accumulated += latestMeterData.AccumulatedValue;
                    }

                    continue; // Skip further processing for this meter
                }

                // Calculate current wattage based on impulses and timestamps
                double currentWattage = CalculateCurrentWattage(meterData);

                subMeters.Add(new MeterDataDto
                {
                    Id = meter.Id,
                    Name = meter.Name,
                    RealTime = currentWattage,
                    Accumulated = meterData.Last().AccumulatedValue,
                    IsConnected = true
                });

                mainMeter.RealTime += currentWattage;
                mainMeter.Accumulated += meterData.Last().AccumulatedValue;
            }

            return new RoomDataDto
            {
                MainMeter = mainMeter,
                SubMeters = subMeters
            };
        }

        // Calculate current wattage based on impulses and timestamps
        // Power(W) = Energy per Pulse(J) / Time between Pulses(s)
        private double CalculateCurrentWattage(List<EnergyData> energyData)
        {
            if (energyData.Count < 2 || energyData.Any(data => data == null))
                return 0.0;

            var lastTwoDataPoints = energyData.TakeLast(2).ToList();

            double impulsesPerSecond = CalculateImpulsesPerSecond(lastTwoDataPoints);

            // Joules per impulse is 3600 since the meter measures in kWh
            return impulsesPerSecond * JoulesPerImpulse;
        }

        // Finds the time difference between the first and last timestamp
        private double CalculateImpulsesPerSecond(List<EnergyData> dataPoints)
        {
            var validDataPoints = dataPoints.Where(data => data != null).ToList();

            if (validDataPoints.Count < 2)
                return 0.0;

            TimeSpan timeDifference = validDataPoints.Last().DateTime - validDataPoints.First().DateTime;

            if (timeDifference <= TimeSpan.Zero)
                return 0.0;

            return (validDataPoints.Last().AccumulatedValue - validDataPoints.First().AccumulatedValue) / timeDifference.TotalSeconds;
        }
    }
}
