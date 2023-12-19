using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskAuthenticationAuthorization.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using System.Data;


namespace TaskAuthenticationAuthorization.Controllers
{
    public class AccountController : Controller
    {
        private ShoppingContext db;

        public AccountController(ShoppingContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                Customer user = await db.Customers.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // add user to db
                    user = new Customer { Email = model.Email, Password = model.Password, Discount = Discount.R };
                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Buyer");
                    if (userRole != null)
                        user.Role = userRole;

                    db.Customers.Add(user);
                    await db.SaveChangesAsync();

                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Incorrect login and (or) password");
            }
            return View(model);
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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }


    }
}
