using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphController : ControllerBase
    {
        private readonly IEnergyMeterRepository _energyMeterRepository;
        public GraphController(IEnergyMeterRepository energyMeterRepository)
        {
            _energyMeterRepository = energyMeterRepository;
        }

        [HttpGet("Daily/{meterId}/{date}")]
        public async Task<IActionResult> GetDailyGraphData(Guid meterid, DateTime date)
        {
            try
            {
                return Ok(await _energyMeterRepository.GetDailyAsync(meterid, date));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("Monthly/{meterId}/{date}")]
        public async Task<IActionResult> GetMonthlyGraphData(Guid meterid, DateTime date)
        {
            try
            {
                return Ok(await _energyMeterRepository.GetMonthlyAsync(meterid, date));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("Yearly/{meterId}/{date}")]
        public async Task<IActionResult> GetYearlyGraphData(Guid meterid, DateTime date)
        {
            try
            {
                return Ok(await _energyMeterRepository.GetYearlyAsync(meterid, date));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
