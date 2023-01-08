using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Rental4You.Controllers
{

    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VehiclesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? filter, string? sortOrder)
        {
            if(_userManager.GetUserAsync(User).Result!=null)
                ViewBag.ListOfCompaniesAssociatedToEmployee = getListOfCompaniesAssociatedToEmployee();
            
            var listOfAllCompanies = new List<Company>();

            foreach (var item in _context.Company.ToList())
            {
                listOfAllCompanies.Add(item);
            }

            ViewBag.ListOfAllCompanies = listOfAllCompanies;

            var listOfAllTypes = new List<String>();

            foreach (var item in _context.Vehicle.ToList())
            {
                if(!listOfAllTypes.Contains(item.Type))
                    listOfAllTypes.Add(item.Type);  
            }

            ViewBag.ListOfAllTypes = listOfAllTypes;

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                if (!await _userManager.IsInRoleAsync(currentUser, "Client")){

                    ViewData["ListOfCompanies"] = new SelectList(_context.Company.ToList(), "Id", "Name");

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        var result = from c in _context.Vehicle
                                     where c.Type.Contains(filter) || c.Company.Name.Contains(filter)
                                     select c;

                        return View(result.ToList());
                    }
                    else if (!string.IsNullOrEmpty(sortOrder))
                    {

                        var vehiclesList = from C in _context.Vehicle
                                           select C;

                        switch (sortOrder)
                        {

                            case "price_down":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.Price);
                                break;

                            case "price_up":
                                vehiclesList = vehiclesList.OrderBy(s => s.Price);
                                break;

                        }

                        return View(vehiclesList.ToList());

                    }
                    else
                        return View(await _context.Vehicle.ToListAsync());

                }
            }


            var listOfAvailableCars = new List<Vehicle>();

                    foreach (var item in _context.Vehicle.ToList())
                    {
                        if (item.Available)
                            listOfAvailableCars.Add(item);

                    }

                    ViewData["ListOfCompanies"] = new SelectList(listOfAvailableCars, "Id", "Name");

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        var result = from c in _context.Vehicle
                                     where (c.Type.Contains(filter) || c.Company.Name.Contains(filter)) && c.Available == true
                                     select c;

                        return View(result.ToList());
                    }
                    else if (!string.IsNullOrEmpty(sortOrder))
                    {

                        var vehiclesList = from C in _context.Vehicle
                                           where C.Available == true
                                           select C;

                        switch (sortOrder)
                        {

                            case "price_down":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.Price);
                                break;

                            case "price_up":
                                vehiclesList = vehiclesList.OrderBy(s => s.Price);
                                break;

                        }

                        return View(vehiclesList.ToList());

                    }
                    else
                        return View(listOfAvailableCars.ToList());

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .Include(a => a.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            if (await _userManager.GetUserAsync(User) != null)
            {
                if (vehicle.Available == false && User.IsInRole("Client"))
                    return NotFound();
            }
            else
            {
                if(vehicle.Available == false)
                    return NotFound();
            }

            if (_userManager.GetUserAsync(User).Result != null)
                ViewBag.ListOfCompaniesAssociatedToEmployee = getListOfCompaniesAssociatedToEmployee();

            return View(vehicle);
        }

        [Authorize(Roles = "Employee")]
            public IActionResult Create()
        {
            ViewData["ListOfCompanies"] = new SelectList(getListOfCompaniesAssociatedToEmployee(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Location,Price,CompanyId,Available")] Vehicle vehicle)
        {

            if (vehicle.Name == null)
                ModelState.AddModelError("Name", "Não é possível criar um veículo sem definir um nome!");

            if (vehicle.Name == null)
                ModelState.AddModelError("Type", "Não é possível criar um veículo sem definir um/uma tipo/categoria!");

            if (vehicle.Name == null)
                ModelState.AddModelError("Location", "Não é possível criar um veículo sem definir uma localização!");

            if (vehicle.Name == null)
                ModelState.AddModelError("Price", "Não é possível criar um veículo sem definir um preço!");

            ModelState.Remove(nameof(vehicle.Company));
            ModelState.Remove(nameof(vehicle.CompanyId));

            var ListOfCompaniesAssociatedToEmployee = getListOfCompaniesAssociatedToEmployee();

            ViewData["ListOfCompanies"] = new SelectList(ListOfCompaniesAssociatedToEmployee, "Id", "Name");

            if (vehicle.CompanyId == null)
                ModelState.AddModelError("CompanyId", "Não é possível criar um veículo sem associar uma empresa à qual você pertença!");

            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(vehicle);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            if (!getListOfCompaniesAssociatedToEmployeeIds().Contains(vehicle.CompanyId))
                return NotFound();

            ViewData["ListOfCompanies"] = new SelectList(_context.Company.ToList(), "Id", "Name");

            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Location,Price,CompanyId,Available")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (!getListOfCompaniesAssociatedToEmployeeIds().Contains(vehicle.CompanyId))
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ListOfCompanies"] = new SelectList(_context.Company.ToList(), "Id", "Name");

            return View(vehicle);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vehicle == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vehicle'  is null.");
            }

            var vehicle = await _context.Vehicle.FindAsync(id);

            foreach(var item in _context.Reservation.ToList())
            {
                if (item.VehicleId == vehicle.Id)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            if (vehicle != null)
            {
                _context.Vehicle.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string? TextToSearchName, string? TextToSearchLocation, int? TextToSearchCompany)
        {

            ViewData["ListOfCompanies"] = new SelectList(_context.Company.ToList(), "Id", "Name");
            VehicleSearchViewModel searchVM = new VehicleSearchViewModel();

            if (string.IsNullOrWhiteSpace(TextToSearchName) && string.IsNullOrWhiteSpace(TextToSearchLocation) && TextToSearchCompany == null)
            {
                searchVM.ListOfVehicles = await _context.Vehicle.ToListAsync();
            }
            else if (!string.IsNullOrWhiteSpace(TextToSearchName) && string.IsNullOrWhiteSpace(TextToSearchLocation) && TextToSearchCompany == null)
            {
                searchVM.ListOfVehicles = await _context.Vehicle.Where(c => c.Name.Contains(TextToSearchName)).ToListAsync();
                searchVM.TextToSearchName = TextToSearchName;
            }
            else if (!string.IsNullOrWhiteSpace(TextToSearchName) && !string.IsNullOrWhiteSpace(TextToSearchLocation) && TextToSearchCompany == null)
            {
                searchVM.ListOfVehicles = await _context.Vehicle.Where(c => c.Name.Contains(TextToSearchName) && c.Location.Contains(TextToSearchLocation)).ToListAsync();
                searchVM.TextToSearchName = TextToSearchName;
                searchVM.TextToSearchLocation = TextToSearchLocation;
            }
            else if (!string.IsNullOrWhiteSpace(TextToSearchName) && string.IsNullOrWhiteSpace(TextToSearchLocation) && TextToSearchCompany != null)
            {
                searchVM.ListOfVehicles = await _context.Vehicle.Where(c => c.Name.Contains(TextToSearchName) && c.CompanyId == TextToSearchCompany).ToListAsync();
                searchVM.TextToSearchName = TextToSearchName;
                searchVM.TextToSearchCompany = TextToSearchCompany;
            }
            else if (string.IsNullOrWhiteSpace(TextToSearchName) && !string.IsNullOrWhiteSpace(TextToSearchLocation) && TextToSearchCompany == null)
            {
                searchVM.ListOfVehicles = await _context.Vehicle.Where(c => c.Location.Contains(TextToSearchLocation)).ToListAsync();
                searchVM.TextToSearchLocation = TextToSearchLocation;
            }
            else if (string.IsNullOrWhiteSpace(TextToSearchName) && !string.IsNullOrWhiteSpace(TextToSearchLocation) && TextToSearchCompany != null)
            {
                searchVM.ListOfVehicles = await _context.Vehicle.Where(c => c.Location.Contains(TextToSearchLocation) && c.CompanyId == TextToSearchCompany).ToListAsync();
                searchVM.TextToSearchLocation = TextToSearchLocation;
                searchVM.TextToSearchCompany = TextToSearchCompany;
            }
            else if (string.IsNullOrWhiteSpace(TextToSearchName) && string.IsNullOrWhiteSpace(TextToSearchLocation) && TextToSearchCompany != null)
            {
                searchVM.ListOfVehicles = await _context.Vehicle.Where(c => c.CompanyId == TextToSearchCompany).ToListAsync();
                searchVM.TextToSearchCompany = TextToSearchCompany;
            }
            else if (!string.IsNullOrWhiteSpace(TextToSearchName) && !string.IsNullOrWhiteSpace(TextToSearchLocation) && TextToSearchCompany != null)
            {
                searchVM.ListOfVehicles = await _context.Vehicle.Where(c => c.Name.Contains(TextToSearchName) && c.Location.Contains(TextToSearchLocation) && c.CompanyId == TextToSearchCompany).ToListAsync();
                searchVM.TextToSearchName = TextToSearchName;
                searchVM.TextToSearchLocation = TextToSearchLocation;
                searchVM.TextToSearchCompany = TextToSearchCompany;
            }

            searchVM.NumResults = searchVM.ListOfVehicles.Count();
            return View(searchVM);

        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.Id == id);
        }

        public List<Company> getListOfCompaniesAssociatedToEmployee()
        {

            var listOfCompaniesAssociatedToEmployee = new List<Company>();
            var listOfAssociations = _context.CompanyApplicationUsers.ToList();
            var listOfCompanies = _context.Company.ToList();
            var currentUser = _userManager.GetUserAsync(User).Result;


            foreach (var item in listOfAssociations)
            {
                if (item.ApplicationUserId.Equals(currentUser.Id))
                {
                    foreach (var item2 in listOfCompanies)
                    {

                        if (item2.Id == item.CompanyId)
                            listOfCompaniesAssociatedToEmployee.Add(item2);

                    }
                }
            }

            return listOfCompaniesAssociatedToEmployee;

        }

        public List<int?> getListOfCompaniesAssociatedToEmployeeIds()
        {

            var listOfCompaniesAssociatedToEmployee = new List<Company>();
            var listOfAssociations = _context.CompanyApplicationUsers.ToList();
            var listOfCompanies = _context.Company.ToList();
            var currentUser = _userManager.GetUserAsync(User).Result;


            foreach (var item in listOfAssociations)
            {
                if (item.ApplicationUserId.Equals(currentUser.Id))
                {
                    foreach (var item2 in listOfCompanies)
                    {

                        if (item2.Id == item.CompanyId)
                            listOfCompaniesAssociatedToEmployee.Add(item2);

                    }
                }
            }

            var listOfCompaniesAssociatedToEmployeeIds = new List<int?>();

            foreach (var item in listOfCompaniesAssociatedToEmployee)
            {
                listOfCompaniesAssociatedToEmployeeIds.Add(item.Id);
            }

            return listOfCompaniesAssociatedToEmployeeIds;

        }

    }
}
