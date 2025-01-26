using MediMitra.DTO;
using System.ComponentModel.DataAnnotations;

namespace MediMitra.Models
{
    public class BookingVaccination
    {
        [Key]
        public int BookingId { get; set; }
        public String UserId { get; set; }=String.Empty;
        public String PatientName { get; set; } = String.Empty; 
        public DateOnly DOB { get; set; }
        public String Address { get; set; } = String.Empty;
        public DateOnly BookingDate { get; set; }
        public int VaccinationId { get; set; }
        public string Token { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public Vaccination Vaccination { get; set; }

        internal object Select(Func<object, BookingVaccinationDTO> value)
        {
            throw new NotImplementedException();
        }
    }
    public enum BookingStatus
    {
        Booked,
        Served,
        Delayed
    }
}
