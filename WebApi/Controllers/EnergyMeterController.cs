using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnergyMeterController : ControllerBase
    {
        private readonly IEnergyMeterRepository _energyMeterRepository;


        public EnergyMeterController(IEnergyMeterRepository energyMeterRepository)
        {
            _energyMeterRepository = energyMeterRepository;
        }


        [HttpPost]
        public async Task<IActionResult> CreateEnergyMeter(SimpleInfoDto data)
        {
            // create a energyMeter Object for a room and return the new energyMeterId
            if (data == null)
                return BadRequest("Data is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // check if the room exists
            if (!await _energyMeterRepository.RoomExists(data.Id))
                return BadRequest("Room does not exist");

            // check if the energyMeter already exists in that room
            if (await _energyMeterRepository.EnergyMeterExists(data))
                return BadRequest($"EnergyMeter {data.Name} already exists in that room");

            try
            {
                var result = await _energyMeterRepository.CreateAsync(data);
                return CreatedAtAction(nameof(GetMeter), new { id = result.Id}, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetMeter(Guid id)
        {
            try
            {
                return Ok( await _energyMeterRepository.GetEnergyMeter(id));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
