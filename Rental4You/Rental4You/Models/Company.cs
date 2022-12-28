﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Display(Name = "Nome da empresa", Prompt = "Nome da empresa")]
        public string? Name { get; set; }
        [Display(Name = "Sigla da empresa", Prompt = "Sigla da empresa")]
        public string? Acronym { get; set; }
        public ICollection<Vehicle>? Reservations { get; set; }
        // Collection of clients and managers
        //public List<int> UsersId { get; set; }
        public ICollection<ApplicationUser>? Users { get; set; }
        [Display(Name = "Disponível", Prompt = "Disponibilidade da empresa")]
        public bool Available { get; set; }
    }
}
