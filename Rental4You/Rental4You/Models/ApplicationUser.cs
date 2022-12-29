using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Primeiro Nome", Prompt = "Primeiro nome do utilizador")]
        public string? FirstName { get; set; }
        [Display(Name = "Último Nome", Prompt = "Último nome do utilizador")]
        public string? LastName { get; set; }
        [Display(Name = "Data de nascimento", Prompt = "Data de nascimento do utilizador")]
        public DateTime DateBirth { get; set; }

        //public byte[] Avatar { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<Company>? Companies { get; set; }
        public DateTime? ActualTime { get; set; } = DateTime.Now;
    }
}
