using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TecEnergy.Database.Models.DataModels;
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
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("latest")]
    public async Task<ActionResult<EnergyData>> GetLatestEnergyData(Guid energyMeterId)
    {
        var result = await _repository.GetLatestEnergyDataAsync(energyMeterId);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    //public async Task<ActionResult<EnergyData>> CreateAsync(EnergyData energyData)
    //{
    //    if (energyData.EnergyMeterID == Guid.Empty) return BadRequest("Missing Energy Meter Id.");
    //    //another check here missing for if the energymeter exists
    //    await _repository.AddAsync(energyData);
    //    return Ok(energyData);
    //}
    public async Task<ActionResult<IEnumerable<EnergyData>>> CreateAsync([FromBody] IEnumerable<EnergyData> energyDataList)
    {
        if (energyDataList == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        foreach (var energyData in energyDataList)
        {
            if (energyData.EnergyMeterID == Guid.Empty)
            {
                ModelState.AddModelError(nameof(EnergyData.EnergyMeterID), "Missing Energy Meter Id.");
                return BadRequest(ModelState);
            }

            // Another check here missing for if the energymeter exists
            await _repository.AddAsync(energyData);
        }

        return Ok(energyDataList);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, EnergyData updateResource)
    {
        if (id != updateResource.Id) return BadRequest();
        await _repository.UpdateAsync(updateResource);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }


    [HttpPost("Test")]
    public async Task<IActionResult> TestPostAsync([FromBody] Test any)
    {
        return Ok(any);
    }



}
