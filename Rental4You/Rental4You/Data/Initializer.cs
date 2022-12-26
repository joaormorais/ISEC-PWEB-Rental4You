using Microsoft.AspNetCore.Identity;
using Rental4You.Models;
using System;

namespace Rental4You.Data
{

    public enum Roles
    {
        Admin,
        Manager,
        Employee,
        Client
    }

    public static class Initializer
    {

        public static async Task CreateFirstData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            
            // Add the default roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Client.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Employee.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Manager.ToString()));

            // Create the Admin
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@localhost.com",
                Email = "admin@localhost.com",
                FirstName = "Administrador",
                LastName = "Local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Is3C..00");
                await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
            }
            
        }
    }
}
