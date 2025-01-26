using MediMitra.Data;
using MediMitra.DTO;
using MediMitra.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.NetworkInformation;

namespace MediMitra.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

       
        public async Task<Response<RegisterModel>> Signup(RegisterDTO registerDTO)
        {
            try
            {
                var user = await _context.registerModels.FirstOrDefaultAsync(u => u.Email == registerDTO.Email);
                if (user != null)
                {
                    return new Response<RegisterModel> { Status = false, Message = "Email already in use." ,Type="Email"};
                }

                var userData = new RegisterModel
                {
                    Username = registerDTO.Username,
                    Email = registerDTO.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),
                    Role = "User"
                };

                if (userData != null)
                {
                    await _context.registerModels.AddAsync(userData);
                    await _context.SaveChangesAsync();
                    return new Response<RegisterModel> { Status = true, Message = "User Created Successfully!", Data = userData };
                }

                return new Response<RegisterModel> { Status = false, Message = "Something went wrong!" };
            }
            catch (Exception ex)
            {
                return new Response<RegisterModel> { Status = false, Message = ex.Message};
            }

        }
        public async Task<Response<LoginResponseDTO>> LoginUser(LoginDTO loginDTO)
        {
            var user = await _context.registerModels.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (user != null && (BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password) && user.Email == loginDTO.Email))
            {
                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var createdToken = new JwtSecurityTokenHandler().WriteToken(token);

                var response = new LoginResponseDTO
                {
                    UserName= user.Username,
                    Token = createdToken,
                    Role = user.Role
                };

                return new Response<LoginResponseDTO> { Status = true, Message = "Login Successfully", Data = response };
            }

            return new Response<LoginResponseDTO> { Status = false, Message = "Email or password does not match.", Type = "EmailPassword" };
        }


        public async Task<Response<string>> changePassword(ChangePasswordDTO changePasswordDTO, String Email)
        {
            var user = await _context.registerModels.FirstOrDefaultAsync(u => u.Email == Email);
            if (user!=null && BCrypt.Net.BCrypt.Verify(changePasswordDTO.OldPassword, user.Password) && user.Email ==Email)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.NewPassword);
                _context.Update(user);
                await _context.SaveChangesAsync();
                return new Response<string> { Status = true, Message = "Password changed successfully!"};
            }
            return new Response<string> { Status = false, Message = "Old password doesnot match!",Type="OldPassword" };

        }

        public async Task<Response<int>> forgotPassword(string email, HttpContext httpcontext)
        {
            httpcontext.Session.SetString("UserEmail", email);
            var user = await _context.registerModels.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
               
                Random random = new Random();
                int otp = random.Next(10000, 100000);
             
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Durga Khanal", _configuration["SmtpSettings:Username"]));//user
                    message.To.Add(new MailboxAddress("", user.Email));
                    message.Subject = "Password Reset";
                    message.Body = new TextPart("plain") { Text = $"{otp} is your verification code." };

                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync(_configuration["SmtpSettings:Server"], int.Parse(_configuration["SmtpSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                        await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                        await client.SendAsync(message);
                        user.Otp = otp;
                        await _context.SaveChangesAsync();
                        await client.DisconnectAsync(true);
                     
                        return new Response<int> { Status = true, Message = "Otp Sent Successfully!" };
                    }
                }
                catch (Exception ex)
                {
                    return new Response<int> { Status = false, Message = ex.Message };
                }
            }
            return new Response<int> { Status = false, Message = "Email is invalid!",Type="Email" };
        }

        public async Task<Response<string>> ResetPassword(int otp, string newPassword, string confirmPassword, HttpContext httpContext)
        {
            var email = httpContext.Session.GetString("UserEmail");
            if(email == null)
            {
                return new Response<string> { Status = false, Message = "Session timeout",Type="Session_TimeOut" };
            }
           
            var checkotp = await _context.registerModels.FirstOrDefaultAsync(u => u.Otp == otp && u.Email == email);
            if (checkotp != null)
            {
                checkotp.Password = BCrypt.Net.BCrypt.HashPassword(confirmPassword);
                checkotp.Otp = 0;
                await _context.SaveChangesAsync();
                return new Response<string> { Status = true, Message = "Password reset Successfully!" };
            }
            return new Response<string> { Status = false, Message = "Invalid otp!",Type="Otp" };

        }


    public async Task<Response<ChangeRoleDTO>> ChangeRoleByAdmin([FromBody] ChangeRoleDTO changeRoleDTO)
        {
            var user = await _context.registerModels.FirstOrDefaultAsync(u => u.Email == changeRoleDTO.Email);

            if (user == null)
            {
                return new Response<ChangeRoleDTO> { Status = false, Message = "User not found",Type="InvalidEmail" };
            }
            
            var validRoles = new List<string> { "Admin", "Moderator", "User" }; 
            if (!validRoles.Contains(changeRoleDTO.NewRole))
            {
                return new Response<ChangeRoleDTO> { Status = false, Message = "Role doesnot exist.", Type="InvalidRoleAssign" };
            }
            
            user.Role = changeRoleDTO.NewRole;
            _context.registerModels.Update(user);
            await _context.SaveChangesAsync();

            return new Response<ChangeRoleDTO> { Status = true, Message = "User Role updated successfully",Data=changeRoleDTO };

        }
    }

}




