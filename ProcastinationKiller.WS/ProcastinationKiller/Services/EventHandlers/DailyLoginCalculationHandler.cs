using ProcastinationKiller.Helpers;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services.EventHandlers
{
    public class DailyLoginCalculationHandler : IEventCalculationHandler<DailyLoginEvent>
    {
        public UserState Calculate(DailyLoginEvent @event, UserState currentState)
        {
            int points = SystemSettings.DailyLoginReward;
            currentState.Points += points;
            currentState.DailyLogins++;

            // Jeśli poprzedniego dnia było logowanie to mamy login streak
            if (currentState.LastLoginDate.HasValue &&
                currentState.LastLoginDate.Value.AddDays(1).Date == @event.Date.Date)
            {
                // Zwiększ login streak
                currentState.CurrentLoginStreak += 1;

                // Jeśli najdłuższy login streak jest krótszy niż aktualny login streak przypisz aktualny jako najdłuższy
                if (currentState.LongestLoginStreak < currentState.CurrentLoginStreak)
                    currentState.LongestLoginStreak = currentState.CurrentLoginStreak;

                // Dodaj punkty za login streak
                points += Math.Min(
                    (int)((currentState.CurrentLoginStreak * SystemSettings.DailyLoginStreakMultiplier) * SystemSettings.DailyLoginReward),
                    SystemSettings.DailyLoginStreakCap);

                currentState.Points += points;
            }

            currentState.LastLoginDate = @event.Date;

            @event.Points = points;

            @event.State = currentState;
            return currentState;
        }
    }
}
