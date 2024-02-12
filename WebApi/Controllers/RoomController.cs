using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }


        [HttpGet("RoomId/{id}")]
        public async Task<IActionResult> GetRoomId(Guid id)
        {
            var result = await _roomRepository.GetRoomByIdAsync(id);
            return Ok(result);
        }


        [HttpPost("BuildingId")]
        public async Task<IActionResult> CreateRoom(Guid BuildingId)
        {
            try
            {
                var result = await _roomRepository.CreateAsync(BuildingId);
                return CreatedAtAction(nameof(GetRoomId), result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("SimpleInfo")]
        public async Task<IActionResult> GetSimpleInfo()
        {
            return NoContent();
        }


        [HttpGet("MeterData/{Id}")]
        public async Task<IActionResult> GetRoomMeterData(Guid Id)
        {
            return NoContent();
        }



    }
}
