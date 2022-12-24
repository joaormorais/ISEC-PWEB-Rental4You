using Microsoft.AspNetCore.Identity;

namespace Rental4You.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateBirth { get; set; }

        //public byte[] Avatar { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
    }
}
