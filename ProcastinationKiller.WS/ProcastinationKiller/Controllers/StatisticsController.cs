using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcastinationKiller.Controllers.Abstract;
using ProcastinationKiller.Models.Responses;
using ProcastinationKiller.Models.Responses.Abstract;
using ProcastinationKiller.Services.Abstract;

namespace ProcastinationKiller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatisticsController : BaseController
    {
        private IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        [Route("pointsPerDay/{userId:int}")]
        public IServiceResult<PointsPerDayModel[]> GetPointsPerDayChart(int userId)
        {
            try
            {
                var statistics = _statisticsService.CalculatePointsPerDay(userId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return Error<PointsPerDayModel[]>(ex);
            }
            
        }
    }
}