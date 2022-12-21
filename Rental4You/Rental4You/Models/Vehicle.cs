namespace Rental4You.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Location { get; set; }
        public decimal Price { get; set; }
        public string? Company { get; set; }
        public string? CompanyAcronym { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
    }
}
