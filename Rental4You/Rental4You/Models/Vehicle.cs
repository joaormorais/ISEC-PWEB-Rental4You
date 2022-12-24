namespace Rental4You.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Location { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public Company? Company { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
    }
}
