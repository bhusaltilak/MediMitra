using MailKit.Net.Smtp;
using MediMitra.Data;
using MediMitra.DTO;
using MediMitra.Models;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Security.Claims;

namespace MediMitra.Services
{
    public class BookingVaccinationServices
    {
      
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private const int MaxCapacity = 500;
        private readonly BookingQueueService _queueService;

        public BookingVaccinationServices(ApplicationDbContext context, IConfiguration configuration, BookingQueueService queueService)
        {
            _context = context;
            _configuration = configuration;
            _queueService = queueService;
        }

        public async Task<Response<BookingVaccination>> CreateVaccinationBooking(AddBookingVaccinationDTO booking,String userId,String Email)
        {
            if (IsQueueFull())
            {
                return new Response<BookingVaccination> { Status = false, Message = "Booking queue is full, unable to add more bookings.",Type
                ="QueueFull"};
            }
            var bookingCount = await _context.bookingVaccinations.CountAsync();
          
            var token = GenerateToken(bookingCount + 1);


            var vaccinationpart = await _context.vaccinations.FirstOrDefaultAsync(v => v.VaccinationId == booking.VaccinationId);

            if (vaccinationpart == null)
            {
                return new Response<BookingVaccination> { Status = false, Message = "VaccinationId not Found!",Type="VaccinationId" };
            }

            var vaccinationBookpart = await _context.bookingVaccinations.Where(v => v.VaccinationId == booking.VaccinationId && v.UserId ==userId).FirstOrDefaultAsync();
            if (vaccinationBookpart != null)
            {
                return new Response<BookingVaccination> { Status = false, Message = "Vaccination is already booked by you!",Type="VaccineAlreadyBooked" };
            }

            var bookVaccination = new BookingVaccination
            {
                PatientName = booking.PatientName,
                DOB=booking.DOB,
                Address=booking.Address,
                BookingDate = booking.BookingDate,
                VaccinationId=booking.VaccinationId,
                Status=BookingStatus.Booked,
                Token= token,
                UserId= userId
            };
           
            await _context.bookingVaccinations.AddAsync(bookVaccination);
            await _context.SaveChangesAsync();

            _queueService.EnqueueQueue(bookVaccination.BookingId);
            Console.WriteLine($"Booking ID {bookVaccination.BookingId} enqueued successfully. Current Queue Size: {_queueService.GetTotalQueueSize()}");

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Durga Khanal", _configuration["SmtpSettings:Username"]));
                message.To.Add(new MailboxAddress("", Email));
                message.Subject = "नयाँ खोप सूचना!";
                message.Body = new TextPart("plain")
                {
                    Text = $"Dear {bookVaccination.PatientName},\r\n\r\nतपाईंको खोप बुकिङ सफलतापूर्वक पुष्टि भएको छ। बुकिङ विवरणहरू तल छन्:\r\n\r\n- नाम: {bookVaccination.PatientName}\r\n- जन्म मिति: {bookVaccination.DOB}\r\n - खोपको नाम: {vaccinationpart.VaccinationName}\r\n- खोपको प्रकार: {vaccinationpart.VaccinationType}\r\n- खोपको मात्रा: {vaccinationpart.VaccinationDose}\r\n-खोपको ठेगाना: {vaccinationpart.Location}\r\n- बुकिङ मिति: {bookVaccination.BookingDate}\r\n- टोकन नम्बर: {bookVaccination.Token}\r\n\r\nकृपया सेवा लिन आउँदा यो टोकन साथमा ल्याउनुहोस्। धन्यवाद!\r\n\r\nशुभेच्छा,\r\nमेडिमित्र टिम"
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
                return new Response<BookingVaccination> { Status = false, Message = ex.Message };
            }

            return new Response<BookingVaccination> { Status = true, Message = $"Vaccination Booked and Token sent successfully!", Data = bookVaccination };
        }

       

       

