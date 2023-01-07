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

        [Authorize(Roles = "Client,Employee")]
        public async Task<IActionResult> Index()
        {
            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name");

            var currentUser = await _userManager.GetUserAsync(User);

            if (await _userManager.IsInRoleAsync(currentUser,"Employee"))
            {
                var listOfAsssociatedCompaniesIds = new List<int?>();
                var currentEmployeeId = _userManager.GetUserId(User);
                foreach(var item in _context.CompanyApplicationUsers.ToList())
                {
                    if (item.ApplicationUserId == currentEmployeeId)
                        listOfAsssociatedCompaniesIds.Add(item.CompanyId);
                }

                var listOfReservationsFilteredIds = new List<int?>();
                var listOfReservationsFiltered = new List<Reservation>();

                foreach (var item in _context.Reservation.ToList())
                {
                    foreach(var item2 in _context.Vehicle.ToList())
                    {
                        if (item.VehicleId == item2.Id)
                        {
                            if (listOfAsssociatedCompaniesIds.Contains(item2.CompanyId))
                            {
                                listOfReservationsFiltered.Add(item);
                            }
                        }
                    }
                }

                return View(listOfReservationsFiltered);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create([Bind("Id,VehicleId,StartDate,EndDate")] Reservation reservation)
        {

            if (reservation.VehicleId == null)
                ModelState.AddModelError("VehicleId", "Não é possível criar uma reserva sem um veículo associado!");

            if (reservation.StartDate == null)
                ModelState.AddModelError("StartDate", "Não é possível criar uma reserva sem uma data de levantamento!");

            if (reservation.EndDate == null)
                ModelState.AddModelError("EndDate", "Não é possível criar uma reserva sem uma data de entrega!");


            // Verify if the dates are correct
            if (reservation.StartDate > reservation.EndDate)
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
                        ModelState.AddModelError("StartDate", "Já existe uma reserva para esse carro nessa altura");
                        break;
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

            // find the companie responsible for this reservation
            

            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name", reservation.VehicleId);
            ViewData["ListOfUsers1"] = new SelectList(await getClients(), "Id", "FirstName", reservation.ClientId);
            ViewData["ListOfUsers2"] = new SelectList(await getEmployeesForThisReservation(reservation), "Id", "FirstName");
            //ViewData["ListOfUsers3"] = new SelectList(await getEmployeesForThisReservation(id), "Id", "FirstName", reservation.RecieverEmployeeId);

            return View(reservation);
        }

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
            ViewData["ListOfUsers1"] = new SelectList(await getClients(), "Id", "FirstName", reservation.ClientId);
            ViewData["ListOfUsers2"] = new SelectList(await getEmployeesForThisReservation(reservation), "Id", "FirstName");
            //ViewData["ListOfUsers3"] = new SelectList(await getEmployeesForThisReservation(id), "Id", "FirstName", reservation.RecieverEmployeeId);

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

        private async Task<List<ApplicationUser>> getEmployeesForThisReservation(Reservation reservation)
        {

            //var reservation = await _context.Reservation.FindAsync(id);
            int? companyIdOfVehicle = -1;

            foreach (var item in _context.Vehicle.ToList())
            {
                if(reservation.VehicleId == item.Id)
                {
                    companyIdOfVehicle = item.CompanyId;
                    break;
                }

            }

            var listEmployeesForThisReservation = new List<String>();

            foreach(var item in _context.CompanyApplicationUsers.ToList())
            {
                if (item.CompanyId == companyIdOfVehicle)
                    listEmployeesForThisReservation.Add(item.ApplicationUserId);
            }

            var listOfEmployees = new List<ApplicationUser>();

            foreach (var employee in _context.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(employee, "Employee"))
                {
                    if (listEmployeesForThisReservation.Contains(employee.Id))
                        listOfEmployees.Add(employee);
                }
            }

            return listOfEmployees;
        }

        private async Task<List<ApplicationUser>> getClients()
        {
            var listOfClients = new List<ApplicationUser>();

            foreach (var client in _context.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(client, "Client"))
                {
                    listOfClients.Add(client);
                }
            }

            return listOfClients;
        }

    }
}
