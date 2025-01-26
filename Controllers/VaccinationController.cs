using MediMitra.DTO;
using MediMitra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediMitra.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VaccinationController : ControllerBase
    {
        private readonly IVaccinationServices _vaccinationServices;
        public VaccinationController(IVaccinationServices vaccinationServices)
            {
                _vaccinationServices = vaccinationServices;
        }

        [Authorize(Roles ="Admin,Moderator")]
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> VaccinationCreate([FromBody] AddvaccinationDTO addvaccinationDTO)
        {
            if(ModelState.IsValid)
            {
             var result=await _vaccinationServices.addVaccination(addvaccinationDTO);
             if (result.Status)
            {
                return StatusCode(StatusCodes.Status201Created, result);
            }
             return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            return BadRequest(ModelState);
        }
    
        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetAllVaccination()
        {
            var result=await _vaccinationServices.getallVaccination();
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }

     
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVaccinationDetailById(int id)
        {
            var result=await _vaccinationServices.getVaccinationById(id);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }

      
        [HttpGet]
        [Route("vaccinationname")]
        public async Task<IActionResult> GetVaccinationDetailByName([FromQuery] string vaccinationname)
        {
            var result = await _vaccinationServices.getVaccinationByVaccinationName(vaccinationname);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }

   
        [HttpGet]
        [Route("vaccinationtype")]
        public async Task<IActionResult> GetVaccinationDetailsBytype([FromQuery] string vaccinationtype)
        {
            var result = await _vaccinationServices.getVaccinationByVaccinationType(vaccinationtype);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }

        [HttpGet]
        [Route("vaccinationNameAndType")]
        public async Task<IActionResult> GetVaccinationDetailsByTypeandName([FromQuery] string vaccinationname ,[FromQuery] string vaccinationtype)
        {
            var result = await _vaccinationServices.getVaccinationNameAndType(vaccinationname,vaccinationtype);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("updateVaccination/{id}")]
        
        public async Task<IActionResult> UpdateVaccinationData(int id,UpdateVaccinationDTO updateVaccinationDTO)
        {
            if (ModelState.IsValid)
            {
            var result=await _vaccinationServices.updateVaccination(id,updateVaccinationDTO);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
               
            }
            return BadRequest(ModelState);  

         }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpDelete("deleteVaccination/{id}")]
        public async Task<IActionResult> DeleteVaccinationDetails(int id)
        {
            var result = await _vaccinationServices.deleteVaccination(id);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);

        }
    
    }
}
