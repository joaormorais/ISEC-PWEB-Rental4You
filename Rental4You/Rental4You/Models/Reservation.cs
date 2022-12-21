namespace Rental4You.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime inDate { get; set; }
        public DateTime offDate { get; set; }
        public Vehicle? Vehicle { get; set; }
    }
}
