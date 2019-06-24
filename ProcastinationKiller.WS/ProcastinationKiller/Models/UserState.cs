using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class UserState
    {
        public int Id { get; set; }

        public int Points { get; set; }

        public int DailyLogins { get; set; }

        public int WeeklyLogins { get; set; }

        public int LongestLoginStreak { get; set; }

        public int CurrentLoginStreak { get; set; }

        public int TotalTodosCompleted { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public static UserState Copy(UserState otherState)
        {
            return new UserState()
            {
                DailyLogins = otherState.DailyLogins,
                LongestLoginStreak = otherState.LongestLoginStreak,
                Points = otherState.Points,
                TotalTodosCompleted = otherState.TotalTodosCompleted,
                WeeklyLogins = otherState.WeeklyLogins,
                CurrentLoginStreak = otherState.CurrentLoginStreak,
                LastLoginDate = otherState.LastLoginDate
            };
        }
    }
}
