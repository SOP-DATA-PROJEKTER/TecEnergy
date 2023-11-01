using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecEnergy.Database.DataModels;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EnergyDataController : ControllerBase
{
    private readonly IEnergyDataRepository _repository;

    public EnergyDataController(IEnergyDataRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnergyData>>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnergyData>> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<EnergyData>> CreateAsync(EnergyData createResource)
    {
        await _repository.AddAsync(createResource);
        return CreatedAtAction("GetBuilding", new { id = createResource.Id }, createResource);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, EnergyData updateResource)
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
