namespace Rental4You.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public Vehicle? Vehicle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Confirmed { get; set; }
        public Decimal KmsStart { get; set; }
        public bool DamageStart { get; set; }
        public string? ObservationsStart { get; set; }
        public Decimal KmsEnd { get; set; }
        public bool DamageEnd { get; set; }
        public string? ObservationsEnd { get; set; }
        // possiveis fotos do estrago
        public byte[] DamageImages { get; set; }
        // de acordo com as posicoes --> 0: cliente 1: funcionario de levantamento 2: funcionario de entrega
        public ICollection<ApplicationUser>? Users { get; set; }

    }
}
