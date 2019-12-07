using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public Task<IEnumerable<Badge>> Rewards()
        {
            _logger.LogInformation($"Getting rewards for user {GetUserId()}");
            return _rewardService.GetRewards(GetUserId());
        }
    }
}