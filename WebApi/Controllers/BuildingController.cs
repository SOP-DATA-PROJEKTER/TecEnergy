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

        [HttpPost]
        public async Task<IActionResult> CreateBuilding()
        {
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetBuildings()
        {
            return NoContent();
        }


    }
}
