using System;
using System.Linq;
using CustomerManager2.Data;
using CustomerManager2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManager2.Controllers
{
    [Authorize(Roles = "User")]
    public class UserPageController : Controller
    {
        private IServiceProvider _serviceProvider;
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public UserPageController(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        public IActionResult Index()
        {
            var username = User.Identity.Name;

            var loginDetails = _context.LoginUsers.Where(x => x.UserName == username).Include(y => y.Customer).FirstOrDefault();

            var customerToUserData = (from customer in _context.CustomerInformations
                                      where customer.Name == loginDetails.Customer.Name
                                      select customer).FirstOrDefault();

            return View(customerToUserData);
        }
    }
}