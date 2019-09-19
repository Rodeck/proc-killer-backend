using NSubstitute;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace TodosTests
{
    public class StateCalculationServiceTests
    {
        private IDefinitionsContext _defContext;

        public StateCalculationServiceTests()
        {
            var leagues = new League[]
            {
                new League()
                {
                    Id = 1,
                    Name = "x"
                },
                new League()
                {
                    Id = 2,
                    Name = "y"
                },
                new League()
                {
                    Id = 3,
                    Name = "z"
                },
                new League()
                {
                    Id = 4,
                    Name = "a"
                },
            };

            _defContext = Substitute.For<IDefinitionsContext>();

            _defContext.GetLevel(1).Returns(new LevelDefinition()
            {
                Id = 1,
                League = leagues.Single(x => x.Id == 1),
                Number = 1,
                RequiredExp = 100
            });

            _defContext.GetLevel(2).Returns(new LevelDefinition()
            {
                Id = 1,
                League = leagues.Single(x => x.Id == 1),
                Number = 1,
                RequiredExp = 200
            });

            _defContext.GetLeague(Arg.Any<int>()).Returns(x => leagues.Single(y => y.Id == (int)x[0]));
        }

        [Fact(DisplayName = "[CalcService] Dodanie operacji zakończenia todo przelicza poprawnie stan.")]
        public void Test_CompleteTodo()
        {
            StateCalculationService calculationService = new StateCalculationService(_defContext);

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
            StateCalculationService calculationService = new StateCalculationService(_defContext);

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
