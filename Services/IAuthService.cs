using MediMitra.DTO;
using MediMitra.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediMitra.Services
{
    public interface IAuthService
    {
        Task<Response<RegisterModel>> Signup(RegisterDTO registerDTO);
        Task<Response<LoginResponseDTO>> LoginUser(LoginDTO loginDTO);
        Task<Response<string>> changePassword(ChangePasswordDTO changePasswordDTO,String Email);
        Task<Response<int>> forgotPassword(string email, HttpContext httpcontext);
        Task<Response<string>> ResetPassword(int otp, string newPassword, string confirmPassword, HttpContext httpContext);
        Task<Response<ChangeRoleDTO>> ChangeRoleByAdmin([FromBody] ChangeRoleDTO changeRoleDTO);
    }
}
