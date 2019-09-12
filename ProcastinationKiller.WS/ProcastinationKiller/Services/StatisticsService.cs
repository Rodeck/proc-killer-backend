using Microsoft.EntityFrameworkCore;
using ProcastinationKiller.Models;
using ProcastinationKiller.Models.Responses;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services
{
    public class StatisticsService : IStatisticsService
    {

        private readonly UsersContext _context;

        public StatisticsService(UsersContext context)
        {
            _context = context;
        }

        public PointsPerDayModel[] CalculatePointsPerDay(int userId)
        {
            var user = GetUser(userId);
            var result = new List<PointsPerDayModel>();

            foreach (var @event in user.Events
                .Where(x => !x.Hidden)
                .GroupBy(x => x.Date.Date)
                .Select(x => x.Last()))
            {
                if (result.Any())
                {
                    var lastEventPoints = result.Sum(x => x.Points);
                    result.Add(new PointsPerDayModel()
                    {
                        Date = @event.Date,
                        Points = @event.State.Points - lastEventPoints
                    });
                }
                else
                {
                    result.Add(new PointsPerDayModel()
                    {
                        Date = @event.Date.Date,
                        Points = @event.State.Points
                    });
                }
            }

            return result.ToArray();
        }

        public CumulativeResult[] GetCumulativeCompletedTodos(int userId)
        {
            var user = GetUsersTodos(userId);
            List<CumulativeResult> result = new List<CumulativeResult>();

            DateTime minDate = user.UserTodos.Min(x => x.TargetDate);
            DateTime maxDate = user.UserTodos.Max(x => x.TargetDate);

            foreach (DateTime day in EachDay(minDate, maxDate))
            {
                int thisDayTodos = user.UserTodos.Count(x => x.TargetDate.Date == day.Date);
                int completed = user.UserTodos.Count(x => x.TargetDate.Date == day.Date && x.Completed);
                int notCompleted = thisDayTodos - completed;
                CumulativeResult todosCumulativeModel = new CumulativeResult()
                {
                    Date = day,
                    Completed = result.Any()
                        ? result.Last().Completed + completed
                        : completed,
                    All = result.Any()
                        ? result.Last().All + thisDayTodos
                        : thisDayTodos,
                    NotCompleted = result.Any()
                        ? result.Last().NotCompleted + notCompleted
                        : notCompleted,
                };

                result.Add(todosCumulativeModel);
            }

            return result.ToArray();
        }

        private User GetUser(int userId)
        {
            return _context.Users
                .Include(x => x.Events)
                    .ThenInclude(e => e.State)
                .SingleOrDefault(x => x.Id == userId);
        }

        private User GetUsersTodos(int userId)
        {
            return _context.Users
                .Include(x => x.UserTodos)
                .SingleOrDefault(x => x.Id == userId);
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}
