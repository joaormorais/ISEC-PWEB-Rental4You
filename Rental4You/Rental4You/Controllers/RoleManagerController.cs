
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;

namespace Rental4You.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleManagerController : Controller // adicionar uma view para este controller e colocar na nav bar para o admin
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleManagerController(RoleManager<IdentityRole> roleManager)
        {
            //....
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            //....
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            //....
            if (roleName != null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = _roleManager.Roles.Where(r => r.Id == id).FirstOrDefault();
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }                
            return RedirectToAction("Index");
        }
    }
}
