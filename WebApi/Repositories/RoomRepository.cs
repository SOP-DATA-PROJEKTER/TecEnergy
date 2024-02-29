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
            // find meters that has the roomId of the parameter

            var metersInRoom = await _context.EnergyMeters
                .Include(x => x.EnergyDatas)
                .Where(x => x.RoomId == roomId).OrderBy(x => x.Name)
                .ToListAsync() ?? throw new Exception("No meters found");

            // then find the meter data of each meter and store in a list of meterDataDto

            DateTime start = DateTime.Now;
            var end = start;
            start = start.AddSeconds(-60);
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

                //var meterData = await _context.EnergyData
                //    .Where(x => x.EnergyMeterId == meter.Id && x.DateTime >= start && x.DateTime <= end)
                //    .OrderByDescending(x => x.DateTime)
                //    .ToListAsync();


                var meterData = meter.EnergyDatas
                    .Where(x => x.DateTime >= start && x.DateTime <= end)
                    .OrderByDescending(x => x.DateTime)
                    .ToList();

                // handle no values from meterdata (no data from the last 60 seconds)
                if (meterData.Count == 0)
                {
                    // since there is no meterData we have to get the last accumulated value from the meter look at all time, instead of last 60 seconds
                    var meterDataWasZero = meter.EnergyDatas
                        .Where(x => x.EnergyMeterId == meter.Id)
                        .OrderByDescending(x => x.DateTime)
                        .FirstOrDefault();


                    // if no data was found in a table,
                    // then the accumulated value should be set to 0 to avoid null reference exception
                    meterDataWasZero ??= new EnergyData
                    {
                        AccumulatedValue = 0
                    };

                    subMeters.Add(new MeterDataDto
                    {
                        Id = meter.Id,
                        Name = meter.Name,
                        RealTime = 0,
                        Accumulated = meterDataWasZero.AccumulatedValue,
                        IsConnected = true
                    });
                    // break the loop and continue to the next meter

                    mainMeter.Accumulated += meterDataWasZero.AccumulatedValue;
                    continue;
                }

                double realtime = meterData.Count * 60f / 1000f;


                subMeters.Add(new MeterDataDto
                {
                    Id = meter.Id,
                    Name = meter.Name,
                    RealTime = realtime,
                    Accumulated = meterData.First().AccumulatedValue,
                    IsConnected = true
                });

                mainMeter.RealTime += realtime; // the same as getting all the counts of meterData and multiplying it by 60 and dividing by 1000,
                                                // but here we are just adding the realtime of each meter resulting in the same value
                mainMeter.Accumulated += meterData.First().AccumulatedValue;



            }

            return new RoomDataDto
            {
                MainMeter = mainMeter,
                SubMeters = subMeters
            };


        }


    }
}
