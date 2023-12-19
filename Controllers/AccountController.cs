using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TaskAuthenticationAuthorization.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TaskAuthenticationAuthorization.Controllers
{
    public class AccountController : Controller
    {

        private readonly ShoppingContext _shoppingContext;

        public AccountController(ShoppingContext shoppingContext)
        {
            _shoppingContext = shoppingContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        private async Task Authenticate(Customer customer)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, customer.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, customer.Role?.Name)
                };

            // create ClaimsIdentity object
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // set auth cookies
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Customer customer = await _shoppingContext.Customers.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (customer == null)
                {
                    if (_shoppingContext.Roles.FirstOrDefaultAsync(r => r.Name == "user") == null)
                    {
                        _shoppingContext.Roles.Add(new Role { Name = "user" });
                        await _shoppingContext.SaveChangesAsync();
                    }

                    Role userRole = await _shoppingContext.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    customer = new Customer { Email = model.Email, Password = model.Password, Role = userRole };
                   
                    _shoppingContext.Customers.Add(customer);
                    await _shoppingContext.SaveChangesAsync();

                    Customer customerToAuthorize = await _shoppingContext.Customers.FirstOrDefaultAsync(u => u.Email == model.Email);

                    await Authenticate(customerToAuthorize);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Incorrect login and(or) password");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Customer customer = await _shoppingContext.Customers.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (customer != null)
                {
                    await Authenticate(customer);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Incorrect login and(or) password");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
