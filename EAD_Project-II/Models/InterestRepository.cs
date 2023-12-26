using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace EAD_Project_II.Models
{
    public class InterestRepository : IInterestRepository
    {
        private readonly string _connectionString;
        public InterestRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<string> GetAllInterests()
        {
            List<string> interests = new();
            using (SqlConnection connection = new(_connectionString))
            {
                string query = "SELECT DISTINCT Interest FROM UserInfo"; // Adjust for your table and column names
                using (SqlCommand command = new(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object? interest = reader["Interest"];
                            if (interest != null && interest != DBNull.Value)
                            {
                                interests.Add(interest.ToString()!);
                            }
                        }

                    }
                }
            }
            return interests;
        }

        public bool DoesInterestExist(string interest)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM UserInfo WHERE Interest = @interest"; // Adjust for your table and column names
                using (SqlCommand command = new(query, connection))
                {
                    command.Parameters.AddWithValue("@interest", interest);
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public void AddInterest(string interest)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                string query = "INSERT INTO UserInfo (Interest) VALUES (@interest)"; // Adjust for your table and column names
                using (SqlCommand command = new(query, connection))
                {
                    command.Parameters.AddWithValue("@interest", interest);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<string> GetTop5Interests()
        {
            return GetInterestList("SELECT Interest FROM (SELECT Interest, ROW_NUMBER() OVER(ORDER BY COUNT(*) DESC) AS InterestCount FROM UserInfo GROUP BY Interest) AS TopInterests WHERE InterestCount <= 5");
        }

        public List<string> GetBottom5Interests()
        {
            return GetInterestList("SELECT Interest FROM (SELECT Interest, ROW_NUMBER() OVER(ORDER BY COUNT(*) ASC) AS InterestCount FROM UserInfo GROUP BY Interest) AS BottomInterests WHERE InterestCount <= 5");
        }

        public int GetCountDistinctInterests()
        {
            using (SqlConnection connection = new(_connectionString))
            {
                string query = "SELECT COUNT(DISTINCT Interest) FROM UserInfo";
                using (SqlCommand command = new(query, connection))
                {
                    connection.Open();
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private List<string> GetInterestList(string query)
        {
            List<string> interests = new();
            using (SqlConnection connection = new(_connectionString))
            {
                using (SqlCommand command = new(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object? interest = reader["Interest"];
                            if (interest != null && interest != DBNull.Value)
                            {
                                interests.Add(interest.ToString()!);
                            }
                        }
                    }
                }
            }
            return interests;
        }
    }
}
