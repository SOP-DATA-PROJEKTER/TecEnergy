using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecEnergy.Database.DataModels;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EnergyMeterController : ControllerBase
{
    private readonly IEnergyMeterRepository _repository;

    public EnergyMeterController(IEnergyMeterRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnergyMeter>>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnergyMeter>> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<EnergyMeter>> CreateAsync(EnergyMeter energyMeter)
    {
        if (energyMeter is null) return BadRequest("Invalid input data.");
        await _repository.AddAsync(energyMeter);
        //return CreatedAtAction(nameof(GetByIdAsync), new { id = energyMeter.Id }, energyMeter);
        return Ok(energyMeter);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, EnergyMeter updateResource)
    {
        if (id != updateResource.Id) return BadRequest();
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
