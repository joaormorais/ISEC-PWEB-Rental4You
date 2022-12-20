namespace Rental4You.Models
{
    public class Vehicle
    {
        public int ID { get; set; }
        public string? Type { get; set; }
        public string? Location { get; set; }
        public DateTime DatePickUp { get; set; }
        public DateTime DateDropOff { get; set; }
        public decimal Price { get; set; }
        public string? Company { get; set; }
    }
}
