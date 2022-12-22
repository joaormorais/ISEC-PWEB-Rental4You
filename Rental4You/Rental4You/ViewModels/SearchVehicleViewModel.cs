using Rental4You.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.ViewModels
{
    public class SearchVehicleViewModel
    {
        public List<Vehicle>? VehiclesList { get; set; }
        public int NumberOfResults { get; set; }
        [Display(Name = "VEHICLES SEARCH ...", Prompt = "introduce the text to search")]
        public string? TextToSearchName { get; set; }
        public string? TextToSearchLocation { get; set; }
        public string? TextToSearchType { get; set; }
    }
}
