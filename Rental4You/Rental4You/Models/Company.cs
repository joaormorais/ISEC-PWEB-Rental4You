using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Display(Name = "Company Name", Prompt = "Name of the company")]
        public string? Name { get; set; }
        public string? Acronym { get; set; }
        public ICollection<Vehicle>? Reservations { get; set; }
        // Collection of clients and managers
        //public List<int> UsersId { get; set; }
        public ICollection<ApplicationUser>? Users { get; set; }
        public bool Available { get; set; }
    }
}
