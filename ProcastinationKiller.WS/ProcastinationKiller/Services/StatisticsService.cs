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

        private User GetUser(int userId)
        {
            return _context.Users
                .Include(x => x.Events)
                    .ThenInclude(e => e.State)
                .SingleOrDefault(x => x.Id == userId);
        }
    }
}
