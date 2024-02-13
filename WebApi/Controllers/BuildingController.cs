using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IBuildingRepository _buildingRepository;
        public BuildingController(IBuildingRepository buildingRepository)
        {
            _buildingRepository = buildingRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBuilding(Guid id)
        {

            if(id == Guid.Empty)
            {
                return BadRequest("Id cannot be empty");
            }

            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid model state");
            }

            try
            {
                var result = await _buildingRepository.GetBuildingAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
 
        }

        [HttpPost("{BuildingName}")]
        public async Task<IActionResult> CreateBuilding(string BuildingName)
        {
            try
            {
                var result = await _buildingRepository.CreateBuildingAsync(BuildingName);
                return CreatedAtAction(nameof(GetBuilding), result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBuildings()
        {
            return NoContent();
        }


    }
}
