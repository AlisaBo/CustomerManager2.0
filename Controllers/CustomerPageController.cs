using System;
using System.Linq;
using CustomerManager2.Data;
using CustomerManager2.Helpers;
using CustomerManager2.Models;
using CustomerManager2.Models.CustomerModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManager2.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerPageController : Controller
    {
        private IServiceProvider _serviceProvider;
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public CustomerPageController(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        public IActionResult Index()
        {
            return View();
        }

        #region CONTACT
        public IActionResult CreateContact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateContact(ContactsDetail contactsDetail)
        {
            if (contactsDetail == null)
            {
                throw new ArgumentNullException(nameof(contactsDetail));
            }

            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                contactsDetail.CustomerInformation = _context.CustomerInformations.Where(c => c.Name == username).FirstOrDefault();

                _context.ContactsDetails.Add(contactsDetail);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contactsDetail);
        }

        public IActionResult EditContact(int? id)
        {
            var contactsDetail = _context.ContactsDetails.Find(id);
            return View(contactsDetail);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditContact(ContactsDetail contactsDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(contactsDetail).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactsDetail);
        }

        public IActionResult DeleteContact(int? id)
        {
            ContactsDetail contactsDetail = _context.ContactsDetails.Find(id);
            return View(contactsDetail);
        }

        [HttpPost, ActionName("DeleteContact")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteContactConfirmed(int id)
        {
            var contactsDetail = _context.ContactsDetails.Find(id);
            _context.ContactsDetails.Remove(contactsDetail);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region DEPARTAMENT
        public IActionResult CreateDepartament()
        {
            var departament = new Departament();

            var username = User.Identity.Name;

            departament.Customer = (from customer in _context.CustomerInformations
                                    where customer.Name == username
                                    select customer).FirstOrDefault();


            var listOfUsers = (from loginUser in _context.LoginUsers
                               where loginUser.Customer.Name == username
                               select loginUser).ToList().Select(u => new SelectListItem
                               {
                                   Text = u.Name,
                                   Value = u.Name
                               });

            ViewBag.ListOfUsers = listOfUsers;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDepartament(Departament departament)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;

                departament.Customer = (from customer in _context.CustomerInformations
                                        where customer.Name == username
                                        select customer).FirstOrDefault();

                departament.Manager = (from users in _context.LoginUsers
                                       where users.Name == departament.ManagerName
                                       select users).FirstOrDefault();

                _context.Departaments.Add(departament);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(departament);
        }

        public IActionResult EditDepartament(int? id)
        {
            Departament departament = _context.Departaments.Find(id);

            var username = User.Identity.Name;

            departament.Customer = (from customer in _context.CustomerInformations
                                    where customer.Name == username
                                    select customer).FirstOrDefault();


            var listOfUsers = (from users in _context.LoginUsers
                               where users.Customer.Name == username
                               select users).ToList().Select(u => new SelectListItem
                               {
                                   Text = u.Name,
                                   Value = u.Name
                               });

            ViewBag.ListOfUsers = listOfUsers;

            return View(departament);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDepartament(Departament departament)
        {
            if (ModelState.IsValid)
            {
                departament.Manager = (from users in _context.LoginUsers
                                       where users.Name == departament.ManagerName
                                       select users).FirstOrDefault();
                try
                {
                    //I admit that it's not legal
                    var sqlCommand = String.Format("update dbo.Departaments set ManagerId = {0} where Id = {1}", departament.Manager.Id, departament.Id);
                    _context.Database.ExecuteSqlCommand(sqlCommand);
                    _context.Entry(departament).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("FullName", "Query error");
                    return View(departament);
                }
            }

            return View(departament);
        }
        public IActionResult DeleteDepartament(int? id)
        {
            var departament = _context.Departaments.Find(id);
            return View(departament);
        }

        [HttpPost, ActionName("DeleteDepartament")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDepartamentConfirmed(int id)
        {
            var departament = _context.Departaments.Find(id);
            _context.Departaments.Remove(departament);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region USERS
        public IActionResult CreateUser()
        {
            var loginUser = new LoginUser();

            var username = User.Identity.Name;

            loginUser.Customer = (from customer in _context.CustomerInformations
                                  where customer.Name == username
                                  select customer).FirstOrDefault();


            var listOfDepartamets = (from departament in _context.Departaments
                                     where departament.Customer.Name == username
                                     select departament).ToList().Select(u => new SelectListItem
                                     {
                                         Text = u.Name,
                                         Value = u.Name
                                     });

            ViewBag.ListOfDepartaments = listOfDepartamets;
            return View(loginUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(LoginUser loginUser)
        {
            var username = User.Identity.Name;

            if (ModelState.IsValid)
            {
                try
                {

                    UserActionsHelper.CreateUser(_serviceProvider, _context, loginUser.UserName, loginUser.Mail, loginUser.Password, "User");
                    loginUser.OldName = loginUser.UserName;

                    loginUser.Customer = (from customer in _context.CustomerInformations
                                          where customer.Name == username
                                          select customer).FirstOrDefault();

                    loginUser.Departament = (from departament in _context.Departaments
                                             where departament.Name == loginUser.DepartamentName
                                             select departament).FirstOrDefault();

                    loginUser.Id = _context.LoginUsers.LastOrDefault() != null ? _context.LoginUsers.AsNoTracking().LastOrDefault().Id + 1 : 1;

                    _context.LoginUsers.Add(loginUser);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("FullName", "Query error");
                    return View(loginUser);
                }
            }

            var listOfDepartamets = (from departament in _context.Departaments
                                     where departament.Customer.Name == username
                                     select departament).ToList().Select(u => new SelectListItem
                                     {
                                         Text = u.Name,
                                         Value = u.Id.ToString()
                                     });

            ViewBag.ListOfDepartaments = listOfDepartamets;

            return View(loginUser);
        }

        public IActionResult EditUser(int? id)
        {
            LoginUser loginUser = _context.LoginUsers.Find(id);
            loginUser.OldName = loginUser.UserName;

            var username = User.Identity.Name;

            loginUser.Customer = (from customer in _context.CustomerInformations
                                  where customer.Name == username
                                  select customer).FirstOrDefault();


            var listOfDepartamets = (from departament in _context.Departaments
                                     where departament.Customer.Name == username
                                     select departament).ToList().Select(u => new SelectListItem
                                     {
                                         Text = u.Name,
                                         Value = u.Name
                                     });

            ViewBag.ListOfDepartaments = listOfDepartamets;

            return View(loginUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult EditUser(LoginUser loginUser)
        {
            var username = User.Identity.Name;    
            if (ModelState.IsValid)
            {
                loginUser.OldName = _context.LoginUsers.Where(u => u.Id == loginUser.Id).Select(v => v.UserName).FirstOrDefault();
                UserActionsHelper.ChangeUserData(_serviceProvider, _context, loginUser.OldName, loginUser.UserName, loginUser.Mail, loginUser.Password);
                loginUser.OldName = loginUser.UserName;

                loginUser.Departament = _context.Departaments.Where(d => d.Name == loginUser.DepartamentName).FirstOrDefault();

                try
                {
                    //I admit that it's not legal
                    var sqlCommand = String.Format("update dbo.LoginUsers set DepartamentId = {0} where Id = {1}", loginUser.Departament.Id, loginUser.Id);
                    _context.Database.ExecuteSqlCommand(sqlCommand);
                    _context.Entry(loginUser).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("FullName", "Query error");
                    return View(loginUser);
                }
            }
            var listOfDepartamets = (from departament in _context.Departaments
                                     where departament.Customer.Name == username
                                     select departament).ToList().Select(u => new SelectListItem
                                     {
                                         Text = u.Name,
                                         Value = u.Id.ToString()
                                     });

            ViewBag.ListOfDepartaments = listOfDepartamets;
            return View(loginUser);
        }

        public IActionResult DeleteUser(int? id)
        {
            var loginUser = _context.LoginUsers.Where(u => u.Id == id).Include(u => u.Departament).FirstOrDefault();
            return View(loginUser);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUserConfirmed(int id)
        {
            //var loginUser = _context.LoginUsers.Include(c => c.Departament).FirstOrDefault(c => c.Id == id);
            var loginUser = _context.LoginUsers.FirstOrDefault(c => c.Id == id);
            _context.LoginUsers.Remove(loginUser);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion
    }
}