using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.Extensions;
using ProcastinationKiller.Exceptions;
using ProcastinationKiller.Helpers;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace TodosTests
{
    public class UserServiceTests
    {

        private readonly UsersContext _ctx;
        private readonly IEncryptor _encryptor;
        private readonly IRewardService _rewards;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            var opt = new DbContextOptions<UsersContext>();
            _ctx = Substitute.ForPartsOf<UsersContext>(opt);

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

            _ctx.Configure().GetLevel(1).Returns(new LevelDefinition()
            {
                Id = 1,
                League = leagues.Single(x => x.Id == 1),
                Number = 1,
                RequiredExp = 100
            });

            _ctx.Configure().GetLevel(2).Returns(new LevelDefinition()
            {
                Id = 1,
                League = leagues.Single(x => x.Id == 1),
                Number = 1,
                RequiredExp = 200
            });

            _ctx.Configure().GetLeague(Arg.Any<int>()).Returns(x => leagues.Single(y => y.Id == (int)x[0]));

            _ctx.When(x => x.SaveChanges()).DoNotCallBase();

            var options = Options.Create<AppSettings>(new AppSettings()
            {
                Secret = "aasdas_afassqyqw"
            });

            var templateProvider = new FileTemplateProvider();

            var mailProvider = new MailProvider(templateProvider);

            _encryptor = Substitute.For<IEncryptor>();
            _rewards = Substitute.For<IRewardService>();
            _userService = new UserService(options, _ctx, Substitute.For<IMailingService>(), mailProvider, _encryptor, _rewards);
        }

        [Fact(DisplayName = "[UserService] Wykonanie todo dodaje poprawny event.")]
        public void FinishTodo_AddsEvent()
        {
            var user = new User()
            {
                Events = new List<BaseEvent>(),
                UserTodos = new List<TodoItem>()
                {
                    new TodoItem()
                    {
                        Completed = false,
                        Description = "desc",
                        Id = 1,
                        TargetDate = DateTime.Now,
                        FinishTime = null,
                    }
                },
                CalculationService = new StateCalculationService(_ctx)
            };

            _ctx.Configure().GetUserById(1).Returns(user);

            _userService.MarkAsCompleted(1, DateTime.Now, 1);

            Assert.Single(user.Events);
            Assert.Single(user.Events.OfType<TodoCompletedEvent>());

            var @event = user.Events.OfType<TodoCompletedEvent>().Single();

            Assert.Equal(user.UserTodos.Single(), @event.CompletedItem);
            Assert.Equal(DateTime.Now.Date, @event.Date.Date);
            Assert.False(@event.Hidden);
        }

        [Fact(DisplayName = "[UserService] Nie mo¿na wykonaæ todo na przysz³oœæ.")]
        public async Task FinishTodoFromFuture_Error()
        {
            var user = new User()
            {
                Events = new List<BaseEvent>(),
                UserTodos = new List<TodoItem>()
                {
                    new TodoItem()
                    {
                        Completed = false,
                        Description = "desc",
                        Id = 1,
                        TargetDate = DateTime.Now.AddDays(1),
                        FinishTime = null,
                    }
                },
                CalculationService = new StateCalculationService(_ctx)
            };

            _ctx.Configure().GetUserById(1).Returns(user);

            var ex = await Record.ExceptionAsync(() => _userService.MarkAsCompleted(1, DateTime.Now, 1));

            Assert.NotNull(ex);
            Assert.IsType<NotAllowedOperation>(ex);
            Assert.Equal("Time traveler, You cannot complete todo from the future!", ex.Message);
        }

        [Fact(DisplayName = "[UserService] Logowanie pierwszy raz danego dnia dodaje event.")]
        public void Login_AddsDailyLogin()
        {
            var user = new User()
            {
                Events = new List<BaseEvent>(),
                Username = "test",
                Password = "test",
                CalculationService = new StateCalculationService(_ctx)
            };

            _ctx.Configure().GetUserForLogin(user.Username, user.Password).Returns(user);

            _userService.Authenticate("test", "test", DateTime.Now);

            Assert.Single(user.Events);
            Assert.Single(user.Events.OfType<DailyLoginEvent>());

            var @event = user.Events.OfType<DailyLoginEvent>().Single();

            Assert.Equal(DateTime.Now.Date, @event.Date.Date);
            Assert.False(@event.Hidden);
        }

        [Fact(DisplayName = "[UserService] Tylko pierwsze logowanie dodaje event.")]
        public void Login_AddsDailyLogin_OnlyOnce()
        {
            var user = new User()
            {
                Events = new List<BaseEvent>(),
                Username = "test",
                Password = "test",
                CalculationService = new StateCalculationService(_ctx)
            };

            _ctx.Configure().GetUserForLogin(user.Username, user.Password).Returns(user);

            _userService.Authenticate("test", "test", DateTime.Now);
            _userService.Authenticate("test", "test", DateTime.Now);

            Assert.Single(user.Events);
            Assert.Single(user.Events.OfType<DailyLoginEvent>());

            var @event = user.Events.OfType<DailyLoginEvent>().Single();

            Assert.Equal(DateTime.Now.Date, @event.Date.Date);
            Assert.False(@event.Hidden);
        }

        [Fact(DisplayName = "[UserService] Logowanie dwa dni pod rz¹d dodaje dwie operacje.")]
        public void Login_AddsDailyLogin_TwoDays()
        {
            var user = new User()
            {
                Events = new List<BaseEvent>(),
                Username = "test",
                Password = "test",
                CalculationService = new StateCalculationService(_ctx)
            };

            _ctx.Configure().GetUserForLogin(user.Username, user.Password).Returns(user);

            List<DateTime> dates = new List<DateTime>()
            {
                DateTime.Now,
                DateTime.Now.AddDays(1)
            };

            _userService.Authenticate("test", "test", dates[0]);

            Assert.Single(user.Events);
            Assert.Single(user.Events.OfType<DailyLoginEvent>());

            var @event = user.Events.OfType<DailyLoginEvent>().Single();

            Assert.Equal(DateTime.Now.Date, @event.Date.Date);
            Assert.False(@event.Hidden);

            _userService.Authenticate("test", "test", dates[1]);

            Assert.Equal(2, user.Events.Count);
            Assert.Equal(2, user.Events.OfType<DailyLoginEvent>().Count());


            Assert.All(dates, x =>
            {
                var e = user.Events.OfType<DailyLoginEvent>().SingleOrDefault(y => y.Date.Date == x.Date.Date && !y.Hidden);
                Assert.NotNull(e);
            });
        }

        [Fact(DisplayName = "[UserService] 7 logowañ pod rz¹d dodaje te¿ tygodniowy event.")]
        public void Login_AddsDailyLogin_7Days()
        {
            var user = new User()
            {
                Events = new List<BaseEvent>(),
                Username = "test",
                Password = "test",
                CalculationService = new StateCalculationService(_ctx)
            };

            _ctx.Configure().GetUserForLogin(user.Username, user.Password).Returns(user);

            List<DateTime> dates = new List<DateTime>()
            {
                DateTime.Now,
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(2),
                DateTime.Now.AddDays(3),
                DateTime.Now.AddDays(4),
                DateTime.Now.AddDays(5),
                DateTime.Now.AddDays(6),
                DateTime.Now.AddDays(7)
            };

            foreach (var date in dates)
                _userService.Authenticate("test", "test", date);

            Assert.Equal(9, user.Events.Count);
            Assert.Equal(8, user.Events.OfType<DailyLoginEvent>().Count());
            Assert.Single(user.Events.OfType<WeeklyLoginEvent>());

            var weeklyEvent = user.Events.OfType<WeeklyLoginEvent>().Single();

            Assert.False(weeklyEvent.Hidden);
            Assert.Equal(DateTime.Now.AddDays(6).Date, weeklyEvent.Date.Date);

            Assert.All(dates, x =>
            {
                var @event = user.Events.OfType<DailyLoginEvent>().SingleOrDefault(y => y.Date.Date == x.Date.Date && !y.Hidden);
                Assert.NotNull(@event);
            });
        }

        [Fact(DisplayName = "[UserService] Aktywacja konta - sukces")]
        public void ActivateAccount()
        {
            var json = JsonConvert.SerializeObject(new ActivationModel()
            {
                GenerationDate = DateTime.Now,
                Username = "test",
                Password = "test",
                Code = "19998ggasds"
            }, new JsonSerializerSettings{
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            });

            _encryptor.Encrypt(json).Returns("xxx");
            _encryptor.Decrypt("xxx").Returns(json);

            var encoded = _encryptor.Encrypt(json);

            var secret = HttpUtility.UrlEncode(encoded);

            var user = new User()
            {
                Events = new List<BaseEvent>(),
                Username = "test",
                Password = "test",
                CalculationService = new StateCalculationService(_ctx),
                Code = new RegistartionCode()
                {
                    Code = "19998ggasds",
                    IsConfirmed = false,
                    ConfirmationDate = null
                },
                UserStatus = ProcastinationKiller.Models.Enums.UserStatus.New
            };

            _ctx.Configure().GetUserForLogin(user.Username, user.Password).Returns(user);

            _userService.ActivateAccount(secret);

            Assert.True(user.Code.IsConfirmed);
            Assert.Equal(DateTime.Now.Date, user.Code.ConfirmationDate.GetValueOrDefault().Date);
            Assert.Equal(ProcastinationKiller.Models.Enums.UserStatus.Confirmed, user.UserStatus);
        }
    }
}
