using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> CreateEnergyData()
        {
            return NoContent();
        }
    }
}
