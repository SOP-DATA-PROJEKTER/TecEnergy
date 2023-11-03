using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecEnergy.Database.DataModels;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.WebAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BuildingController : ControllerBase
{
    private readonly IBuildingRepository _repository;

    public BuildingController(IBuildingRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Building>>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Building>> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Building>> CreateAsync(Building createResource)
    {
        await _repository.AddAsync(createResource);
        //return CreatedAtAction("GetBuilding", new { id = createResource.Id }, createResource);
        return Ok(createResource);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, Building updateResource)
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