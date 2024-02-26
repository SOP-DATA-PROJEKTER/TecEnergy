using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnergyDataController : ControllerBase
    {
        private readonly IEnergyDataRepository _energyDataRepository;
        public EnergyDataController(IEnergyDataRepository energyDataRepository)
        {
            _energyDataRepository = energyDataRepository;
        }

        // need to decide what data is actually required for this endpoint
        // we need to pass the energymeterId and accumulation but what about dateTime?
        //
        // we post every 10 seconds if there is something to post. but do we need the esp32 to also log the dateTime when an impulse is detected?
        // if so we can't get the miliseconds from the esp32 so it can only be dateOnly we recieve, unless we add a timestamp on the esp32's epoch time.
        // but this is inaccurate since it is from the moment the esp32 is turned on and not the real value, but then again does it matter to be this precise? can we make do with seconds only?
        // we have to take this into account aswell as the minimum impulse time of 80 ms.
        // we also need to decide if we want to post the accumulation or if we want to calculate it on the server side.
        //
        // we will calculate the accumulation on the esp32 and post it to the server.
        // we will return statuscode 201 created with no content
        // 
        // for now we will only post the energymeterId and the accumulation and generate the datetime on this api side.
        // this way we will only get dateTime values every 10 seconds maximum.
        //

        [HttpPost]
        public async Task<IActionResult> CreateEnergyData([FromBody] List<EspDataDto> espData)
        {
            if (espData == null)
                return BadRequest();

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!await _energyDataRepository.CreateData(espData))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save data");
            }
            return Created(); // returns 204

        }
    }
}
