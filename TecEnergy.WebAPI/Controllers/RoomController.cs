using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecEnergy.Database.DataModels;
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

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Room>> CreateAsync(Room room)
    {
        if (room.BuildingID == Guid.Empty) return BadRequest("Missing Building Id.");
        var building = await _buildingRepository.GetByIdAsync(room.BuildingID);
        //if (!await _buildingRepository.(room.BuildingID)) return NotFound("Building not found.");

        await _repository.AddAsync(room);
        return CreatedAtAction("GetBuilding", new { id = room.Id }, room);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, Room updateResource)
    {
        if (id != updateResource.Id)
        {
            return BadRequest();
        }

        await _repository.UpdateAsync(updateResource);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
