using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services
{
    public class RewardService : IRewardService
    {
        private readonly ILogger<RewardService> _logger;
        private readonly UsersContext _context;

        public RewardService(UsersContext context, ILogger<RewardService> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task AssignBaseRewards(string userId)
        {
            var existingBadges = _context.Badges
                .Include(x => x.Definition)
                    .ThenInclude(x => x.Conditions)
                .Where(x => x.UserId == userId);

            foreach(var rewardDefinition in _context.BadgeDefinitions.Include(x => x.Conditions).Where(x => x.AssignableFormBeggining))
            {
                _logger.LogInformation($"Assigning {rewardDefinition.Id} to user {userId}");
                if (!existingBadges.Any(x => x.Definition.Id == rewardDefinition.Id))
                {
                    var reward = new Badge()
                    {
                        Definition = rewardDefinition,
                        UserId = userId,
                        Conditions = BuildConditions(rewardDefinition.Conditions.ToList()).ToList(),
                    };

                    await _context.AddAsync(reward);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task Calculate(string userId, int dailyLogins, int points, int totalTodosCompleted)
        {
            foreach(var badge in await _context.GetUserBadges(userId))
            {
                if (!badge.IsFulfiled)
                {
                    foreach(var condition in badge.Conditions)
                    {
                        var definition = badge.GetConditionDefinitin(condition);
                        switch (condition.Condition)
                        {
                            case Conditions.Logins:
                                if (definition.Amount - dailyLogins <= 0)
                                    condition.Amount = definition.Amount;
                                else
                                    condition.Amount = dailyLogins;
                                break;
                            case Conditions.PointsEarned:
                                if (definition.Amount - points <= 0)
                                    condition.Amount = definition.Amount;
                                else
                                    condition.Amount = points;
                                break;
                            case Conditions.TodosCompleted:
                                if (definition.Amount - totalTodosCompleted <= 0)
                                    condition.Amount = definition.Amount;
                                else
                                    condition.Amount = totalTodosCompleted;
                                break;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Badge>> GetRewards(string userId)
        {
            return await _context.GetUserBadges(userId);
        }

        private IEnumerable<RewardCondition> BuildConditions(List<RewardCondition> rewardConditions)
        {
            foreach(var condition in rewardConditions)
            {
                yield return new RewardCondition()
                {
                    Amount = 0,
                    Condition = condition.Condition,
                };
            }
        }
    }
}
