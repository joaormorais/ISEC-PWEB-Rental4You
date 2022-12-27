﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Confirmed { get; set; }
        public Decimal? KmsStart { get; set; }
        public bool? DamageStart { get; set; }
        public string? ObservationsStart { get; set; }
        public Decimal? KmsEnd { get; set; }
        public bool? DamageEnd { get; set; }
        public string? ObservationsEnd { get; set; }
        // possiveis fotos do estrago
        public byte[]? DamageImages { get; set; }
        // de acordo com as posicoes --> 0: cliente 1: funcionario de levantamento 2: funcionario de entrega
        [Display(Name = "~Clienteeeeeeeeeeeee", Prompt = "Select the name of the user")]
        public int? ClientId { get; set; }
        public int? DelieverEmployeeId { get; set; }
        public int? RecieverEmployeeId { get; set; }
        public ICollection<ApplicationUser>? Users { get; set; }
        /*public int? ClientId { get; set; }
        public ApplicationUser? Client { get; set; }
        public int? DelieverEmployeeId { get; set; }
        public ApplicationUser? DelieverEmployee { get; set; }
        public int? RecieverEmployeeId { get; set; }
        public ApplicationUser? RecieverEmployee { get; set; }*/

    }
}
