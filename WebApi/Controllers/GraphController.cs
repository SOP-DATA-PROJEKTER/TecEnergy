using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using WebApi.Dtos;
using WebApi.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphController : ControllerBase
    {
        private readonly IGraphRepository _graphRepository;
        public GraphController(IGraphRepository graphRepository)
        {
            _graphRepository = graphRepository;
        }

        [HttpGet("Daily/{MeterId}/{Date}")]
        public async Task<IActionResult> GetDailyGraphData(string MeterId, DateTime Date)
        {
            Guid meterId = Guid.Parse(MeterId);
            
            if(await _graphRepository.IsRoomId(meterId))
            {
                try
                {
                    return Ok(await _graphRepository.GetDailyAsyncFromRoomId(meterId, Date));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                try
                {
                    return Ok(await _graphRepository.GetDailyAsync(meterId, Date));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

            }

        }

        [HttpGet("Monthly/{MeterId}/{date}")]
        public async Task<IActionResult> GetMonthlyGraphData(string MeterId, DateTime date)
        {
            Guid meterId = Guid.Parse(MeterId);

            if (await _graphRepository.IsRoomId(meterId))
            {
                try
                {
                    return Ok(await _graphRepository.GetMonthlyAsyncFromRoomId(meterId, date));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                try
                {
                    return Ok(await _graphRepository.GetMonthlyAsync(meterId, date));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

        }

        [HttpGet("Yearly/{MeterId}")]
        public async Task<IActionResult> GetYearlyGraphData(string MeterId)
        {
            Guid meterId = Guid.Parse(MeterId);

            if (await _graphRepository.IsRoomId(meterId))
            {
                try
                {
                    return Ok(await _graphRepository.GetYearlyAsyncFromRoomId(meterId));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                try
                {
                    return Ok(await _graphRepository.GetYearlyAsync(meterId));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }
    }
}
