using MediMitra.Data;
using MediMitra.DTO;
using MediMitra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MediMitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {

            if (ModelState.IsValid)
            {
                var result = await _authService.Signup(registerDTO);
                if (result.Status)
                {
                    return StatusCode(StatusCodes.Status201Created, result);
                }
                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            return BadRequest(ModelState);
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login(LoginDTO loginDTO)
        {
            var result = await _authService.LoginUser(loginDTO);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }

        [Authorize]
        [HttpPut]
        [Route("change-password")]
        public async Task<IActionResult> updatePassword(ChangePasswordDTO changePasswordDTO)
        {
            if (ModelState.IsValid)
            {
                var Email = User.FindFirstValue(ClaimTypes.Email);
                var result = await _authService.changePassword(changePasswordDTO, Email);
                if (result.Status)
                {
                    return StatusCode(StatusCodes.Status200OK, result);
                }
                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("forgotpassword")]
        public async Task<IActionResult> forgotEmailPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            var result = await _authService.forgotPassword(forgotPasswordDTO.Email, HttpContext);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }

        [HttpPost]
        [Route("reset-password")]

        public async Task<IActionResult> ResetPasswordusingOTP(sendOtpDTO sendOtpDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ResetPassword(sendOtpDTO.Otp, sendOtpDTO.newPassword, sendOtpDTO.confirmPassword, HttpContext);

                if (result.Status)
                {
                    return StatusCode(StatusCodes.Status200OK, result);
                }
                return StatusCode(StatusCodes.Status400BadRequest, result);

            }
            return BadRequest(ModelState);
        }

        [Authorize(Roles ="Admin")]
        [HttpPut("ChangeUserRole")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeRoleDTO changeRoleDTO)
        {

            var result = await _authService.ChangeRoleByAdmin(changeRoleDTO);
            if (result.Status)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status400BadRequest, result);
        }
    }

}
