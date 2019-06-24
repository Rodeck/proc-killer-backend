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
        [Fact(DisplayName = "[CalcService] Dodanie operacji zakończenia todo przelicza poprawnie stan.")]
        public void Test_CompleteTodo()
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

        [Theory(DisplayName = "[CalcService] Dodanie operacji logowania przelicza poprawnie stan.")]
        [MemberData(nameof(LoginTestData))]
        public void Test_Login(IEnumerable<BaseEvent> events, UserState expectedState)
        {
            StateCalculationService calculationService = new StateCalculationService();

            var currentState = calculationService.Calculate(events, DateTime.Now);

            Assert.Equal(expectedState, currentState, new UserStateComparer());
        }

        public class UserStateComparer : IEqualityComparer<UserState>
        {
            public bool Equals(UserState x, UserState y)
            {
                return x.LastLoginDate?.Date == y.LastLoginDate?.Date &&
                    x.LongestLoginStreak == y.LongestLoginStreak &&
                    x.Points == y.Points &&
                    x.TotalTodosCompleted == y.TotalTodosCompleted &&
                    x.WeeklyLogins == y.WeeklyLogins &&
                    x.CurrentLoginStreak == y.CurrentLoginStreak &&
                    x.DailyLogins == y.DailyLogins;
            }

            public int GetHashCode(UserState obj)
            {
                return obj.GetHashCode();
            }
        }

        public static IEnumerable<object[]> LoginTestData()
        {
            yield return new object[]
            {
                new List<BaseEvent>()
                {
                    new DailyLoginEvent() { Date = DateTime.Now.AddDays(-5) },
                    new DailyLoginEvent() { Date = DateTime.Now.AddDays(-4) },
                    new DailyLoginEvent() { Date = DateTime.Now.AddDays(-3) },
                    new DailyLoginEvent() { Date = DateTime.Now.AddDays(-2) },
                    new DailyLoginEvent() { Date = DateTime.Now.AddDays(-1) },
                    new DailyLoginEvent() { Date = DateTime.Now }
                },
                new UserState()
                {
                    CurrentLoginStreak = 5,
                    DailyLogins = 6,
                    LastLoginDate = DateTime.Now,
                    LongestLoginStreak = 5,
                    Points = 210
                }
            };

            yield return new object[]
{
                new List<BaseEvent>()
                {
                    new DailyLoginEvent() { Date = DateTime.Now.AddDays(-10) },
                    new DailyLoginEvent() { Date = DateTime.Now.AddDays(-8) },
                    new DailyLoginEvent() { Date = DateTime.Now.AddDays(-6) },
                    new DailyLoginEvent() { Date = DateTime.Now.AddDays(-4) },
                    new DailyLoginEvent() { Date = DateTime.Now.AddDays(-2) },
                    new DailyLoginEvent() { Date = DateTime.Now }
                },
                new UserState()
                {
                    CurrentLoginStreak = 0,
                    DailyLogins = 6,
                    LastLoginDate = DateTime.Now,
                    LongestLoginStreak = 0,
                    Points = 120
                }
};
        }
    }
}
