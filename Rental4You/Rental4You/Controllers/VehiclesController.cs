﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;

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

        // GET: Vehicles
        public async Task<IActionResult> Index(string? filter, string? sortOrder)
        {
            // send to the view a list of every company
            var listOfAllCompanies = new List<Company>();

            foreach (var item in _context.Company.ToList())
            {
                listOfAllCompanies.Add(item);
            }

            ViewBag.ListOfAllCompanies = listOfAllCompanies;

            // send every category of vehicle to the view to help filtering
            var listOfAllTypes = new List<String>();

            foreach (var item in _context.Vehicle.ToList())
            {
                if(!listOfAllTypes.Contains(item.Type))
                    listOfAllTypes.Add(item.Type);  
            }

            ViewBag.ListOfAllTypes = listOfAllTypes;

            // fazer uma condição caso o utilizador seja cliente e assim só vê o que é available (manter o que já está caso seja outra role qualquer)

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                if (await _userManager.IsInRoleAsync(currentUser, "Client"))
                {

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
            }
            
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

        // GET: Vehicles/Details/5
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

            return View(vehicle);
        }

        //GET: Vehicles/Create
        [Authorize(Roles = "Employee")]
            public IActionResult Create()
        {
            var currentUser = _userManager.GetUserAsync(User);
            var listOfCompaniesAssociatedToEmployee = new List<Company>();
            
            foreach(var item in _context.CompanyApplicationUsers.ToList())
            {
                if(item.ApplicationUserId.Equals(currentUser.Id))
                {
                    foreach(var item2 in _context.Company.ToList())
                    {

                        if(item2.Id==item.CompanyId)
                            listOfCompaniesAssociatedToEmployee.Add(item2);

                    }
                }
            }

            ViewData["ListOfCompanies"] = new SelectList(listOfCompaniesAssociatedToEmployee, "Id", "Name");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Location,Price,CompanyId,Available")] Vehicle vehicle)
        {
            
            ModelState.Remove(nameof(vehicle.Company));
            ModelState.Remove(nameof(vehicle.CompanyId));

            if (vehicle.CompanyId == null)
                ModelState.AddModelError("CompanyId","Não é possível criar um veículo sem associar uma empresa à qual você pertença!");

            var currentUser = _userManager.GetUserAsync(User);
            var listOfCompaniesAssociatedToEmployee = new List<Company>();

            foreach (var item in _context.CompanyApplicationUsers.ToList())
            {
                if (item.ApplicationUserId.Equals(currentUser.Id))
                {
                    foreach (var item2 in _context.Company.ToList())
                    {

                        if (item2.Id == item.CompanyId)
                            listOfCompaniesAssociatedToEmployee.Add(item2);

                    }
                }
            }

            ViewData["ListOfCompanies"] = new SelectList(listOfCompaniesAssociatedToEmployee, "Id", "Name");

            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
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

            ViewData["ListOfCompanies"] = new SelectList(_context.Company.ToList(), "Id", "Name");

            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Location,Price,CompanyId,Available")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

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

        // GET: Vehicles/Delete/5
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

            // verify if the vehicle has a reservation
            if(vehicle.Reservations!=null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
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
            if (vehicle != null)
            {
                _context.Vehicle.Remove(vehicle);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
          return _context.Vehicle.Any(e => e.Id == id);
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

    }
}
