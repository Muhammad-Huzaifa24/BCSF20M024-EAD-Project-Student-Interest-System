using EAD_Project_II.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace EAD_Project_II.Services
{
    public class ActivityLoggerService : IActivityLoggerService
    {
        private readonly string _connectionString;

        public ActivityLoggerService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void LogActivity(ActivityLogModel activityLogModel)
        {
            string sqlQuery = "INSERT INTO ActivityLogTable (Id, ActivityType, Timestamp) VALUES (@UserId, @Action, @Timestamp)";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@UserId", activityLogModel.UserId);
                    command.Parameters.AddWithValue("@Action", activityLogModel.ActivityType);
                    command.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, or handle it accordingly)
                Console.WriteLine($"Error logging activity: {ex.Message}");
                throw; // Re-throwing the exception for further handling
            }
        }

        public Dictionary<DateTime, int> GetLast30DaysActivity(int userId)
        {
            string sqlQuery = "SELECT Timestamp, COUNT(*) AS Count " +
                              "FROM ActivityLogTable " +
                              "WHERE UserId = @UserId AND Timestamp >= DATEADD(day, -30, GETDATE()) " +
                              "GROUP BY Timestamp";

            var last30DaysActivity = new Dictionary<DateTime, int>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@UserId", userId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DateTime timestamp = reader.GetDateTime(0);
                        int count = reader.GetInt32(1);
                        last30DaysActivity.Add(timestamp, count);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, throw it, or handle it accordingly)
                Console.WriteLine($"Error fetching last 30 days activity: {ex.Message}");
                throw; // Re-throwing the exception for further handling
            }

            return last30DaysActivity;
        }
    }
}
