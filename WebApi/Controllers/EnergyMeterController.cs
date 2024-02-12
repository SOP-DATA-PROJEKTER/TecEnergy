using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateEnergyMeter()
        {
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyGraphData()
        {
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyGraphData()
        {
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetYearlyGraphData()
        {
            return NoContent();
        }
    }
}
