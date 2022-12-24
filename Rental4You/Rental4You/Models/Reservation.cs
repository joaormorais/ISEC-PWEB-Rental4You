namespace Rental4You.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public Vehicle? Vehicle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Confirmed { get; set; }
        // cliente
        public Decimal KmsStart { get; set; }
        public bool DamageStart { get; set; }
        public string? ObservationsStart { get; set; }
        // funcionario de levantamento
        public Decimal KmsEnd { get; set; }
        public bool DamageEnd { get; set; }
        public string? ObservationsEnd { get; set; }
        // funcionario de entrega
        // possiveis fotos do estrago

    }
}
