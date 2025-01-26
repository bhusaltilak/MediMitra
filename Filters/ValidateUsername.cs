using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MediMitra.Filters
{

    public class ValidateUsernameValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Username is required.");
            }

            var username = value.ToString();

            // Regex pattern: ^[A-Za-z]+ [A-Za-z]+$ 
            var regex = new Regex(@"^[A-Za-z]+ [A-Za-z]+$");

            if (!regex.IsMatch(username))
            {
                return new ValidationResult("Username must contain only alphabets with a space between first and last name.");
            }

            return ValidationResult.Success;
        }
    }

}
