using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }
        [Display(Name = "Data de levantamento", Prompt = "Data de levantamento do veículo")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "Data de entrega", Prompt = "Data de entrega do veículo")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Confirmação", Prompt = "Confirmação da reserva")]
        public bool Confirmed { get; set; }
        [Display(Name = "Km/h no levantamento", Prompt = "Km/h no levantamento do veículo")]
        public Decimal? KmsStart { get; set; }
        [Display(Name = "Danos no levantamento", Prompt = "Danos no levantamento do veículo")]
        public bool DamageStart { get; set; }
        [Display(Name = "Observações no levantamento", Prompt = "Observações no levantamento do veículo")]
        public string? ObservationsStart { get; set; }
        [Display(Name = "Km/h na entrega", Prompt = "Km/h na entrega do veículo")]
        public Decimal? KmsEnd { get; set; }
        [Display(Name = "Danos na entrega", Prompt = "Danos na entrega do veículo")]
        public bool DamageEnd { get; set; }
        [Display(Name = "Observações na entrega", Prompt = "Observações na entrega do veículo")]
        public string? ObservationsEnd { get; set; }
        [Display(Name = "Fotografia dos danos", Prompt = "Fotografia dos danos na entrega do veículo")]
        public byte[]? DamageImages { get; set; }
        // de acordo com as posicoes --> 0: cliente 1: funcionario de levantamento 2: funcionario de entrega
        [Display(Name = "Cliente", Prompt = "Nome do cliente associado à reserva")]
        public string? ClientId { get; set; }
        [Display(Name = "Funcionário do levantamento", Prompt = "Nome do funcionário do levantamento do veículo associado à reserva")]
        public string? DelieverEmployeeId { get; set; }
        [Display(Name = "Funcionário da entrega", Prompt = "Nome do funcionário da entrega do veículo associado à reserva")]
        public string? RecieverEmployeeId { get; set; }
        public ICollection<ApplicationUser>? Users { get; set; }
        public bool Ended { get; set; }

    }
}
