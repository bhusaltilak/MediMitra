using MediMitra.DTO;
using MediMitra.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MediMitra.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<RegisterModel> registerModels { get; set; }
        public DbSet<Vaccination> vaccinations { get; set; }
        public DbSet<BookingVaccination> bookingVaccinations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookingVaccination>()
                .HasOne(b => b.Vaccination)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VaccinationId);

            modelBuilder.Entity<BookingVaccination>()
        .Property(b => b.Status)
        .HasConversion<string>();

            modelBuilder.Entity<Vaccination>()
     .Property(b => b.Status)
     .HasConversion<string>();

            SeedUser(modelBuilder);
         

        }
        private static void SeedUser(ModelBuilder modelBuilder)
        {

            //seed User
            modelBuilder.Entity<RegisterModel>().HasData(
               new RegisterModel { Id = 1, Username = "Durga Khanal", Email = "khanalvaidurga71@gmail.com", Password = BCrypt.Net.BCrypt.HashPassword("durga123"), Role = "Admin" },
               new RegisterModel { Id = 2, Username = "Sunil Dumre", Email = "sumildumre555@gmail.com", Password = BCrypt.Net.BCrypt.HashPassword("sunil123"), Role = "Moderator" },
               new RegisterModel { Id = 3, Username = "Tilak Bhusal", Email = "bhushaltilak9@gmail.com", Password = BCrypt.Net.BCrypt.HashPassword("tilak123"),Role = "Moderator" }
               );
        }
    }
}
