namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Acronym { get; set; }
        public ICollection<Vehicle>? Reservations { get; set; }
        // coleção de funcionarios
        //public ICollection<AspNetUsers>? Users { get; set; }
        // coleção de gestores
        public bool Available { get; set; }
    }
}
