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
        [Authorize(Roles = "Client, Employee")]
        public async Task<IActionResult> Index()
        {
            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name");
            return View(await _context.Reservation.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,VehicleId")] Reservation reservation)
        {

            ViewData["ListaOfVehicles"] =
       new SelectList(_context.Vehicle.ToList(), "Id", "Name");
            ModelState.Remove(nameof(reservation.Vehicle));

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            //ViewData["ListOfUsers"] = new SelectList(_userManager.Users.ToList(), "Id", "Name", reservation.Users);
            ViewData["ListOfUsers"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.Users);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleId,StartDate,EndDate,Confirmed,KmsStart,DamageStart,ObservationsStart,KmsEnd,DamageEnd,ObservationsEnd,DamageImages,Users")] Reservation reservation)
        {

            ViewData["ListOfUsers"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", reservation.Users);

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

            ViewData["ListOfVehicles"] = new SelectList(_context.Vehicle.ToList(), "Id", "Name", reservation.VehicleId);
            //ViewData["ListOfUsers"] = new SelectList(_userManager.Users.ToList(), "Id", "Name", reservation.Users);
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
