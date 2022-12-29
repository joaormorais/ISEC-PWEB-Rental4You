using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;

namespace Rental4You.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        [Authorize(Roles = "Client,Employee")]
        public async Task<IActionResult> Index()
        {
            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name");

            var currentUser = await _userManager.GetUserAsync(User);

            if (await _userManager.IsInRoleAsync(currentUser,"Employee"))
            {
                var applicationDbContext = _context.Reservation.Include(a => a.Vehicle);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var reservationsFiltered = _context.Reservation.
                Include(a => a.Vehicle).
                Where(a => a.ClientId == _userManager.GetUserId(User)).
                Where(a => a.Ended == false);

                return View(await reservationsFiltered.ToListAsync());
            }

        }

        // GET: Reservations/Details/5
        [Authorize(Roles = "Client, Employee")]
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            // fazer o codigo aqui para procurar pelos users

            var client = await _userManager.FindByIdAsync(reservation.ClientId);
            ViewBag.showClient = client.FirstName;

            if (reservation.DelieverEmployeeId != null)
            {
                var delieverEmployee = await _userManager.FindByIdAsync(reservation.DelieverEmployeeId);
                ViewBag.showDelieverEmployee = delieverEmployee.FirstName;
            }

            if (reservation.RecieverEmployeeId != null)
            {
                var receiverEmployee = await _userManager.FindByIdAsync(reservation.RecieverEmployeeId);
                ViewBag.showReceiverEmployee = receiverEmployee.FirstName;
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        [Authorize(Roles = "Client")]
        public IActionResult Create()
        {
            var listOfAvailableCars = new List<Vehicle>();

            foreach (var item in _context.Vehicle.ToList())
            {
                if (item.Available)
                    listOfAvailableCars.Add(item);

            }

            ViewData["ListOfVehicles"] = new SelectList(listOfAvailableCars, "Id", "Name");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create([Bind("Id,VehicleId,StartDate,EndDate")] Reservation reservation)
        {

            // Verify if the dates are correct
            if(reservation.StartDate > reservation.EndDate)
                ModelState.AddModelError("StartDate", "A data de inicio não pode ser maior que a data de fim");

            // Verify if the user can make a reservation on those dates 
            foreach(var item in _context.Reservation.ToList())
            {
                if (item.VehicleId == reservation.VehicleId)
                {
                    // example: reservation1(r1) and reservation2(r2)
                    // a reservation doesn't go above each other
                    // r2.StartDate > r1.EndDate || r2.EndDate < r1.StartDate
                    // in this condition we want the opposite
                    if(!(reservation.StartDate>item.EndDate || reservation.EndDate < item.StartDate))
                    {
                        ModelState.AddModelError("StartDate", "Já existe uma reserva para esse carro nessa altura");
                    }
                }
            }

            if (ModelState.IsValid)
            {

            // remove from the ModelState the propreties about the Vehicle
            ModelState.Remove(nameof(reservation.Vehicle));
            ModelState.Remove(nameof(reservation.VehicleId));

            // remove from the ModelState the propreties about the ApplicationUser
            ModelState.Remove(nameof(reservation.Users));
            ModelState.Remove(nameof(reservation.ClientId));
            ModelState.Remove(nameof(reservation.DelieverEmployeeId));
            ModelState.Remove(nameof(reservation.RecieverEmployeeId));

            // the ApplicationUserId is the Id of the current user
            reservation.ClientId = _userManager.GetUserId(User);
            reservation.Ended = false;
            reservation.Confirmed = false;
            reservation.DamageStart = false;
            reservation.DamageEnd = false;
            reservation.ActualDate = DateTime.Now;

            // the rest of the attributes of the class Reservation go empty because they are suppose to be changed in the edit settings

            
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Adding cars to the ViewData
            var listOfAvailableCars = new List<Vehicle>();

            foreach (var item in _context.Vehicle.ToList())
            {
                if (item.Available)
                    listOfAvailableCars.Add(item);

            }

            // View Data with the cars marked as available by their company
            ViewData["ListOfVehicles"] = new SelectList(listOfAvailableCars, "Id", "Name");


            return View(reservation);
        }

        // GET: Reservations/Edit/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            // find only clients
            var listOfClients = new List<ApplicationUser>();

            foreach (var client in _context.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(client, "Client"))
                {
                    listOfClients.Add(client);
                }
            }

            // find only employees 
            var listOfEmployees = new List<ApplicationUser>();

            foreach(var employee in _context.Users.ToList())
            {
                if(await _userManager.IsInRoleAsync(employee, "Employee"))
                {
                    listOfEmployees.Add(employee);
                }
            }
            

            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name", reservation.VehicleId);
            ViewData["ListOfUsers1"] = new SelectList(listOfClients, "Id", "FirstName", reservation.ClientId);
            ViewData["ListOfUsers2"] = new SelectList(listOfEmployees, "Id", "FirstName", reservation.DelieverEmployeeId);
            ViewData["ListOfUsers3"] = new SelectList(listOfEmployees, "Id", "FirstName", reservation.RecieverEmployeeId);

            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,VehicleId,StartDate,EndDate,DelieverEmployeeId,Confirmed,KmsStart,DamageStart,ObservationsStart,RecieverEmployeeId,KmsEnd,DamageEnd,ObservationsEnd,DamageImages,Users")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(reservation.Vehicle));
            ModelState.Remove(nameof(reservation.VehicleId));

            // remove from the ModelState the propreties about the ApplicationUser
            ModelState.Remove(nameof(reservation.Users));
            ModelState.Remove(nameof(reservation.ClientId));
            ModelState.Remove(nameof(reservation.DelieverEmployeeId));
            ModelState.Remove(nameof(reservation.RecieverEmployeeId));

            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name", reservation.VehicleId);
            ViewData["ListOfUsers1"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.ClientId);
            ViewData["ListOfUsers2"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.DelieverEmployeeId);
            ViewData["ListOfUsers3"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.RecieverEmployeeId);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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

            return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservation == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservation'  is null.");
            }
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return _context.Reservation.Any(e => e.Id == id);
        }
    }
}
