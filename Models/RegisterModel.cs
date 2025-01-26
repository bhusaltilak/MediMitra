using System.ComponentModel.DataAnnotations;

namespace MediMitra.Models
{
    public class RegisterModel
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Role { get; set; } = String.Empty;
        public int Otp { get; set; }
    }
}
