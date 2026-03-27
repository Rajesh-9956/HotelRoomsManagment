using System.Linq.Expressions;
using System.Net;
using hotel_room_api.Models;
using hotel_room_api.Models.DTOs;
using hotel_room_api.Models.DTOs.RoomNumberDTO;
using hotel_room_api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace hotel_room_api.Controllers;

[Route("api/bedNumber")]
[ApiController]
public class BedNumberApi : Controller
{
    protected APIResponse _apiResponse;
    private readonly IBedNumber _bedNumber;
    private readonly IRoomRepository _roomRepository;
    
    public BedNumberApi(IBedNumber dbBedNumber, IRoomRepository roomRepository)
    {
        _bedNumber = dbBedNumber;
        _roomRepository = roomRepository;
        _apiResponse = new APIResponse();
    }

    [Authorize(Roles = "admin")]
    [HttpGet(Name = "GetBedNumbers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public  async Task<ActionResult<APIResponse>> GetBedNumbers()
    {
        try
        {
            var BedNumbers = await _bedNumber.GetAllAsync(Includes: new Expression<Func<BedNumber, object>>[]
            {
                b => b.Room
            });
            if (BedNumbers.Count == 0)
            {
                ModelState.AddModelError("ErrorMessages", "Empty list");
                _apiResponse.ErrorMessages = new List<string>()
                {
                    "BedNumbers list is Empty"
                };
            }
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = BedNumbers;
            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
            _apiResponse.ErrorMessages.Add($"Error: {e.Message}");
            return _apiResponse;
        }
    }

    [HttpGet( "{id:int}", Name = "GetBedNumberById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<APIResponse>> GetBedNumberById(int id)
    {
        try
        {
            if (id < 0)
            {
                ModelState.AddModelError("ErrorMessages", " Id is Not Exist");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = new List<string>()
                {
                    "Ensure BedNumber is grater Than Zero `0`"
                };

                return BadRequest(_apiResponse);
            }

            var BedNumber = await _bedNumber.GetAsync(rn => rn.bedNo == id,
                Includes: new Expression<Func<BedNumber, object>>[]
                {
                    b => b.Room
                });
            if (BedNumber == null)
            {
                ModelState.AddModelError("ErrorMessages", " Id is Not Exist");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.ErrorMessages = new List<string>()
                {
                    $"bed with Bed Number {id} is Not Founded"
                };
                return NotFound(_apiResponse);
            }
            
            BedNumberDTO bedNoDto = new BedNumberDTO()
            {
                bedNo = BedNumber.bedNo,
                RoomId = BedNumber.RoomId,
                specialDetails = BedNumber.specialDetails
            };

            if (BedNumber.Room != null)
            {
                RoomDTO mappedRoom = new RoomDTO()
                {
                    Id = BedNumber.Room.Id,
                    Name = BedNumber.Room.Name,
                    Details = BedNumber.Room.Details,
                    NumberOfBeds = BedNumber.Room.NumberOfBeds,
                    Amenity = BedNumber.Room.Amenity,
                    Rate = BedNumber.Room.Rate,
                    SpaceByMiter = BedNumber.Room.SpaceByMiter,
                    ImageUrl = BedNumber.Room.ImageUrl
                };

                bedNoDto.Room = mappedRoom;
            }
            
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = bedNoDto;

            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
            _apiResponse.ErrorMessages.Add($"Error: {e.Message}");
            return _apiResponse;
        }
    }

    [HttpPost(Name = "AddNewBedNumber")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<APIResponse>> addNewBedNumber([FromBody]BedNumberAddDTO newDTO )
    {
        try
        {
            if (newDTO.bedNo == null || newDTO.bedNo < 0)
            {
                ModelState.AddModelError("ErrorMessages", " Id is Not Exist");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = new List<string>()
                {
                    ("Invalid object, Ensure bed Number is > 0")
                };
                _apiResponse.Result = newDTO;
                
                return BadRequest(_apiResponse);
            }

            var bedNumber = await _bedNumber.GetAsync(rn => rn.bedNo == newDTO.bedNo);
            if (bedNumber != null )
            {
                ModelState.AddModelError("ErrorMessages", "Ensure bed Number is > 0, and Unique");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages = new List<string>()
                {
                    ("Invalid object, Ensure bed Number is > 0, and Unique")
                };
                _apiResponse.Result = newDTO;
                
                return StatusCode(500, _apiResponse);
            }

            if (await _roomRepository.GetAsync(r => r.Id == newDTO.RoomId) == null)
            {
                ModelState.AddModelError("ErrorMessages", "Room Id is Not Exist");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = new List<string>()
                {
                    ("Room Id is Not Exist Ensure this Room is Exist, Then Try!")
                };
                _apiResponse.Result = newDTO;
                
                return BadRequest(_apiResponse);
                
            }
            
            BedNumber newBed = new BedNumber()
            {
                bedNo = newDTO.bedNo,
                RoomId = newDTO.RoomId,
                specialDetails = newDTO.specialDetails,
                CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                UpdatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            
            await _bedNumber.AddAsync(newBed);

            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.Result = newDTO;

            return CreatedAtRoute("GetBedNumberById", new { id = newDTO.bedNo }, _apiResponse);

        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
            _apiResponse.ErrorMessages.Add($"Error: {e.Message}");
            return _apiResponse;
        }
    }

    [HttpDelete("{id:int}", Name = "DeleteBedNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<APIResponse>> DeleteBedNumber(int id)
    {
        try
        {
            if (id < 0)
            {
                ModelState.AddModelError("ErrorMessages", " Id must  > 0");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = new List<string>()
                {
                    "id must > 0"
                };
                return BadRequest(_apiResponse);
            }

            var bedNumber = await _bedNumber.GetAsync(rn => rn.bedNo == id);
            if (bedNumber == null)
            {
                ModelState.AddModelError("ErrorMessages", "Room Id is Not Founded");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.ErrorMessages = new List<string>()
                {
                    $"bedNumber With id {id} is Not Exist"
                };

                return NotFound(_apiResponse);
            }

            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;

            await _bedNumber.RemoveAsync(bedNumber);
            
            return  Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
            _apiResponse.ErrorMessages.Add($"Error: {e.Message}");
            return _apiResponse;
        }
    }

    [HttpPut("{id:int}", Name = "UpdateBedNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<APIResponse>> UpdateBedNumber(int id, [FromBody] BedNumberUpdateDTO bedNumberUpdateDto)
    {
        try
        {
            if (id < 0)
            {
                ModelState.AddModelError("ErrorMessages", " Id is Not Founded");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = new List<string>()
                {
                    "id must > 0"
                };
                return BadRequest(_apiResponse);
            }

            var bedNumber = await _bedNumber.GetAsync(rn => rn.bedNo == id);
            if (bedNumber == null)
            {
                ModelState.AddModelError("ErrorMessages", " Id is Not Founded");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.ErrorMessages = new List<string>()
                {
                    $"bedNumber With id {id} is Not Exist"
                };

                return NotFound(_apiResponse);
            }
            
            if (await _roomRepository.GetAsync(r => r.Id == bedNumberUpdateDto.RoomId) == null)
            {
                ModelState.AddModelError("ErrorMessages", "Room Id is Not Exist");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = new List<string>()
                {
                    ("Room Id is Not Exist Ensure this Room is Exist, Then Try!")
                };
                _apiResponse.Result = bedNumberUpdateDto;
                
                return BadRequest(_apiResponse);
                
            }

            bedNumber.bedNo = bedNumberUpdateDto.bedNo;
            bedNumber.RoomId = bedNumberUpdateDto.RoomId;
            bedNumber.specialDetails = bedNumberUpdateDto.specialDetails;
            bedNumber.UpdatedDate = DateOnly.FromDateTime(DateTime.UtcNow);

            await _bedNumber.update(bedNumber);
            
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return  Ok( _apiResponse);

        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
            _apiResponse.ErrorMessages.Add($"Error: {e.Message}");
            return _apiResponse;
        }
        
    }
    
}