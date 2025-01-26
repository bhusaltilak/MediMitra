using System.ComponentModel.DataAnnotations;

namespace MediMitra.DTO
{
    public class ChangePasswordDTO
    {
        public string OldPassword { get; set; } = String.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "NewPassword must be at least 8 characters long.")] 
        public string NewPassword { get; set; } = String.Empty;

        [Required(ErrorMessage = "ConfirmNewPassword is required.")]
        [Compare("NewPassword", ErrorMessage = "NewPassword and ConfirmNewPassword do not match.")]
        public string ConfirmNewPassword { get; set; } = String.Empty;
    }
}
