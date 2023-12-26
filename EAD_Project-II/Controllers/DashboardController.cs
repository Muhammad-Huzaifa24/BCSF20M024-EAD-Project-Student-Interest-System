using Microsoft.AspNetCore.Mvc;
using EAD_Project_II.Models;

using Microsoft.Data.SqlClient;

namespace EAD_Project_II.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IInterestRepository _interestRepository;
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProjectDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite"; // Replace with your connection string

        public DashboardController(IInterestRepository interestRepository)
        {
            _interestRepository = interestRepository;
        }

        public IActionResult Dashboard()
        {

            // Top and bottom interests

            var top5Interests = _interestRepository.GetTop5Interests() ?? new List<string>();
            var bottom5Interests = _interestRepository.GetBottom5Interests() ?? new List<string>();
            var distinctInterestCount = _interestRepository.GetCountDistinctInterests();

            ViewData["TopInterests"] = top5Interests;
            ViewData["BottomInterests"] = bottom5Interests;
            ViewData["CountDistinctInterests"] = distinctInterestCount;

            // Gender distribution

            int maleCount = GetGenderCount("Male");
            int femaleCount = GetGenderCount("Female");

            List<DataPoint> dataPoints = new List<DataPoint>
            {
                new DataPoint("Male", maleCount),
                new DataPoint("Female", femaleCount)
            };

            ViewBag.DataPoints = Newtonsoft.Json.JsonConvert.SerializeObject(dataPoints);

           // Department distribution
            Dictionary<string, int> departmentCounts = GetDepartmentCounts();

           
            List<DataPoint> departmentDataPoints = new List<DataPoint>();
            foreach (var departmentCount in departmentCounts)
            {
                departmentDataPoints.Add(new DataPoint(departmentCount.Key, departmentCount.Value));
            }

            ViewBag.DepartmentDataPoints = Newtonsoft.Json.JsonConvert.SerializeObject(departmentDataPoints);

            // Degree Distribution
            Dictionary<string, int> degreeCounts = GetDegreeCounts();

            List<DataPoint> degreeDataPoints = new List<DataPoint>();
            foreach (var degreeCount in degreeCounts)
            {
                degreeDataPoints.Add(new DataPoint(degreeCount.Key, degreeCount.Value));
            }
            ViewBag.DegreeDataPoints = Newtonsoft.Json.JsonConvert.SerializeObject(degreeDataPoints);

            //Provincial Distribution
            Dictionary<string, int> cityCounts = GetCityCounts();

            List<DataPoint> cityDataPoints = new();
            foreach (var cityCount in cityCounts)
            {
                cityDataPoints.Add(new DataPoint(cityCount.Key, cityCount.Value));
            }
            ViewBag.CityDataPoints = Newtonsoft.Json.JsonConvert.SerializeObject(cityDataPoints);

            // Students Status
            var graduatedCount = FetchStudentCount("Graduated");
            var goingToGraduateCount = FetchStudentCount("Going to graduate");
            var recentlyEnrolledCount = FetchStudentCount("Recently enrolled");
            var studyingCount = FetchStudentCount("Studying");

            ViewBag.GraduatedCount = graduatedCount;
            ViewBag.GoingToGraduateCount = goingToGraduateCount;
            ViewBag.RecentlyEnrolledCount = recentlyEnrolledCount;
            ViewBag.StudyingCount = studyingCount;

            // Age distribution
            List<int> ages = FetchAges();
            ViewBag.Ages = ages;

            // Last 30 days activity
            var studentsCreatedDailyLast30Days = GetStudentsCreatedDailyLast30Days();
            ViewBag.StudentsCreatedDailyLast30Days = studentsCreatedDailyLast30Days;


            return View();
        }
        

        // Last 30 days activity
        public List<int> GetStudentsCreatedDailyLast30Days()
        {
            List<int> studentsCreatedDaily = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) AS StudentsCount, CONVERT(DATE, CreatedDate) AS DateOnly " +
                               "FROM UserInfo " +
                               "WHERE CreatedDate >= DATEADD(DAY, -30, GETDATE()) " +
                               "GROUP BY CONVERT(DATE, CreatedDate) " +
                               "ORDER BY DateOnly";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int studentsCount = Convert.ToInt32(reader["StudentsCount"]);
                    studentsCreatedDaily.Add(studentsCount);
                }

                reader.Close();
            }

            return studentsCreatedDaily;
        }

        private List<int> FetchAges()
        {
            List<int> ages = new();
            using (SqlConnection connection = new(connectionString))
            {
                string query = "SELECT Dob FROM UserInfo";
                SqlCommand command = new(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DateTime dob = Convert.ToDateTime(reader["Dob"]);
                    int age = CalculateAge(dob);
                    ages.Add(age);
                }
                reader.Close();
            }
            return ages;
        }
        private int CalculateAge(DateTime dob)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - dob.Year;
            if (dob > now.AddYears(-age))
            {
                age--;
            }
            return age;
        }
        private int FetchStudentCount(string category)
        {
            int count = 0;
            using (SqlConnection connection = new(connectionString))
            {
                string query = "";

                switch (category)
                {
                    case "Graduated":
                        query = "SELECT COUNT(*) FROM UserInfo WHERE EndDate < GETDATE()";
                        break;

                    case "GoingToGraduate":
                        query = "SELECT COUNT(*) FROM UserInfo WHERE StartDate > GETDATE()";
                        break;

                    case "RecentlyEnrolled":
                        query = "SELECT COUNT(*) FROM UserInfo WHERE StartDate > DATEADD(MONTH, -1, GETDATE())";
                        break;

                    case "Studying":
                        query = "SELECT COUNT(*) FROM UserInfo WHERE (EndDate IS NULL OR EndDate >= GETDATE()) AND (StartDate IS NULL OR StartDate <= GETDATE())";
                        break;

                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(query))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    count = (int)command.ExecuteScalar();
                }
            }
            return count;
        }
        private Dictionary<string, int> GetCityCounts()
        {
            Dictionary<string, int> cityCounts = new Dictionary<string, int>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT City, COUNT(*) AS CityCount FROM UserInfo GROUP BY City";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string city = reader["City"].ToString()!;
                    int count = Convert.ToInt32(reader["CityCount"]);
                    cityCounts.Add(city!, count);
                }
                reader.Close();
            }
            return cityCounts;
        }
        private Dictionary<string, int> GetDegreeCounts()
        {
            Dictionary<string, int> degreeCounts = new Dictionary<string, int>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Degree, COUNT(*) AS DegreeCount FROM UserInfo GROUP BY Degree";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string degree = reader["Degree"].ToString()!;
                    int count = Convert.ToInt32(reader["DegreeCount"]);
                    degreeCounts.Add(degree!, count);
                }
                reader.Close();
            }
            return degreeCounts;
        }
        private int GetGenderCount(string gender)
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"SELECT COUNT(*) FROM UserInfo WHERE Gender = @gender";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@gender", gender);
                connection.Open();
                count = (int)command.ExecuteScalar();
            }
            return count;
        }
        private Dictionary<string, int> GetDepartmentCounts()
        {
            Dictionary<string, int> departmentCounts = new Dictionary<string, int>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Dept, COUNT(*) AS DeptCount FROM UserInfo GROUP BY Dept";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string department = reader["Dept"].ToString()!;
                    int count = Convert.ToInt32(reader["DeptCount"]);
                    departmentCounts.Add(department!, count);
                }
                reader.Close();
            }
            return departmentCounts;
        }

    }
}
