using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _repository;
    private readonly IBuildingRepository _buildingRepository;

    public RoomController(IRoomRepository repository, IBuildingRepository buildingRepository)
    {
        _repository = repository;
        _buildingRepository = buildingRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("WithEnergyMeters/{id}")]
    public async Task<ActionResult<Room>> GetByIdWithEnergyMetersAsync(Guid id)
    {
        var result = await _repository.GetByIdWithEnergyMetersAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Room>> CreateAsync(Room room)
    {
        if (room.BuildingID == Guid.Empty) return BadRequest("Missing Building Id.");
        if (await _buildingRepository.GetByIdAsync(room.BuildingID) is null) return NotFound("Building Id for room does not exists");
        await _repository.AddAsync(room);
        return Ok(room);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, Room updateResource)
    {
        if (id != updateResource.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var existingEnergyMeter = await _repository.GetByIdAsync(id);
        if (existingEnergyMeter is null) return NotFound("Room Not Found");
        await _repository.UpdateAsync(updateResource);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
