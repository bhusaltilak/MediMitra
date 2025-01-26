using MediMitra.Data;
using MediMitra.DTO;
using MediMitra.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;

namespace MediMitra.Services
{
    public class VaccinationServices : IVaccinationServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public VaccinationServices(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<Response<Vaccination>> addVaccination([FromBody] AddvaccinationDTO addvaccinationDTO)
        {
            var vaccination = new Vaccination
            {
                VaccinationName = addvaccinationDTO.VaccinationName,
                VaccinationType = addvaccinationDTO.VaccinationType,
                Location = addvaccinationDTO.Location,  
                VaccinationDose = addvaccinationDTO.VaccinationDose,
                AgeGroup = addvaccinationDTO.AgeGroup,
                ServeDate = addvaccinationDTO.ServeDate,
                Status=Models.VaccinationStatus.Running
            };

            if(vaccination!=null)
            {
                var user = await _context.registerModels.ToListAsync();
                if(user!=null)
                {
                   
                    for (int i = 0;i<user.Count;i++)
                    {
                        try
                        {
                            var message = new MimeMessage();
                            message.From.Add(new MailboxAddress("Durga Khanal", _configuration["SmtpSettings:Username"]));
                            message.To.Add(new MailboxAddress("", user[i].Email));
                            message.Subject = "नयाँ खोप सूचना!";
                            message.Body = new TextPart("plain")
                            {
                                Text = $@"
                                नयाँ खोप निम्न विवरणहरू सहित सिर्जना गरिएको छ:

                                - खोपको नाम: {vaccination.VaccinationName}
                                - खोपको प्रकार: {vaccination.VaccinationType}
                                - स्थान:{vaccination.Location}
                                - खोपको मात्रा: {vaccination.VaccinationDose}
                                - उमेर समूह: {vaccination.AgeGroup}
                                - Service मिति: {vaccination.ServeDate:yyyy-MM-dd}

                                कृपया विवरणहरू समीक्षा गर्नुहोस् र आवश्यक कार्यहरू गर्नुहोस्।
                                "
                                                            };

                            using (var client = new SmtpClient())
                            {
                                await client.ConnectAsync(_configuration["SmtpSettings:Server"], int.Parse(_configuration["SmtpSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                                await client.SendAsync(message);
                                await _context.SaveChangesAsync();
                                await client.DisconnectAsync(true);
                               

                            }
                        }
                        catch (Exception ex)
                        {
                            return new Response<Vaccination> { Status = false, Message = ex.Message };
                        }

                    }
                }

                await _context.vaccinations.AddAsync(vaccination);
                await _context.SaveChangesAsync();
                return new Response<Vaccination> { Status = true, Message = "Vaccination created and sent to user successfully !", Data = vaccination };
            }
            return new Response<Vaccination> { Status = false, Message = "Vaccination not created ,something went wrong !" };

        }

        public async Task<Response<List<Vaccination>>> getallVaccination()
        {
            var vaccinationrecords=await _context.vaccinations.ToListAsync();
            if(vaccinationrecords==null || vaccinationrecords.Count == 0)
            {
                return new Response<List<Vaccination>> { Status = false, Message = "No vaccination Records found.",Type="NoVaccinationData" };
            }
            return new Response<List<Vaccination>> { Status = true, Message = "vaccination Records retrieved successfully!.",Data=vaccinationrecords };

        }

        public async Task<Response<Vaccination>> getVaccinationById(int id)
        {
            var vaccination=await _context.vaccinations.FirstOrDefaultAsync(v=>v.VaccinationId==id);
            if (vaccination == null)
            {
                return new Response<Vaccination> { Status = false, Message = "Vaccination not found!",Type="VaccinationIdNotFound" };
            }
            return new Response<Vaccination> { Status = true, Message = "Vaccination retrieved!",Data=vaccination };

        }
        public async Task<Response<Vaccination>> getVaccinationByVaccinationName([FromQuery] String vaccinationName)
        {
            var vaccination = await _context.vaccinations.FirstOrDefaultAsync(v => v.VaccinationName == vaccinationName);
            if (vaccination == null)
            {
                return new Response<Vaccination> { Status = false, Message = "Vaccinationname not found!",Type="VaccinatioNameNotFound" };
            }
            return new Response<Vaccination> { Status = true, Message = "Vaccination retrieved successfully!", Data = vaccination };

        }
        public async Task<Response<Vaccination>> getVaccinationByVaccinationType([FromQuery] String vaccinationType)
        {
            var vaccination = await _context.vaccinations.FirstOrDefaultAsync(v => v.VaccinationType == vaccinationType);
            if (vaccination == null)
            {
                return new Response<Vaccination> { Status = false, Message = "VaccinationType not found!",Type="NoVaccinationType" };
            }
            return new Response<Vaccination> { Status = true, Message = "Vaccination retrieved successfully!", Data = vaccination };

        }

        public async Task<Response<Vaccination>> getVaccinationNameAndType(String vaccinationName, String vaccinationType)
        {
            var vaccination = await _context.vaccinations.FirstOrDefaultAsync(v =>v.VaccinationName==vaccinationName && v.VaccinationType == vaccinationType);
            if (vaccination == null)
            {
                return new Response<Vaccination> { Status = false, Message = "VaccinationName along with VaccinationType not found!" ,Type="VaccinationNameAndTypeNotFound"};
            }
            return new Response<Vaccination> { Status = true, Message = "Vaccination retrieved successfully!", Data = vaccination };

        }
        public async Task<Response<Vaccination>> updateVaccination(int id,UpdateVaccinationDTO updateVaccinationDTO)
        {
            var vaccination = await _context.vaccinations.FindAsync(id);

            if (vaccination == null)
            {
                return new Response<Vaccination> { Status = false, Message = "Vaccination not found to Update!" };
            }
            vaccination.VaccinationName=updateVaccinationDTO.VaccinationName;
            vaccination.VaccinationType = updateVaccinationDTO.VaccinationType;
            vaccination.VaccinationDose = updateVaccinationDTO.VaccinationDose;
            vaccination.AgeGroup= updateVaccinationDTO.AgeGroup;
            vaccination.ServeDate= updateVaccinationDTO.ServeDate;
          
            var user = await _context.registerModels.ToListAsync();
            if (user != null)
            {

                for (int i = 0; i < user.Count; i++)
                {
                    try
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("Durga Khanal", _configuration["SmtpSettings:Username"]));
                        message.To.Add(new MailboxAddress("", user[i].Email));
                        message.Subject = "विद्यमान खोपमा नयाँ परिवर्तनहरू";
                        message.Body = new TextPart("plain")
                        {
                            Text = $@"
                                नयाँ खोप निम्न विवरणहरू सहित सिर्जना गरिएको छ:

                                - खोपको नाम: {vaccination.VaccinationName}
                                - खोपको प्रकार: {vaccination.VaccinationType}
                                - खोपको मात्रा: {vaccination.VaccinationDose}
                                - उमेर समूह: {vaccination.AgeGroup}
                                - Service मिति: {vaccination.ServeDate:yyyy-MM-dd}

                                कृपया विवरणहरू समीक्षा गर्नुहोस् र आवश्यक कार्यहरू गर्नुहोस्।
                                "
                        };


                        using (var client = new SmtpClient())
                        {
                            await client.ConnectAsync(_configuration["SmtpSettings:Server"], int.Parse(_configuration["SmtpSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                            await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                            await client.SendAsync(message);
                            await _context.SaveChangesAsync();
                            await client.DisconnectAsync(true);


                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response<Vaccination> { Status = false, Message = ex.Message };
                    }

                }
            }

            await _context.SaveChangesAsync();
            return new Response<Vaccination> { Status=true,Message="Vaccination updated successfully!",Data= vaccination};
        }

        public async Task<Response<Vaccination>> deleteVaccination(int id)
        {
            var vaccination = await _context.vaccinations.FindAsync(id);

            if (vaccination == null)
            {
                return new Response<Vaccination> { Status = false, Message = "Vaccination not found to Delete!" };
            }
            _context.vaccinations.Remove(vaccination);
            await _context.SaveChangesAsync();
            return new Response<Vaccination> { Status = true, Message = "Vaccination deleted Successfully!!" };
        }
    }
}
