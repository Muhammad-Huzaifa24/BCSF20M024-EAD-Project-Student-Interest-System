using System.Collections.Generic;

namespace EAD_Project_II.Models
{
    public interface IInterestRepository
    {
        List<string> GetAllInterests();
        bool DoesInterestExist(string interest);
        void AddInterest(string interest);
        List<string> GetTop5Interests();
        List<string> GetBottom5Interests();
        int GetCountDistinctInterests();
    }
}
