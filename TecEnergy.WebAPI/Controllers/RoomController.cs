using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;
using TecEnergy.Database.Repositories.Interfaces;
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
    public async Task<ActionResult<EnergyDto>> GetByIdWithEnergyMetersAsync(Guid id, DateTime? startDateTime, DateTime? endDateTime)
    {
        if (endDateTime == null && startDateTime == null) endDateTime = DateTime.UtcNow; startDateTime = endDateTime.Value.AddSeconds(-60);
        var result = await _service.GetEnergyDtoAsync(id, startDateTime, endDateTime);
        if (result is null) return NotFound();
        return Ok(result);
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
}
