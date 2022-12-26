using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Name", Prompt = "Select the name of the user")]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateBirth { get; set; }

        //public byte[] Avatar { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<Company>? Companies { get; set; }
    }
}
