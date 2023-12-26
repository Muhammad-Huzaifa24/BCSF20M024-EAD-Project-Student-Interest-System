namespace EAD_Project_II.Models
{
    public interface IActivityLoggerService
    {
        void LogActivity(ActivityLogModel activity);
        public Dictionary<DateTime, int> GetLast30DaysActivity(int userId);
    }
}
