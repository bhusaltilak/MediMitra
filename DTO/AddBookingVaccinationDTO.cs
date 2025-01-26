using MediMitra.Filters;
using System.ComponentModel.DataAnnotations;

namespace MediMitra.DTO
{
    public class AddBookingVaccinationDTO
    {
        [Required(ErrorMessage = "Patient Name is required.")]
        [StringLength(100, ErrorMessage = "Patient Name cannot exceed 100 characters.")]
        [ValidateUsernameValidationAttribute]
        public string PatientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        public DateOnly DOB { get; set; }

        [Required(ErrorMessage = "Booking Date is required.")]
        [DataType(DataType.Date)]
        public DateOnly BookingDate { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(100,MinimumLength =3, ErrorMessage = "Address shouldnot be less than 3 and more than 100 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vaccination ID is required.")]
        public int VaccinationId { get; set; }
    }
}
