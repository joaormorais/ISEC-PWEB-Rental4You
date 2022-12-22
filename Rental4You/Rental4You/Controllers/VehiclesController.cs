using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Location,Price,Company,CompanyAcronym")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
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
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Location,Price,Company,CompanyAcronym")] Vehicle vehicle)
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
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
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

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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

        public async Task<IActionResult> Search(string? TextToSearch)
        {
            SearchVehicleViewModel searchVM = new SearchVehicleViewModel();

            if (string.IsNullOrWhiteSpace(TextToSearch))
                searchVM.VehiclesList = await _context.Vehicle.ToListAsync();
            else
            {
                searchVM.VehiclesList = await _context.Vehicle.Where(c => c.Name.Contains(TextToSearch) || c.Location.Contains(TextToSearch) || c.Type.Contains(TextToSearch)).ToListAsync();
                searchVM.TextToSearch = TextToSearch;
            }

            searchVM.NumberOfResults = searchVM.VehiclesList.Count();
            return View(searchVM);

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(
            [Bind("TextToSearch")]
            SearchVehicleViewModel searchVehicle
            )
        {

            if (string.IsNullOrEmpty(searchVehicle.TextToSearch))
            {
                searchVehicle.VehiclesList = await _context.Vehicle.ToListAsync();
            }
            else
            {
                searchVehicle.VehiclesList = await _context.Vehicle.Where(c => c.Name.Contains(searchVehicle.TextToSearch) || c.Location.Contains(searchVehicle.TextToSearch) || c.Type.Contains(searchVehicle.TextToSearch)).ToListAsync();
                searchVehicle.TextToSearch = searchVehicle.TextToSearch;
            }

            searchVehicle.NumberOfResults = searchVehicle.VehiclesList.Count();
            return View(searchVehicle);

        }

        [HttpPost]
        public async Task<IActionResult> Index(string TextToSearch)
        {
            if (string.IsNullOrWhiteSpace(TextToSearch))
            {
                return View(await _context.Vehicle.ToListAsync());
            }
            else
            {
                var result = from c in _context.Vehicle
                                where c.Name.Contains(TextToSearch) || c.Location.Contains(TextToSearch) || c.Type.Contains(TextToSearch)
                             select c;
                return View(result);
            }
        }

    }
}
