
using EAD_Project_II.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace EAD_Project_II.Controllers
{
    public class StudentListController : Controller
    {
        string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProjectDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"; // Replace with your SQL Server connection string
        private readonly IActivityLoggerService _activityLoggerService; // Inject the ActivityLoggerService

        public StudentListController(IActivityLoggerService activityLoggerService)
        {
            _activityLoggerService = activityLoggerService;
        }
        // fetch all students from db and 
        // display into the list       
        public IActionResult Students()
        {
            string isAdminString = HttpContext.Session.GetString("IsAdmin") ?? "false";
            string isStudentString = HttpContext.Session.GetString("IsStudent") ?? "false";

            bool isAdmin = bool.Parse(isAdminString);
            bool isStudent = bool.Parse(isStudentString);

            ViewData["IsAdmin"] = isAdmin;
            ViewData["IsStudent"] = isStudent;



            List<StudentViewModel> user = new();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Id, FullName, RollNumber, Dept, Degree, Dob, City, Interest FROM UserInfo"; // Your SQL query
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Map data to Student object
                                StudentViewModel student = new StudentViewModel
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    FullName = reader["FullName"].ToString(),
                                    RollNumber = reader["RollNumber"].ToString(),
                                    Dept = reader["Dept"].ToString(),
                                    Degree = reader["Degree"].ToString(),
                                    Dob = Convert.ToDateTime(reader["Dob"]),
                                    City = reader["City"].ToString(),
                                    Interest = reader["Interest"].ToString()
                                };
                                user.Add(student);
                               


                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log error)
                Console.WriteLine(ex.Message);
                // You might want to add ModelState error or handle the exception in a way suitable for your application
            }

            return View(user);
        }

        // view selected / current student      
        public IActionResult StudentView(int id)
        {
            StudentViewModel? student = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Id, FullName, RollNumber, Dept, Degree, Dob, City, Interest FROM UserInfo WHERE Id = @Id"; // SQL query with a parameter
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id); // Set the parameter value

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Map data to UserViewModel object for the specific student
                                student = new StudentViewModel
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    FullName = reader["FullName"].ToString(),
                                    RollNumber = reader["RollNumber"].ToString(),
                                    Dept = reader["Dept"].ToString(),
                                    Degree = reader["Degree"].ToString(),
                                    Dob = Convert.ToDateTime(reader["Dob"]),
                                    City = reader["City"].ToString(),
                                    Interest = reader["Interest"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log error)
                Console.WriteLine(ex.Message);
            }
            return View(student);
        }

        [HttpGet]      
        public IActionResult EditStudent(int id)
        {
            StudentViewModel student = GetStudentById(id); // Fetch the student details by ID from the database

            if (student == null)
            {
                return RedirectToAction("Error"); // Handle the case where the student is not found
            }

            return View(student); // Pass the student data to the view for editing
        }
       
        [HttpPost]        
        public IActionResult EditStudent(StudentViewModel updatedStudent)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                                UPDATE UserInfo 
                                SET FullName = @FullName, RollNumber = @RollNumber, EmailAddress = @EmailAddress,
                                    Gender = @Gender, Interest = @Interest, Dob = @Dob, Subject = @Subject, 
                                    City = @City, Dept = @Dept, Degree = @Degree, StartDate = @StartDate, EndDate = @EndDate
                                WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Set parameters for the update query
                        command.Parameters.AddWithValue("@Id", updatedStudent.Id);
                        command.Parameters.AddWithValue("@FullName", updatedStudent.FullName);
                        command.Parameters.AddWithValue("@RollNumber", updatedStudent.RollNumber);
                        command.Parameters.AddWithValue("@EmailAddress", updatedStudent.EmailAddress);
                        command.Parameters.AddWithValue("@Gender", updatedStudent.Gender);
                        command.Parameters.AddWithValue("@Interest", updatedStudent.Interest);
                        command.Parameters.AddWithValue("@Dob", updatedStudent.Dob); // Ensure Dob is added here
                        command.Parameters.AddWithValue("@Subject", updatedStudent.Subject);
                        command.Parameters.AddWithValue("@City", updatedStudent.City);
                        command.Parameters.AddWithValue("@Dept", updatedStudent.Dept);
                        command.Parameters.AddWithValue("@Degree", updatedStudent.Degree);
                        command.Parameters.AddWithValue("@StartDate", updatedStudent.StartDate);
                        command.Parameters.AddWithValue("@EndDate", updatedStudent.EndDate);

                        // Execute the update query
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Update successful, perform any additional actions or redirect as needed

                            // For example, redirect to the student list after update

                            string isAdminString = HttpContext.Session.GetString("IsAdmin") ?? "false";
                            string isStudentString = HttpContext.Session.GetString("IsStudent") ?? "false";

                            bool isAdmin = bool.Parse(isAdminString);
                            bool isStudent = bool.Parse(isStudentString);

                            ViewData["IsAdmin"] = isAdmin;
                            ViewData["IsStudent"] = isStudent;
                            return RedirectToAction("Students");

                        }
                        else
                        {
                            // Handle case where update did not occur
                            // You can redirect to an error page or perform other error handling
                            return RedirectToAction("Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log error)
                Console.WriteLine(ex.Message);
                // You might want to add ModelState error or handle the exception in a way suitable for your application
                return RedirectToAction("Error");
            }
        }
      
        // delete selected / Current student
        [HttpPost]       
        public IActionResult DeleteStudent(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "DELETE FROM UserInfo WHERE Id = @Id"; // SQL query to delete the student
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id); // Set the parameter value for student ID

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Deletion successful, perform any additional actions or redirect as needed
                            // For example, redirect to the student list after deletion
                            string isAdminString = HttpContext.Session.GetString("IsAdmin") ?? "false";
                            string isStudentString = HttpContext.Session.GetString("IsStudent") ?? "false";

                            bool isAdmin = bool.Parse(isAdminString);
                            bool isStudent = bool.Parse(isStudentString);

                            ViewData["IsAdmin"] = isAdmin;
                            ViewData["IsStudent"] = isStudent;
                            return RedirectToAction("Students");
                        }
                        else
                        {
                            // Handle case where deletion did not occur (student with provided ID not found)
                            // You can redirect to an error page or perform other error handling
                            return RedirectToAction("Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log error)
                Console.WriteLine(ex.Message);
                // You might want to add ModelState error or handle the exception in a way suitable for your application
                return RedirectToAction("Error");
            }
        }
        public StudentViewModel GetStudentById(int id)
        {
            StudentViewModel student = null!;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Id, FullName, RollNumber, EmailAddress, Gender, Interest, Dob, Subject, City, Dept, Degree, StartDate, EndDate FROM UserInfo WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                student = new StudentViewModel
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    FullName = reader["FullName"].ToString(),
                                    RollNumber = reader["RollNumber"].ToString(),
                                    EmailAddress = reader["EmailAddress"].ToString(),
                                    Gender = reader["Gender"].ToString(),
                                    Interest = reader["Interest"].ToString(),
                                    Dob = Convert.ToDateTime(reader["Dob"]),
                                    Subject = reader["Subject"].ToString(),
                                    City = reader["City"].ToString(),
                                    Dept = reader["Dept"].ToString(),
                                    Degree = reader["Degree"].ToString(),
                                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                                    EndDate = Convert.ToDateTime(reader["EndDate"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log error)
                Console.WriteLine(ex.Message);
                // You might want to add ModelState error or handle the exception in a way suitable for your application
            }

            return student;
        }


    }
}
