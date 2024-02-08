using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;
using TecEnergy.Database.Repositories.Interfaces;

namespace TecEnergy.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EnergyDataController : ControllerBase
{
    
    // why is this not consistent with the other controllers, here we directly inject the repository but in the other controllers we inject services.

    private readonly IEnergyDataRepository _repository;

    public EnergyDataController(IEnergyDataRepository repository)
    {
        _repository = repository;
    }


    // doubt getting all data will be useful
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnergyData>>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        return Ok(result);
    }


    // doubt this will get used
    [HttpGet("{id}")]
    public async Task<ActionResult<EnergyData>> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }


    // we can use this to initialize the esp32 if a crash occurs, get the last value and set it to the accumulatedvalue
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
    public async Task<ActionResult<IEnumerable<EnergyDataDto>>> CreateAsync([FromBody] IEnumerable<EnergyDataDto> energyDataList)
    {
        if (energyDataList == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        foreach (var energyData in energyDataList)
        {
            if (energyData.EnergyMeterID == Guid.Empty)
            {
                ModelState.AddModelError(nameof(EnergyDataDto.EnergyMeterID), "Missing Energy Meter Id.");
                return BadRequest(ModelState);
            }

            // Another check here missing for if the energymeter exists
            await _repository.AddAsync(energyData);
        }

        //return Ok(energyDataList);
        // instead of returning ok, we should return the created status code 201.
        return Created("~api/EnergyData", energyDataList);
    }


    // should be deleted, only used for testing without modifying the database
    [HttpPost("Test")]
    public async Task<IActionResult> TestPostAsync([FromBody] List<Test> any)
    {
        return CreatedAtAction(nameof(TestPostAsync), any);
    }


    // put is for replace data with a new one. so we first delete the old one and then add the new one
    // seems to me this shuld have been a patch since we update the data.
    // not sure how useful this even is.
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

}
