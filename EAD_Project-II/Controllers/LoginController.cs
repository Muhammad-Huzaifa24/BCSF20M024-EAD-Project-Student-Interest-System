using EAD_Project_II.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

using Newtonsoft.Json;
using System.Security.Claims;
using Firebase.Auth;
using EAD_Project_II.Services;
using System.Reflection;

namespace EAD_Project_II.Controllers
{
    public class LoginController : Controller
    {
        public readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActivityLoggerService _activityLoggerService;
        FirebaseAuthProvider auth;
        public LoginController(IHttpContextAccessor httpContextAccessor, IActivityLoggerService activityLoggerService)
        {
            _httpContextAccessor = httpContextAccessor;
            auth = new FirebaseAuthProvider(
                           new FirebaseConfig("AIzaSyCk_mvF-PqphAoN8pbMy7Vo3CjS2UoMrAk\r\n"));
            _activityLoggerService = activityLoggerService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                HttpContext.Session.SetString("IsAdmin", "false");
                HttpContext.Session.SetString("IsStudent", "true");
                return RedirectToAction("Dashboard", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, bool rememberMe = false)
        {
            try
            {
                var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                string token = fbAuthLink.FirebaseToken;

                if (!string.IsNullOrEmpty(token))
                {
                   

                    // Customize as needed: Retrieve user data from Firebase
                    string userName = "YourUserNameFromFirebase";

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userName),
                        // Add other claims if needed based on Firebase user data
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = rememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        authProperties);

                    HttpContext.Session.SetString("IsAdmin","false");
                    HttpContext.Session.SetString("IsStudent","true");

                    return RedirectToAction("Dashboard", "Dashboard");
                }

                ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
                return View(model);
            }
            catch (FirebaseAuthException ex)
            {
                // Handle Firebase authentication error
                ModelState.AddModelError(string.Empty, ex.Message); // Add error message to ModelState
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
        public async Task<IActionResult> Logout()
        {
            var activityLog = new ActivityLogModel
            {
                ActivityType = "Logout", 
                Timestamp = DateTime.Now
            };

            // Add activity to the local list
            ActivityLogModel.AddActivity(activityLog);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("StartView", "Admin");
        }

    }

}
