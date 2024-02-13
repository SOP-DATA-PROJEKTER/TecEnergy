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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomId(Guid id)
        {
            try
            {
                var result = await _roomRepository.GetRoomByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }

        }

        [HttpPost("{name}")]
        public async Task<IActionResult> CreateRoom(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name is required");
            }

            if(await _roomRepository.GetRoomByNameAsync(name))
            {
                return BadRequest("Name already exists");
            }

            try
            {
                var result = await _roomRepository.CreateAsync(name);
                return CreatedAtAction("GetRoomId", new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }

        }


        [HttpGet("SimpleInfo")]
        public async Task<IActionResult> GetSimpleInfo()
        {
            // Returns a list of SimpleInfoDto of rooms
            try
            {
                return Ok(await _roomRepository.GetSimpleInfoAsync());
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
        }


        [HttpGet("MeterData/{Id}")]
        public async Task<IActionResult> GetRoomData(Guid Id)
        {
            // return a RoomDataDto

            if (Id == Guid.Empty)
            {
                return BadRequest("Id is required");
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(await _roomRepository.GetRoomDataAsync(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }

        }



    }
}
