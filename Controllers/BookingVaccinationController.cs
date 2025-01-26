using MediMitra.Data;
using MediMitra.DTO;
using MediMitra.Models;
using MediMitra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MediMitra.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingVaccinationController : ControllerBase
    {
        private readonly BookingVaccinationServices _bookingVaccinationServices;
        public BookingVaccinationController(ApplicationDbContext dbContext, IConfiguration configuration, BookingVaccinationServices bookingVaccinationServices)
        {
            _bookingVaccinationServices = bookingVaccinationServices;
        }

        [Authorize(Roles ="User")]
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> CreateBooking(AddBookingVaccinationDTO booking)
        {
            if (booking == null)
            {
                return BadRequest("Invalid booking data.");
            }
           if(ModelState.IsValid)
            {
                String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                String Email = User.FindFirstValue(ClaimTypes.Email);
                var addedBooking = await _bookingVaccinationServices.CreateVaccinationBooking(booking, userId, Email);
                if (addedBooking.Status)
                {
                    return StatusCode(StatusCodes.Status201Created, addedBooking);
                }
                return StatusCode(StatusCodes.Status400BadRequest, addedBooking);
            }
            return BadRequest(ModelState);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("peeknextBooking")]
        public async Task<IActionResult> PeekNextBookingvaccination()
        {
            // Retrieve the next booking ID from the queue
            var nextBookingId =await _bookingVaccinationServices.PeekNextBooking();
            if (nextBookingId.Status)
            {
                return StatusCode(StatusCodes.Status200OK, nextBookingId);
            }
            return StatusCode(StatusCodes.Status400BadRequest, nextBookingId);

        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("markedStatusAsDelayed")]
        public async Task<IActionResult> DelayedBookingvaccination()
        {
            // Retrieve the next booking ID from the queue
            var nextBookingId = await _bookingVaccinationServices.MarkDelayedNextBooking();
            if (nextBookingId.Status)
            {
                return StatusCode(StatusCodes.Status200OK, nextBookingId);
            }
            return StatusCode(StatusCodes.Status400BadRequest, nextBookingId);

        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("markedStatusAsServed")]
        public async Task<IActionResult> ServedBookingvaccination()
        {
            // Retrieve the next booking ID from the queue
            var nextBookingId = await _bookingVaccinationServices.MarkServedNextBooking();
            if (nextBookingId.Status)
            {
                return StatusCode(StatusCodes.Status200OK, nextBookingId);
            }
            return StatusCode(StatusCodes.Status400BadRequest, nextBookingId);

        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("retrieveVaccinationBookDelayed")]
        public async Task<IActionResult> GetDelayedBookingvaccination()
        {
            // Retrieve the next booking ID from the queue
            var nextBookingId = await _bookingVaccinationServices.GetDelayedBookingsSortedAsync();
            if (nextBookingId!=null)
            {
                return StatusCode(StatusCodes.Status200OK, nextBookingId);
            }
            return StatusCode(StatusCodes.Status400BadRequest, nextBookingId);

        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("DelayedToServe/{id}")]
        public async Task<IActionResult> ChangeDelayedStatusToServed(int id)
        {
            var result=await _bookingVaccinationServices.UpdateDelayedStatusToServed(id);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }


        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("getAllBookVaccination")]
        public async Task<IActionResult> GetAllBookVaccination()
        {
            var result = await _bookingVaccinationServices.getallBookVaccination();
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }

        [Authorize(Roles ="User")]
        [HttpGet("getBookVaccinationOfCurrentUser")]
        public async Task<IActionResult> GetAllBookVaccinationOfCurrentUser()
        {
            String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingVaccinationServices.getallBookVaccinationOfUser(userId);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }
    }
}

