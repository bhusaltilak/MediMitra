namespace MediMitra.DTO
{
    // BookingVaccinationDTO.cs
    public class BookingVaccinationDTO
    {
        public int BookingVaccinationId { get; set; }
        public string PatientName { get; set; }
        public DateOnly DOB { get; set; }
        public DateOnly BookingDate { get; set; }
        public string Token { get; set; }   
        public string Address { get; set; }

        // Vaccination details
        public int VaccinationId { get; set; }
        public string VaccinationName { get; set; }
        public string VaccinationType { get; set; }
        public int VaccinationDose { get; set; }
        public string Location { get; set; }
        public string AgeGroup { get; set; }
        public DateOnly ServeDate { get; set; }
        public VaccinationStatus VaccinationStatus { get; set; }
    }

}
