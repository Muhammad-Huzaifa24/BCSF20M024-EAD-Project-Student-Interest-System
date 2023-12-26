using EAD_Project_II.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Reflection;


namespace EAD_Project_II.Controllers
{
    public class SignUpController : Controller
    {
      //  private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProjectDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite"; // Replace with your connection string
        FirebaseAuthProvider auth;
        private readonly IActivityLoggerService _activityLoggerService; // Inject the ActivityLoggerService

        public SignUpController(IActivityLoggerService activityLoggerService)
        {
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyCk_mvF-PqphAoN8pbMy7Vo3CjS2UoMrAk\r\n"));
            _activityLoggerService = activityLoggerService;
        }
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }      

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    /*
                    //using (SqlConnection connection = new SqlConnection(_connectionString))
                    //{
                    //    connection.Open();

                    //    string insertQuery = "INSERT INTO Users (Name, Email, Password, ConfirmPassword) " +
                    //                         "VALUES (@Name, @Email, @Password, @ConfirmPassword)";

                    //    using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                    //    {
                    //        cmd.Parameters.AddWithValue("@Name", signUpModel.Name);
                    //        cmd.Parameters.AddWithValue("@Email", signUpModel.Email);
                    //        cmd.Parameters.AddWithValue("@Password", signUpModel.Password);
                    //        cmd.Parameters.AddWithValue("@ConfirmPassword", signUpModel.ConfirmPassword);

                    //        int rowsAffected = cmd.ExecuteNonQuery();

                    //        if (rowsAffected > 0)
                    //        {
                    //            return RedirectToAction("Login", "Login");
                    //        }
                    //        Handle insertion failure if required
                    //    }
                    //}

                    */


                    //create the user
                    await auth.CreateUserWithEmailAndPasswordAsync(signUpModel.Email, signUpModel.Password);
                    //log in the new user
                    var fbAuthLink = await auth
                                    .SignInWithEmailAndPasswordAsync(signUpModel.Email, signUpModel.Password);
                    string token = fbAuthLink.FirebaseToken;
                    if (token != null)
                    {
                       


                        HttpContext.Session.SetString("_UserToken", token);

                        return RedirectToAction("Login", "Login");
                    }

                }
            }
            catch (FirebaseAuthException ex)
            {
                // Handle Firebase authentication error
                ModelState.AddModelError(string.Empty, ex.Message); // Add error message to ModelState
                return View(signUpModel);
            }

            // If user creation fails or encounters an error, stay on the SignUp view
            return View(signUpModel);
        }
        

    }
}
