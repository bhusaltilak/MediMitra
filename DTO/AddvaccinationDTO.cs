using MediMitra.Filters;
using MediMitra.ValidationAttributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MediMitra.DTO
{
    public class AddvaccinationDTO
    {
        [Required(ErrorMessage = "Vaccination Name is required.")]
        [StringLength(50,MinimumLength =3, ErrorMessage = "Vaccination Name must be more than 3 and less than 50 characters.")]
        public string VaccinationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vaccination Type is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Vaccination Type must be more than 3 and less than 50 characters.")]
        public string VaccinationType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vaccination Dose is required.")]
        [Range(1, 10, ErrorMessage = "Vaccination Dose must be between 1 and 10.")]
        public int VaccinationDose { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Location must be more than 3 and less than 50 characters.")]
        public String Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age Group is required.")]
        [AgeGroupFormat(ErrorMessage = "Age Group must be in the format '0-2 years', '3-5 years', etc.")]
        public string AgeGroup { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        public DateOnly ServeDate { get; set; }
        public VaccinationStatus Status { get; set; }   
    }
    public enum VaccinationStatus
    {
        Available,
        NotAvailable
    }
}


