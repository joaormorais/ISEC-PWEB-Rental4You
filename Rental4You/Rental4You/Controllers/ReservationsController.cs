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
            //System.Diagnostics.Trace.WriteLine("This is a log message -----------------------------------------------------------------------------------");
            //System.Diagnostics.Trace.WriteLine(currentUser.UserName);
            //System.Diagnostics.Trace.WriteLine("This is a log message -----------------------------------------------------------------------------------");

            if (await _userManager.IsInRoleAsync(currentUser,"Employee"))
            {
                var applicationDbContext = _context.Reservation.Include(a => a.Vehicle).Include(a => a.Users);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var reservationsFiltered = _context.Reservation.
                Include(a => a.Vehicle).
                Include(a => a.Users).
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
                .Include(a => a.Users)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            ViewBag.clientteste = reservation.Users.ElementAt(0);
            if(reservation.Users.Count >= 2)
            ViewBag.func1 = reservation.Users.ElementAt(1);
            if(reservation.Users.Count == 3)
            ViewBag.func2 = reservation.Users.ElementAt(2);


            System.Diagnostics.Trace.WriteLine("This is a log message -----------------------------------------------------------------------------------");
            System.Diagnostics.Trace.WriteLine(reservation.Users.Count);
            System.Diagnostics.Trace.WriteLine("This is a log message -----------------------------------------------------------------------------------");

            return View(reservation);
        }

        // GET: Reservations/Create
        [Authorize(Roles = "Client")]
        public IActionResult Create()
        {
            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name");
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

            ModelState.Remove(nameof(reservation.Vehicle));
            ModelState.Remove(nameof(reservation.VehicleId));

            // remove from the ModelState the propreties about the ApplicationUser
            ModelState.Remove(nameof(reservation.Users));
            ModelState.Remove(nameof(reservation.ClientId));
            ModelState.Remove(nameof(reservation.DelieverEmployeeId));
            ModelState.Remove(nameof(reservation.RecieverEmployeeId));

            // the ApplicationUserId is the Id of the current user
            reservation.ClientId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            reservation.Users = new List<ApplicationUser>() { };
            reservation.Users.Add(currentUser);
            reservation.Ended = false;
            reservation.Confirmed = false;

            // the rest of the attributes of the class Reservation go empty because they are suppose to be changed in the edit settings

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name");
            
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

            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name", reservation.VehicleId);
            ViewData["ListOfUsers1"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.ClientId);
            ViewData["ListOfUsers2"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.DelieverEmployeeId);
            ViewData["ListOfUsers3"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.RecieverEmployeeId);
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

            /*
            var currentUser = await _userManager.GetUserAsync(User);
            reservation.Users = new List<ApplicationUser>() { };
            reservation.Users.Add(currentUser);
             */

            // sabber como alterar por ID   pos 0 1 e 2!!!!!!!!!!!!!

           


            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name", reservation.VehicleId);
            ViewData["ListOfUsers1"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.ClientId);
            ViewData["ListOfUsers2"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.DelieverEmployeeId);
            ViewData["ListOfUsers3"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.RecieverEmployeeId);
            
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
