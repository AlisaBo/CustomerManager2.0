using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManager2.Data;
using CustomerManager2.Helpers;
using CustomerManager2.Models;
using CustomerManager2.Models.CustomerModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManager2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPageController : Controller
    {
        private IServiceProvider _serviceProvider;
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public AdminPageController(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }


        public IActionResult Index()
        {
            return View(_context.CustomerInformations.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomerInformation customerInformation)
        {
            if (ModelState.IsValid)
            {
                UserActionsHelper.CreateUser(_serviceProvider, _context, customerInformation.Name, customerInformation.Email, customerInformation.Password, "Customer");
                customerInformation.OldName = customerInformation.Name;
                _context.CustomerInformations.Add(customerInformation);

                _context.SaveChanges();                              
                return RedirectToAction("Index");
            }

            return View(customerInformation);
        }

        public IActionResult Edit(int? id)
        {
            CustomerInformation customerInformation = _context.CustomerInformations.Find(id);
            customerInformation.OldName = customerInformation.Name;

            return View(customerInformation);
        }

        // POST: CustomerInformations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CustomerInformation customerInformation)
        {
            if (ModelState.IsValid)
            {
                customerInformation.OldName = _context.CustomerInformations.Where(u => u.Id == customerInformation.Id).Select(v =>v.Name).FirstOrDefault();
                UserActionsHelper.ChangeUserData(_serviceProvider,_context, customerInformation.OldName, customerInformation.Name, customerInformation.Email, customerInformation.Password);
                customerInformation.OldName = customerInformation.Name;

                _context.Entry(customerInformation).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(customerInformation);
        }

        // GET: CustomerInformations/Delete/5
        public IActionResult Delete(int? id)
        {            
            CustomerInformation customerInformation = _context.CustomerInformations.Find(id);
            return View(customerInformation);
        }

        // POST: CustomerInformations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomerInformation customerInformation = _context.CustomerInformations.Include(c => c.LoginUsers).
                Include(c => c.ContactsDetails).Include(c => c.Departaments).FirstOrDefault(c => c.Id == id);

            _context.CustomerInformations.Remove(customerInformation);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
