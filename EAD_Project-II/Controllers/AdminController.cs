using Microsoft.AspNetCore.Mvc;
using EAD_Project_II.Models;

namespace EAD_Project_II.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult StartView()
        {
            return View("StartView");
        }

        [HttpGet]
        public IActionResult Admin()
        {
            return View(new AdminViewModel());
        }
        [HttpPost]
        public IActionResult Admin(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Name == "admin" && model.Password == "admin")
                {
                    HttpContext.Session.SetString("IsAdmin", "true");
                    HttpContext.Session.SetString("IsStudent", "false");
                    return RedirectToAction("Dashboard", "Dashboard"); // Valid credentials, redirect to admin dashboard
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid credentials");
                    ModelState.AddModelError("InvalidLogin", "Invalid username or password."); // Add error for specific field
                    return View(model); // Invalid credentials, return to the login page with an error message
                }
            }

            return View(model); // Model state is invalid, return to the login page with validation errors
        }
    }
}
