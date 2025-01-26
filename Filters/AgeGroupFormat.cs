using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MediMitra.ValidationAttributes
{
    public class AgeGroupFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string ageGroup && !string.IsNullOrWhiteSpace(ageGroup))
            {
                // Define the regex pattern for age group format "0-2 years", "3-5 years", etc.
                var regexPattern = @"^\d+-\d+ years$";

                // Check if the age group matches the pattern
                if (Regex.IsMatch(ageGroup, regexPattern))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage ?? "Age Group must be in the format '0-2 years', '3-5 years', etc.");
                }
            }

            return new ValidationResult(ErrorMessage ?? "Age Group is required.");
        }
    }
}
