using System.ComponentModel.DataAnnotations;

namespace MediMitra.DTO
{
    public class sendOtpDTO
    {
        public int Otp { get; set; }
        [Required(ErrorMessage = "Password is required.")]

        [StringLength(50, MinimumLength = 8, ErrorMessage = "NewPassword must be at least 8 characters long.")]
        public string newPassword { get; set; }=string.Empty;

        [Required(ErrorMessage = "ConfirmNewPassword is required.")]
        [Compare("newPassword", ErrorMessage = "NewPassword and ConfirmNewPassword do not match.")]
        public string confirmPassword { get; set; } = string.Empty;
    }
}
