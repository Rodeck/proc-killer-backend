using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Helpers
{
    public static class SystemSettings
    {
        public static int MaxDayTodos => 10;

        public static int PointsForCompletition => 10;

        public static int DailyLoginReward => 20;

        public static decimal DailyLoginStreakMultiplier => 0.3m;

        public static int DailyLoginStreakCap => 100;
    }
}
