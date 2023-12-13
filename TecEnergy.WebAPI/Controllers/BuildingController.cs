using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;
using TecEnergy.Database.Repositories.Interfaces;
using TecEnergy.WebAPI.Services;

namespace TecEnergy.WebAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BuildingController : ControllerBase
{
    private readonly BuildingService _service;

    public BuildingController(BuildingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Building>>> GetAllAsync()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<SimpleDto>> GetByIdAsync(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    //Supposed to have two nullable datetimes (startDateTime and endDateTime) for filtering data with timeinterval and if nullable set default value
    [HttpGet("EnergyDto/{id}")]
    public async Task<ActionResult<EnergyDto>> GetByIdWithRoomsAsync(Guid id)
    {
        var startDateTime = DateTime.UtcNow;
        var result = await _service.GetBuildingEnergyDtoAsync(id, startDateTime, startDateTime.AddSeconds(-60));

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Building>> CreateAsync(Building createResource)
    {
        await _service.AddAsync(createResource);
        //return CreatedAtAction("GetBuilding", new { id = createResource.Id }, createResource);
        return Ok(createResource);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, Building updateResource)
    {
        if (id != updateResource.Id)
        {
            return BadRequest();
        }

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