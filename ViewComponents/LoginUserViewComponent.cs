using CustomerManager2.Data;
using CustomerManager2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CustomerManager2.ViewComponents
{
    public class LoginUserViewComponent : ViewComponent
    {
        private IServiceProvider _serviceProvider;
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public LoginUserViewComponent(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        public IViewComponentResult Invoke()
        {
            var username = User.Identity.Name;
            var loginUsersToCustomer = _context.LoginUsers.Where(c => c.Customer.Name == username).Include(c => c.Customer).Include(c => c.DepartamentsToManage).ToList();
            return View(loginUsersToCustomer);
        }
    }
}



