using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;
using static System.Formats.Asn1.AsnWriter;

namespace Rental4You.Controllers
{
    
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager { get; set; }

        public CompaniesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            UserManager = userManager;
        }

        public async Task clearNulls()
        {
            foreach(var item in _context.CompanyApplicationUsers.ToList())
            {
                if(item.ApplicationUserId == null)
                {
                    _context.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Index(bool? filter, string? sortOrder)
        {

            Task myTask = clearNulls();
            myTask.Wait();

            var currentUser = await _userManager.GetUserAsync(User);

            if(currentUser != null)
            {
                if(await _userManager.IsInRoleAsync(currentUser, "Manager"))
                {

                    var listOfCompaniesAssociatedToManager = new List<Company>();

                    foreach (var item in _context.CompanyApplicationUsers.ToList())
                    {
                        if (item.ApplicationUserId.Equals(currentUser.Id))
                        {
                            foreach (var item2 in _context.Company.ToList())
                            {

                                if (item2.Id == item.CompanyId)
                                    listOfCompaniesAssociatedToManager.Add(item2);

                            }
                        }
                    }

                    ViewData["ListOfCompanies"] = new SelectList(listOfCompaniesAssociatedToManager, "Id", "Name");

                    if (filter != null)
                    {

                        var result = from c in listOfCompaniesAssociatedToManager
                                     where (c.Available == filter)
                                     select c;

                        return View(result.ToList());

                    }
                    else if (!string.IsNullOrEmpty(sortOrder))
                    {

                        switch (sortOrder)
                        {
                            case "foward":
                                return View(listOfCompaniesAssociatedToManager.OrderBy(s => s.Name));

                            case "back":
                                return View(listOfCompaniesAssociatedToManager.OrderByDescending(s => s.Name));
                        }

                    }

                    return View(listOfCompaniesAssociatedToManager);
                }
            }

            if(filter != null)
            {

                var result = from c in _context.Company
                             where (c.Available == filter)
                             select c;

                return View(result.ToList());

            }else if (!string.IsNullOrEmpty(sortOrder))
            {

                switch(sortOrder)
                {
                    case "foward":
                        return View(_context.Company.OrderBy(s => s.Name).ToList());

                    case "back":
                        return View(_context.Company.OrderByDescending(s => s.Name).ToList());
                }

            }

            return View(await _context.Company.ToListAsync());
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            var listOfVehiclesCompaies = new List<Vehicle>();

            foreach (var item in _context.Vehicle.ToList())
            {
                if (item.CompanyId == company.Id)
                    listOfVehiclesCompaies.Add(item);
            }

            ViewBag.ListOfCarsCompany = listOfVehiclesCompaies;

            var listOfEmployeesAssociated = new List<String>();

            foreach (var item in _context.CompanyApplicationUsers.ToList())
            {
                if (item.CompanyId == company.Id)
                    foreach(var item2 in _context.Users.ToList())
                    {
                        if (item2.Id == item.ApplicationUserId)
                            listOfEmployeesAssociated.Add(item2.FirstName);
                    }
            }

            ViewBag.listOfEmployeesAssociated = listOfEmployeesAssociated;

            return View(company);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Acronym,Available")] Company company)
        {

            foreach (var item in _context.Company.ToList())
            {
                if (item.Name.Equals(company.Name))
                    ModelState.AddModelError("Name", "Já existe uma empresa com esse nome.");
                else if (item.Acronym.Equals(company.Acronym))
                    ModelState.AddModelError("Acronym", "Já existe uma empresa com esse acrónimo.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();

                // Create the Manager
                var defaultManager = new ApplicationUser
                {
                    UserName = "manager" + company.Name + "@isec.pt",
                    Email = "manager" + company.Name + "@isec.pt",
                    FirstName = "Manager of " + company.Name,
                    LastName = company.Acronym,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var userRepeated = false;

                foreach (var item in _userManager.Users.ToList())
                    if (item.FirstName.Equals(defaultManager.FirstName))
                        userRepeated = true;

                if (!userRepeated)
                {
                    await UserManager.CreateAsync(defaultManager, "Facil.123");
                    await UserManager.AddToRoleAsync(defaultManager, Roles.Manager.ToString());
                }

                var manager = await _userManager.FindByEmailAsync(defaultManager.Email);
                CompanyApplicationUser companyApplicationUser = new CompanyApplicationUser();
                companyApplicationUser.CompanyId = company.Id;
                companyApplicationUser.ApplicationUserId = manager.Id;
                _context.Add(companyApplicationUser);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        [Authorize(Roles = "Manager,Admin")]
        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var foundAssociation = false;

            foreach(var item in _context.CompanyApplicationUsers.ToList())
            {
                if (currentUser.Id.Equals(item.ApplicationUserId))
                    if (company.Id == item.CompanyId)
                    {
                        foundAssociation = true;
                        break;
                    }
            }

            if (!foundAssociation)
                return NotFound();

            // find only employees
            var listOfEmployees = new List<ApplicationUser>();

            foreach (var employee in _context.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(employee, "Employee"))
                {
                    listOfEmployees.Add(employee);
                }
            }

            CompanyApplicationUser companyApplicationUser = new CompanyApplicationUser();
            companyApplicationUser.CompanyId = company.Id;
            ViewData["ListOfUsers"] = new SelectList(listOfEmployees, "Id", "FirstName", company.NewUserId);
            _context.Add(companyApplicationUser);
            await _context.SaveChangesAsync(); 

            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Acronym,Available,NewUserId")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var foundAssociation = false;

            foreach (var item in _context.CompanyApplicationUsers.ToList())
            {
                if (currentUser.Id.Equals(item.ApplicationUserId))
                    if (company.Id == item.CompanyId)
                    {
                        foundAssociation = true;
                        break;
                    }
            }

            if (!foundAssociation)
                return NotFound();

            ModelState.Remove(nameof(company.CompanyApplicationUsers));

            // find only employees
            var listOfEmployees = new List<ApplicationUser>();

            foreach (var employee in _context.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(employee, "Employee"))
                {
                    listOfEmployees.Add(employee);
                }
            }


            CompanyApplicationUser companyApplicationUserTemp = new CompanyApplicationUser();
            ModelState.Remove(nameof(companyApplicationUserTemp.ApplicationUserId));
            ModelState.Remove(nameof(companyApplicationUserTemp.ApplicationUser));
            ModelState.Remove(nameof(companyApplicationUserTemp.CompanyId));
            ModelState.Remove(nameof(companyApplicationUserTemp.Company));
            companyApplicationUserTemp.CompanyId = company.Id;
            ViewData["ListOfUsers"] = new SelectList(listOfEmployees, "Id", "FirstName", company.NewUserId);

            foreach (var item in _context.CompanyApplicationUsers.ToList())
            {

                if(item.CompanyId == company.Id && item.ApplicationUserId == company.NewUserId) // it means the this user is already associated
                {
                    
                    foreach(var item2 in _context.CompanyApplicationUsers.ToList())
                    {
                        if (item2.CompanyId == company.Id && item2.ApplicationUserId == null)
                        {
                            _context.Remove(item2);
                            await _context.SaveChangesAsync();
                        }
                    }
                    break; // make sure that we don't add him and that we remove the temporary user that was created in the database
                }

                if (item.CompanyId == company.Id && item.ApplicationUserId == null) { // siiiiiiiiiiiiiiiiiiiiiiiiiiiiiiim!
                    item.ApplicationUserId = company.NewUserId;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Company == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Company'  is null.");
            }
            var company = await _context.Company.FindAsync(id);
            if (company != null)
            {
                _context.Company.Remove(company);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return _context.Company.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Search(string? TextToSearchName)
        {
            Task myTask = clearNulls();
            myTask.Wait();

            CompaniesSearchViewModel searchVM = new CompaniesSearchViewModel();

            if(string.IsNullOrWhiteSpace(TextToSearchName))
                searchVM.ListOfCompanies = await _context.Company.ToListAsync();
            else
            {
                searchVM.ListOfCompanies = await _context.Company.Where(c => c.Name.Contains(TextToSearchName)).ToListAsync();
                searchVM.TextToSearchName = TextToSearchName;
            }

            searchVM.NumResults = searchVM.ListOfCompanies.Count();
            return View(searchVM);

        }
    }
}
