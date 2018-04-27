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
    public class ContactsDetailViewComponent: ViewComponent
    {
        private IServiceProvider _serviceProvider;
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public ContactsDetailViewComponent(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        public IViewComponentResult Invoke()
        {
            var username = User.Identity.Name;
            var contactsToCustomer = _context.ContactsDetails.Where(c => c.CustomerInformation.Name == username).Include(c => c.CustomerInformation).ToList();
            return View(contactsToCustomer);
        }
    }
}
