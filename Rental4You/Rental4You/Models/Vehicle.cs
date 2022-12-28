using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        [Display(Name = "Nome do veículo", Prompt = "Nome do veículo")]
        public string? Name { get; set; }
        [Display(Name = "Categoria", Prompt = "Categoria do veículo")]
        public string? Type { get; set; }
        [Display(Name = "Localização", Prompt = "Localização do veículo")]
        public string? Location { get; set; }
        [Display(Name = "Preço", Prompt = "Preço do veículo")]
        public decimal Price { get; set; }
        [Display(Name = "Disponível", Prompt = "Disponibilidade do veículo")]
        public bool Available { get; set; }
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
    }
}
