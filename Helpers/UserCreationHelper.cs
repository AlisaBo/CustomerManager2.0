using CustomerManager2.Data;
using CustomerManager2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CustomerManager2.Helpers
{
    public static class UserActionsHelper
    {
        public static void CreateUser(IServiceProvider serviceProvider,ApplicationDbContext context, string username, string email, string password, string userRole)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            if (!roleManager.RoleExistsAsync(userRole).Result)
            {
                var role = new IdentityRole();
                role.Name = userRole;
                roleManager.CreateAsync(role);
            }

            var user = new ApplicationUser();
            user.UserName = username;
            user.Email = email;
            var chkUser = userManager.CreateAsync(user, password);
            chkUser.Wait();

            if (chkUser.Result.Succeeded)
            {

                var newUserRole =  userManager.AddToRoleAsync(user, userRole);
                newUserRole.Wait();
            }

        }

        public static void ChangeUserData(IServiceProvider serviceProvider, ApplicationDbContext context, string username, string newUsername, string newEmail, string newPassword)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = userManager.FindByNameAsync(username).Result;

            user.UserName = newUsername;
            user.Email = newEmail;

            userManager.RemovePasswordAsync(user);
            userManager.AddPasswordAsync(user, newPassword);
            userManager.UpdateAsync(user);
        }
    }
}