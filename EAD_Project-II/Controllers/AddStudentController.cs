using Microsoft.AspNetCore.Mvc;
using EAD_Project_II.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;

namespace EAD_Project_II.Controllers
{
    public class AddStudentController : Controller
    {
        private readonly IFirebaseClient _client;      
        private readonly IInterestRepository _interestRepository;
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProjectDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddStudentController(IInterestRepository interestRepository, IHttpContextAccessor httpContextAccessor)
        {
            _interestRepository = interestRepository;
            _httpContextAccessor = httpContextAccessor;
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "JTN1jk3m1muTO7TeU4tzeDXLJzWGbshzDwAoCg2B",
                BasePath = "https://ead-project-ii-default-rtdb.asia-southeast1.firebasedatabase.app/"
            };
            _client = new FireSharp.FirebaseClient(config);
        }

        [HttpGet]
        public IActionResult AddStudent()
        {

            var viewModel = new StudentViewModel();

            // Fetch existing interests from the repository
            var existingInterests = _interestRepository.GetAllInterests();
            viewModel.ExistingInterests = existingInterests;

            var userName = _httpContextAccessor.HttpContext?.Session.GetString("UserName");
            ViewBag.UserName = userName;

            string isAdminString = HttpContext.Session.GetString("IsAdmin") ?? "false";
            string isStudentString = HttpContext.Session.GetString("IsStudent") ?? "false";

            bool isAdmin = bool.Parse(isAdminString);
            bool isStudent = bool.Parse(isStudentString);

            ViewData["IsAdmin"] = isAdmin;
            ViewData["IsStudent"] = isStudent;

            return View(viewModel);
        }



        [HttpPost]
        public IActionResult AddStudent(StudentViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = _connectionString; // Using the initialized connection string

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string insertQuery = $"INSERT INTO UserInfo (FullName, RollNumber, EmailAddress, Gender, Interest, Dob, Subject, City, Dept, Degree, StartDate, EndDate) VALUES (@fullName, @rollNumber, @emailAddress, @gender, @interest, @dob, @subject, @city, @dept, @degree, @startDate, @endDate)";

                        using (SqlCommand command = new SqlCommand(insertQuery, connection))
                        {
                            object createdDateValue = DateTime.Now; // Current timestamp for CreatedDate
                            command.Parameters.AddWithValue("@createdDate", createdDateValue);
                            
                            command.Parameters.AddWithValue("@fullName", userViewModel.FullName);
                            command.Parameters.AddWithValue("@rollNumber", userViewModel.RollNumber);
                            command.Parameters.AddWithValue("@emailAddress", userViewModel.EmailAddress);
                            command.Parameters.AddWithValue("@gender", userViewModel.Gender);

                            // Logic to handle the interest field
                            string selectedInterest = userViewModel.SelectedInterest!;
                            string newInterest = userViewModel.Interest!;
                            command.Parameters.AddWithValue("@interest", !string.IsNullOrEmpty(selectedInterest) ? selectedInterest : newInterest);

                            command.Parameters.AddWithValue("@dob", userViewModel.Dob);
                            command.Parameters.AddWithValue("@subject", userViewModel.Subject);
                            command.Parameters.AddWithValue("@city", userViewModel.City);
                            command.Parameters.AddWithValue("@dept", userViewModel.Dept);
                            command.Parameters.AddWithValue("@degree", userViewModel.Degree);
                            object startDateValue = (object)userViewModel.StartDate! ?? DBNull.Value;
                            command.Parameters.AddWithValue("@startDate", startDateValue);
                            object endDateValue = (object)userViewModel.EndDate! ?? DBNull.Value;
                            command.Parameters.AddWithValue("@endDate", endDateValue);

                            // Execute SQL command
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                string isAdminString = HttpContext.Session.GetString("IsAdmin") ?? "false";
                                string isStudentString = HttpContext.Session.GetString("IsStudent") ?? "false";

                                bool isAdmin = bool.Parse(isAdminString);
                                bool isStudent = bool.Parse(isStudentString);

                                ViewData["IsAdmin"] = isAdmin;
                                ViewData["IsStudent"] = isStudent;
                                TempData["SuccessMessage"] = "Data has been saved.";
                                return RedirectToAction("Students", "StudentList");
                            }
                            else
                            {
                                return View(userViewModel);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("", "An error occurred while processing your request.");
                    return View(userViewModel);
                }
            }
            else
            {
                // Model state is not valid, return to the same view with validation errors
                var existingInterests = _interestRepository.GetAllInterests();
                userViewModel.ExistingInterests = existingInterests;
                return View(userViewModel);
            }
        }

        /*
                [HttpPost]
       
                public async Task<IActionResult> AddStudent(StudentViewModel userViewModel)
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            var data = new
                            {
                                FullName = userViewModel.FullName,
                                RollNumber = userViewModel.RollNumber,
                                EmailAddress = userViewModel.EmailAddress,
                                Gender = userViewModel.Gender,
                                Interest = !string.IsNullOrEmpty(userViewModel.SelectedInterest)
                                    ? userViewModel.SelectedInterest
                                    : userViewModel.Interest,
                                Dob = userViewModel.Dob,
                                Subject = userViewModel.Subject,
                                City = userViewModel.City,
                                Dept = userViewModel.Dept,
                                Degree = userViewModel.Degree,
                                StartDate = userViewModel.StartDate,
                                EndDate = userViewModel.EndDate
                            };

                            FirebaseResponse response = await _client.PushAsync("Students", data);
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                TempData["SuccessMessage"] = "Data has been saved to Firebase.";
                                return RedirectToAction("Students", "StudentList");
                            }
                            else
                            {
                                Console.WriteLine(response.Body);
                                return View(userViewModel);
                            }

                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions
                            Console.WriteLine(ex.Message);
                            ModelState.AddModelError("", "An error occurred while processing your request.");
                            return View(userViewModel);
                        }
                    }
                    else
                    {
                        // Model state is not valid, return to the same view with validation errors
                        var existingInterests = _interestRepository.GetAllInterests();
                        userViewModel.ExistingInterests = existingInterests;
                        return View(userViewModel);
                    }
                }
        */  
    }
}
