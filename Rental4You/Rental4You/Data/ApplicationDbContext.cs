using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rental4You.Models;

namespace Rental4You.Data
{
    public class ApplicationDbContext : IdentityDbContext <ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Rental4You.Models.Vehicle> Vehicle { get; set; }
        public DbSet<Rental4You.Models.Reservation> Reservation { get; set; }
        public DbSet<Rental4You.Models.Company> Company { get; set; }
        public DbSet<Rental4You.Models.Admin> Admin { get; set; }
    }
}