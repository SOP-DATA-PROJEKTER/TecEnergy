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

        [HttpPost]
        public async Task<IActionResult> CreateRoom()
        {
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetSimpleInfo()
        {
            return NoContent();
        }

        [HttpGet("/Id")]
        public async Task<IActionResult> GetRoomMeterData([FromQuery] Guid Id)
        {
            return NoContent();
        }
    }
}
