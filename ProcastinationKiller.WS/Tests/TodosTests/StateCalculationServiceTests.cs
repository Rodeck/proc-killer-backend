using ProcastinationKiller.Models;
using ProcastinationKiller.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TodosTests
{
    public class StateCalculationServiceTests
    {
        [Fact(DisplayName = "Test")]
        public void Test()
        {
            StateCalculationService calculationService = new StateCalculationService();

            var @events = new List<BaseEvent>()
            {
                new TodoCompletedEvent()
                {
                    Date = DateTime.Now.AddDays(-3)
                },
                new TodoCompletedEvent()
                {
                    Date = DateTime.Now.AddDays(-2)
                },
                new TodoCompletedEvent()
                {
                    Date = DateTime.Now.AddDays(-1)
                },
                new TodoCompletedEvent()
                {
                    Date = DateTime.Now.AddDays(0)
                },
                new TodoCompletedEvent()
                {
                    Date = DateTime.Now.AddDays(1)
                }
            };

            var currentState = calculationService.Calculate(events, DateTime.Now);

            Assert.Equal(40, currentState.Points);
        }
    }
}