        public async Task<Response<BookingVaccination>> MarkDelayedNextBooking()
        {
            var nextBookingId = _queueService.GetNextBookingForDelayed();
            if(nextBookingId == null || nextBookingId==0)
            {
                return new Response<BookingVaccination> { Status = false, Message = "No next booking found in queue" };
            }
            var booking =await _context.bookingVaccinations.FirstOrDefaultAsync(b => b.BookingId == nextBookingId);
            if(booking == null)
            {
                return new Response<BookingVaccination> { Status = false, Message = "No nextbooking found in database!" };
            }
            booking.Status = BookingStatus.Delayed;
            await _context.SaveChangesAsync();
            return new Response<BookingVaccination> { Status = true, Message = "Marking User as Delayed!", Data = booking };
        }

        public async Task<Response<BookingVaccination>> MarkServedNextBooking()
        {
            var nextBookingId = _queueService.GetNextBookingForDelayed();
            if (nextBookingId == null || nextBookingId == 0)
            {
                return new Response<BookingVaccination> { Status = false, Message = "No next booking found in queue" };
            }
            var booking = await _context.bookingVaccinations.FirstOrDefaultAsync(b => b.BookingId == nextBookingId);
            if (booking == null)
            {
                return new Response<BookingVaccination> { Status = false, Message = "No nextbooking found in database!" };
            }
            booking.Status = BookingStatus.Served;
            await _context.SaveChangesAsync();
            return new Response<BookingVaccination> { Status = true, Message = "User Served sucessfully!", Data = booking };
        }

        //public async Task<Response<List<BookingVaccination>>> getallBookVaccination()
        //{
        //    var vaccinationrecords = await _context.bookingVaccinations.Include(b=>b.Vaccination).ToListAsync();
        //    if (vaccinationrecords == null || vaccinationrecords.Count == 0)
        //    {
        //        return new Response<List<BookingVaccination>> { Status = false, Message = "No BookVaccination Records found.", Type = "NoBookVaccination" };
        //    }
        //    return new Response<List<BookingVaccination>> { Status = true, Message = "vaccination Records retrieved successfully!.", Data = vaccinationrecords };

        //}

        public async Task<Response<List<BookingVaccination>>> getallBookVaccinationOfUser(string userId)
        {
            var vaccinationrecords = await _context.bookingVaccinations
       .Include(b => b.Vaccination) 
       .Where(b => b.UserId == userId) 
       .ToListAsync();
            if (vaccinationrecords == null || vaccinationrecords.Count == 0)
            {
                return new Response<List<BookingVaccination>> { Status = false, Message = "No BookVaccination Records found.", Type = "NoBookVaccination" };
            }
            return new Response<List<BookingVaccination>> { Status = true, Message = "vaccination Records retrieved successfully!.", Data = vaccinationrecords };

        }

        public async Task<Response<List<BookingVaccination>>>  GetDelayedBookingsSortedAsync()
        {
            var bookings = await _context.bookingVaccinations
     .Where(b => b.Status == BookingStatus.Delayed)
     .Include(b => b.Vaccination) 
     .ToListAsync();

            if (bookings.Count == 0 || bookings == null)
            {
                return new Response<List<BookingVaccination>> { Status = false, Message = " Delayed data not found!" };
            }
    
            MergeSort(bookings, 0, bookings.Count - 1);

            return new Response<List<BookingVaccination>> { Status = true, Message = " Delayed data retrieved successfully!", Data = bookings };
        }

        private void MergeSort(List<BookingVaccination> bookings, int left, int right)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;

              
                MergeSort(bookings, left, mid);
                MergeSort(bookings, mid + 1, right);

