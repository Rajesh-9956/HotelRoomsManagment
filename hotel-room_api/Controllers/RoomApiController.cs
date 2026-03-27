using System.Net;
using AutoMapper;
using hotel_room_api.Models;
using hotel_room_api.Models.DTOs;
using hotel_room_api.Models.DTOs.InternalDTO;
using hotel_room_api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace hotel_room_api.Controllers;


// [Route("api/[controller]")]

// Not recommended : cause when you change apiName its will auto change route,
// its lead you to notify all consumers to change to new route
// but hard code keep route same !

// [Route("api/RoomApi")]

/*
 * this attribute help to
 *
 * Automatically validates DTOs
 * Handles binding from JSON, query params, headers.
 */
// [Route("api/v{version:apiVersion}/RoomApi")]
[Route("api/RoomApi")]
[ApiController]
[ApiVersion("1.0")]
public class RoomApiController : ControllerBase
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ILogger<RoomApiController> _logger;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    
    protected APIResponse _response ;
    
    public RoomApiController(IWebHostEnvironment hostingEnvironment,ILogger<RoomApiController> logger, IRoomRepository roomRepository, IMapper mapper)
    {
        _hostingEnvironment = hostingEnvironment;
        _logger = logger;
        _roomRepository = roomRepository;
        _mapper = mapper;
        this._response = new ();
        
    }
    
    
    
    [HttpGet]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public  async Task<ActionResult<APIResponse>> GetRooms(
        [FromQuery(Name = "PageSizeName")]int pageSize = 0, int pageNumber = 1)
    {
        try
        {
            _logger.LogInformation("Get All Rooms :)");
            IEnumerable<Room> RoomsList = await _roomRepository.GetAllAsync(pageSize:pageSize, pageNumber:pageNumber );

            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = _mapper.Map<List<RoomDTO>>(RoomsList);
        
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages.Add($"Error: {e.Message}");
            return _response;
        }
        
    }

    
    // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HotelDTO))]
    // [ResponseCache(Duration = 30)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(404)]
    [Authorize(Roles = "admin")]
    [HttpGet("{id:int}", Name = "GetRoomById")]
    public async Task<ActionResult<APIResponse>> GetRoomById(int id)
    {
        try{
            if (id == 0)
            {
                // Console.BackgroundColor = ConsoleColor.Red;
            // Console.WriteLine($"{id}");
                _logger.LogError($"Error Getting User with ID + {id}");

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            
            var room =await _roomRepository.GetAsync(h => h.Id == id, null, false);
            
            if (room == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound; 
                return NotFound(_response);
            }
            
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = _mapper.Map<RoomDTO>(room);
            
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages.Add($"Error: {e.Message}");
            return _response;
        }
    }
    
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "admin")] 
    [HttpPost]
    public async Task<ActionResult<APIResponse>> AddNewRoom([FromForm] RoomCreateDTO roomDto)
    {
        try
        {
            if (roomDto == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = roomDto;
                
                return BadRequest(_response);
            }
               
            
            if ( await _roomRepository.GetAsync(h => h.Name == roomDto.Name, null, false) != null)
            {
                ModelState.AddModelError("ErrorName_canEmpty_andMustUnique", "Room Name must Unique");
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
               
                foreach (var VARIABLE in ModelState)
                    _response.ErrorMessages.Add(VARIABLE.Key + ": " + VARIABLE.Value.ToString());
                
                return BadRequest(_response);
            }
            
            Room model = _mapper.Map<Room>(roomDto);
            // Room model = new Room()
            // {
            //     Amenity = roomDto.Amenity,
            //     Details = roomDto.Details,
            //     ImageUrl = roomDto.ImageUrl,
            //     Name = roomDto.Name,
            //     NumberOfBeds = roomDto.NumberOfBeds,
            //     Rate = roomDto.Rate,
            //     SpaceByMiter = roomDto.SpaceByMiter
            // };
            
            await _roomRepository.AddAsync(model);

            if (roomDto.Image != null)
            {
                var (newImageUrl, newImageLocalPath) = await ImageHandler.SaveNewImageAsync(roomDto.Image,
                    _hostingEnvironment.WebRootPath, HttpContext.Request.Scheme, HttpContext.Request.Host.Value,
                    model.Id);
                model.ImageUrl = newImageUrl;
                model.ImageLocalPath = newImageLocalPath;
            }

            await _roomRepository.UpdateAsync(model);
            _response.StatusCode = HttpStatusCode.Created;
            _response.Result = _mapper.Map<RoomCreateDTO>(model);
            
            return CreatedAtRoute("GetRoomById", new {id = model.Id} ,_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages.Add($"Error: {e.Message}");
            return _response;
        }
    }       
            
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "admin")]
    [HttpDelete("{id:int}", Name = "DeleteRoom")]
    public async Task<ActionResult<APIResponse>> DeleteRoom(int id)
    {
        try
        {
        
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add( "Id Can not = 0");
                return BadRequest(_response);
            }
            var room = await _roomRepository.GetAsync((r => r.Id == id), null, false);
            
            if (room == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>(){$"Room with ID {id} is Not Founded"};
                return NotFound(_response);
            }

            if (room.ImageLocalPath != null)
            {
                await ImageHandler.DeleteImageAsync
                    (room.ImageLocalPath, _hostingEnvironment.WebRootPath);
            }
            await _roomRepository.RemoveAsync(room);
            
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages.Add($"Error: {e.Message}");
            return _response;
        }
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<APIResponse>> UpdateRoom(int id, [FromForm] RoomUpdateDTO roomDto)
    {
        try{
            if (roomDto == null || roomDto.Id != id)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(
                    "Ensure JSON Data and Format, also Ensure id in Object + Request is Equals ");

                return BadRequest(_response);
            }


            var existingRoom = await _roomRepository.GetAsync((r => r.Id == id), null, false);
            if (existingRoom == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add($"Room with ID {id} is Not Exist");
                return NotFound(_response);
            }
            
            
            // existingRoom.Amenity = roomDto.Amenity;
            // existingRoom.Details = roomDto.Details;
            // existingRoom.ImageUrl = roomDto.ImageUrl;
            // existingRoom.Name = roomDto.Name;
            // existingRoom.NumberOfBeds = roomDto.NumberOfBeds;
            // existingRoom.Rate = roomDto.Rate;
            // existingRoom.SpaceByMiter = roomDto.SpaceByMiter;
            
            //Delete old
            // delete old + add new 
            
            await ImageHandler.DeleteImageAsync(
                existingRoom.ImageLocalPath,
                _hostingEnvironment.WebRootPath
            );
            
            if (roomDto.Image == null)
                {
                    
                    existingRoom = _mapper.Map<Room>(roomDto);
                    existingRoom.ImageUrl = null;
                    existingRoom.ImageLocalPath = null;
                }
            
            if (roomDto.Image != null)
                {
                    var (newImageUrl, newImageLocalPath) = await ImageHandler.SaveNewImageAsync(roomDto.Image,
                        _hostingEnvironment.WebRootPath, HttpContext.Request.Scheme, HttpContext.Request.Host.Value,
                        existingRoom.Id);
                    existingRoom = _mapper.Map<Room>(roomDto);
                    existingRoom.ImageUrl = newImageUrl;
                    existingRoom.ImageLocalPath = newImageLocalPath;
                }
            
            
            await _roomRepository.UpdateAsync(existingRoom);

            _response.StatusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages.Add($"Error: {e.Message}");
            return _response;
        }
    }

    // very bad method do not use it Never :(
    
    [HttpPatch("{id:int}", Name = "UpdatePartialRoom")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async  Task<ActionResult<APIResponse>> UpdatePartialRoom(int id, JsonPatchDocument<RoomUpdateDTO> patchDocument)
    {
        try{
            if (patchDocument == null || id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(
                    "Ensure JSON Data and Format, also Ensure id in Object + Request is Equals ");

                return BadRequest(_response);
            }

            var room_ = await _roomRepository.GetAsync((r => r.Id == id), null, false);
            if (room_ == null)

            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(
                    "Ensure JSON Data and Format, also Ensure id in Object + Request is Equals ");

                return BadRequest(_response);
            }


            RoomUpdateDTO roomDTO = _mapper.Map<RoomUpdateDTO>(room_);

            // RoomUpdateDTO roomDTO = new RoomUpdateDTO()
            // {
            //
            //     Amenity = room_.Amenity,
            //     Details = room_.Details,
            //     ImageUrl = room_.ImageUrl,
            //     Name = room_.Name,
            //     NumberOfBeds = room_.NumberOfBeds,
            //     Rate = room_.Rate,
            //     SpaceByMiter = room_.SpaceByMiter
            //
            // };
            patchDocument.ApplyTo(roomDTO, ModelState);

            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                foreach (var Error in ModelState)
                    _response.ErrorMessages.Add(Error.Key.ToString() + ": " + Error.Value.ToString());
                return BadRequest(_response);
            }


            Room room = _mapper.Map<Room>(roomDTO);

            // room.Name = roomDTO.Name;
            // room.SpaceByMiter = roomDTO.SpaceByMiter;
            // room.NumberOfBeds = roomDTO.NumberOfBeds;
            // room.Details = roomDTO.Details;
            // room.Rate = roomDTO.Rate;
            // room.Amenity = roomDTO.Amenity;
            // room.ImageUrl = roomDTO.ImageUrl;

            await _roomRepository.UpdateAsync(room);

            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages.Add($"Error: {e.Message}");
            return _response;
        }
    }
    
}
