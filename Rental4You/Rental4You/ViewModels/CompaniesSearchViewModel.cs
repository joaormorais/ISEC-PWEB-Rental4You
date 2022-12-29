using Rental4You.Models;

namespace Rental4You.ViewModels
{
    public class CompaniesSearchViewModel
    {
        public List<Company> ListOfCompanies { get; set; }
        public int NumResults { get; set; }
        public string TextToSearchName { get; set; }
    }
}
