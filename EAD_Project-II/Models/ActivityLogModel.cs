namespace EAD_Project_II.Models
{
    public class ActivityLogModel
    {
        public int UserId { get; set; }
        public string? ActivityType { get; set; }
        public DateTime Timestamp { get; set; }

        private static readonly List<ActivityLogModel> UserActivities = new();

        public static void AddActivity(ActivityLogModel activityLogModel)
        {
            UserActivities.Add(activityLogModel);
        }

        public static List<ActivityLogModel> GetUserActivities()
        {
            return UserActivities;
        }

    }
}
