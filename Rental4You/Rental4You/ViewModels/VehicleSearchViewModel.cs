using Rental4You.Models;

namespace Rental4You.ViewModels
{
    public class VehicleSearchViewModel
    {
        public List<Vehicle> ListOfVehicles { get; set; }
        public int NumResults { get; set; }
        public string TextToSearchName { get; set; }
        public string TextToSearchLocation { get; set; }
        public string TextToSearchCompany { get; set; }
    }
}
