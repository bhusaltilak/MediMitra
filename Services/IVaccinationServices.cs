using MediMitra.DTO;
using MediMitra.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediMitra.Services
{
    public interface IVaccinationServices
    {
        Task<Response<Vaccination>> addVaccination ([FromBody] AddvaccinationDTO addvaccinationDTO);
        Task<Response<List<Vaccination>>> getallVaccination();
        Task<Response<Vaccination>> getVaccinationById(int id);
        Task<Response<Vaccination>> getVaccinationByVaccinationName([FromQuery] String vaccinationName);
        Task<Response<Vaccination>> getVaccinationByVaccinationType([FromQuery] String vaccinationType);
        Task<Response<Vaccination>> getVaccinationNameAndType(String vaccinationName,String vaccinationType);
        Task<Response<Vaccination>> updateVaccination(int id,UpdateVaccinationDTO updateVaccinationDTO);
        Task<Response<Vaccination>>deleteVaccination(int id);
    }
}
