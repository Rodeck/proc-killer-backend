using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoItWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProcastinationKiller.Controllers.Abstract;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services.Abstract;

namespace ProcastinationKiller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RewardsController : BaseController
    {
        private IRewardService _rewardService;
        private ILogger<RewardsController> _logger;

        public RewardsController(IRewardService rewardService, ILogger<RewardsController> logger)
        {
            _rewardService = rewardService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<BadgeViewModel>> Rewards()
        {
            _logger.LogInformation($"Getting rewards for user {GetUserId()}");
            var result = await _rewardService.GetRewards(GetUserId());

            return result.Select(x => {
                var conditions = x.Conditions.Select(y => new BadgeConditionViewModel()
                {
                    ActualAmount = y.Amount,
                    Condition = Enum.GetName(typeof(Conditions), y.Condition),
                    RequiredAmount = x.GetConditionDefinitin(y).Amount,
                    Id = y.Id
                });

                var badge = new BadgeViewModel()
                {
                    AcquiredDate = x.AcquiredDate,
                    Id = x.Id,
                    IsFulfiled = x.IsFulfiled,
                    Image = x.Definition.Image,
                    Conditions = conditions,
                    Percentage = (float)conditions.Sum(y => y.ActualAmount) / (float)conditions.Sum(y => y.RequiredAmount)
                };

                return badge;
            });
        }
    }
}