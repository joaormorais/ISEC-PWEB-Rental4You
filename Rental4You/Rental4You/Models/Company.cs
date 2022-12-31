using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Display(Name = "Nome da empresa", Prompt = "Nome da empresa")]
        public string? Name { get; set; }
        [Display(Name = "Sigla da empresa", Prompt = "Sigla da empresa")]
        public string? Acronym { get; set; }
        public ICollection<Vehicle>? Reservations { get; set; }
        public ICollection<CompanyApplicationUser>? CompanyApplicationUsers { get; set; } 
        [Display(Name = "Disponível", Prompt = "Disponibilidade da empresa")]
        public bool Available { get; set; }
        [Display(Name = "Associar um novo funcionário à empresa:", Prompt = "Associe um novo funcionário à empresa")]
        public string? NewUserId { get; set; }
    }
}
