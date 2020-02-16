using ProcastinationKiller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services.Abstract
{
    public interface IRewardService
    {
        Task AssignBaseRewards(string userId);
        Task<IEnumerable<Badge>> GetRewards(string userId);
        Task Calculate(string userId, int dailyLogins, int points, int totalTodosCompleted);
    }
}
