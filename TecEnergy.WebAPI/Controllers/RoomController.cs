using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;
using TecEnergy.Database.Repositories.Interfaces;
using TecEnergy.WebAPI.Helpers;
using TecEnergy.WebAPI.Mapping;
using TecEnergy.WebAPI.Services;

namespace TecEnergy.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly RoomService _service;

    public RoomController(RoomService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetAllAsync()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    //SimpleDTO
    [HttpGet("{id}")]
    public async Task<ActionResult<SimpleDto>> GetByIdAsync(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("SimpleList/{buildingId}")]
    public async Task<ActionResult<List<SimpleDto>>> GetSimpleRoomListByBuildingId(Guid buildingId)
    {
        var result = await _service.GetSimpleListByBuildingIdAsync(buildingId);
        if (result is null) return NotFound();
        return Ok(result);
    }

    //EnergyDTO
    [HttpGet("EnergyDto/{id}")]
    public async Task<ActionResult<EnergyDto>> GetByIdWithEnergyMetersAsync(Guid id)
    {
        var startDateTime = DateTime.UtcNow;
        var result = await _service.GetEnergyDtoAsync(id, startDateTime.AddSeconds(-60), startDateTime);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("EnergyMeterListDto/{roomId}")]
    public async Task<List<EnergyDto>> GetEnergyMeterListByRoomId(Guid roomId)
    {
        var startDateTime = DateTime.UtcNow;
        var result = await _service.GetEnergyMeterListDtoByRoomId(roomId, startDateTime.AddSeconds(-60), startDateTime);
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<Room>> CreateAsync(Room room)
    {
        if (room.BuildingID == Guid.Empty) return BadRequest("Missing Building Id.");
        //if (await _buildingRepository.GetByIdAsync(room.BuildingID) is null) return NotFound("Building Id for room does not exists");
        await _service.AddAsync(room);
        return Ok(room);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, Room updateResource)
    {
        if (id != updateResource.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var existingEnergyMeter = await _service.GetByIdAsync(id);
        if (existingEnergyMeter is null) return NotFound("Room Not Found");
        await _service.UpdateAsync(id, updateResource);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("roomId")]
    public async Task<ActionResult<GuidDto>> GetFirstRoomIdAsync()
    {
        GuidDto result = new();
        var room = await _service.GetFirstRoomAsync();
        result.Id = room.Id;
        return Ok(result);
    }


    // 2023-12-11T13:00:00

    [HttpGet("TimeInterval/{roomId}/{startTime}/{endTime}")]
    public async Task<IActionResult> GetEnergyDataByTimeIntervalAsync(Guid roomId, DateTime startTime, DateTime endTime)
    {
        // validate inputs
        var result = await _service.GetAccumulatedEnergyForARoom(roomId, startTime, endTime);

        List<DailyAccumulatedDto> dataList = new();

        foreach (var item in result)
        {
            DailyAccumulatedDto temp = new();
            temp.DailyAccumulatedValue = item.DailyAccumulatedValue;
            temp.DateTime = item.DateTime;
            dataList.Add(temp);
        }

        // validate result
        // change result to dto
        // return dto as actionresult
        return Ok(dataList);
    }


    [HttpGet("YearlyAccumulation/{roomId}/{year}")]
    public async Task<IActionResult> GetYearlyAccumulationAsync(Guid roomId, DateOnly year)
    {
        // converted to dto in repository instead of here

        var result = await _service.GetYearlyAccumulation(roomId, year);
        return Ok(result);
    }

    [HttpGet("AllYearData/{roomId}")]
    public async Task<IActionResult> GetAllYearDataAsync(Guid roomId)
    {
        var result = await _service.GetAllYearData(roomId);
        return Ok(result);
    }
}