                Merge(bookings, left, mid, right);
            }
        }
        private void Merge(List<BookingVaccination> bookings, int left, int mid, int right)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            var leftArray = new BookingVaccination[n1];
            var rightArray = new BookingVaccination[n2];

       
            for (int i = 0; i < n1; i++)
                leftArray[i] = bookings[left + i];
            for (int j = 0; j < n2; j++)
                rightArray[j] = bookings[mid + 1 + j];

     
            int k = left;
            int iIndex = 0, jIndex = 0;

            while (iIndex < n1 && jIndex < n2)
            {
             
                int leftToken = int.Parse(leftArray[iIndex].Token);
                int rightToken = int.Parse(rightArray[jIndex].Token);

                if (leftToken <= rightToken)
                {
                    bookings[k++] = leftArray[iIndex++];
                }
                else
                {
                    bookings[k++] = rightArray[jIndex++];
                }
            }

            while (iIndex < n1)
            {
                bookings[k++] = leftArray[iIndex++];
            }

            while (jIndex < n2)
            {
                bookings[k++] = rightArray[jIndex++];
            }
        }

        private string GenerateToken(int bookingId)
        {
            return bookingId.ToString("D4");
        }

        private bool IsQueueFull()
        {
            int value = _queueService.GetTotalQueueSize();
            return value == MaxCapacity;
        }
        private bool IsQueueEmpty()
        {

            int value = _queueService.GetTotalQueueSize();
            return value == 0;
        }

        public async Task<Response<BookingVaccination>> UpdateDelayedStatusToServed(int delayedId)
        {
            var delayed = await _context.bookingVaccinations.Where(b => b.BookingId == delayedId && b.Status==BookingStatus.Delayed).FirstOrDefaultAsync();
            if(delayed != null)
            {
                delayed.Status =BookingStatus.Served;
                await _context.SaveChangesAsync();
                return new Response<BookingVaccination> { Status = true, Message = "Delayed User served successfully!",Data=delayed };
            }
            return new Response<BookingVaccination> { Status = false, Message = "Delayed data not found" };
        }




        public async Task<Response<List<BookingVaccinationDTO>>> getallBookVaccination()
        {
            
            var vaccinationRecords = await _context.bookingVaccinations
                .Include(b => b.Vaccination) 
                .ToListAsync();

            if (vaccinationRecords == null || vaccinationRecords.Count == 0)
            {
                return new Response<List<BookingVaccinationDTO>>
                {
                    Status = false,
                    Message = "No Booking Vaccination Records found.",
                    Type = "NoBookVaccination"
                };
            }

            
            var bookingVaccinationDTOs = vaccinationRecords.Select(b => new BookingVaccinationDTO
            {
                BookingVaccinationId = b.BookingId,
                PatientName = b.PatientName,
                DOB = b.DOB,
                BookingDate = b.BookingDate,
                Address = b.Address,
                Token=b.Token,
                VaccinationId = b.VaccinationId,

               
                VaccinationName = b.Vaccination.VaccinationName,
                VaccinationType = b.Vaccination.VaccinationType,
                VaccinationDose = b.Vaccination.VaccinationDose,
                Location = b.Vaccination.Location,
                AgeGroup = b.Vaccination.AgeGroup,
                ServeDate = b.Vaccination.ServeDate,
                VaccinationStatus = (DTO.VaccinationStatus)b.Vaccination.Status,
            }).ToList();

            return new Response<List<BookingVaccinationDTO>>
            {
                Status = true,
                Message = "Booking Vaccination Records retrieved successfully!",
                Data = bookingVaccinationDTOs
            };
        }

        public async Task<Response<BookingVaccination>> PeekNextBooking()
        {
            if (IsQueueEmpty())
            {
                return new Response<BookingVaccination> { Status = false, Message = $"Queue is Emplty,no booked available or unable to peek data from queue ie {_queueService.GetTotalQueueSize()}." };
            }
            var nextBookingId = _queueService.GetNextBooking();

            if (nextBookingId == null || nextBookingId == 0)
            {
                return new Response<BookingVaccination> { Status = false, Message = "NextBooking not available." };
            }


            var booking = await _context.bookingVaccinations.Include(b => b.Vaccination)
                .FirstOrDefaultAsync(b => b.BookingId == nextBookingId);

            if (booking == null)
            {
                return new Response<BookingVaccination> { Status = false, Message = "No nextbooking found in database!" };
            }

            return new Response<BookingVaccination> { Status = true, Message = "Peeking next book sucessfully!", Data = booking };

        }

    }

}

