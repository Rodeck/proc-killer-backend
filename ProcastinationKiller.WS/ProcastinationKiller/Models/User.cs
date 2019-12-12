using ProcastinationKiller.Models.Enums;
using ProcastinationKiller.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
        
        public string Token { get; set; }

        public string Email { get; set; }

        public DateTime Regdate { get; set; }

        public ICollection<TodoItem> UserTodos { get; set; }

        public ICollection<BaseEvent> Events { get; set; }

        public UserStatus UserStatus { get; set; }

        public RegistartionCode Code { get; set; }

        public UserState CurrentState { get; set; }

        // Ogarnąc jak zmusić EF do DI
        public StateCalculationService CalculationService;

        public User()
        {
            UserTodos = new HashSet<TodoItem>();
            _days = _days ?? new HashSet<Day>();
            Friends = new HashSet<Friend>();
            FriendsInvitations = new HashSet<FriendsInvitation>();
            MyInvitations = new HashSet<MyInvitation>();
        }

        [NotMapped]
        public ICollection<Day> Callendar
        {
            get
            {
                InitializeCallendar();
                return _days;
            }
            private set
            {

            }
        }

        private ICollection<Day> _days;

        public virtual ICollection<Friend> Friends { get; set; }

        public virtual ICollection<FriendsInvitation> FriendsInvitations { get; set; }

        public virtual ICollection<MyInvitation> MyInvitations { get; set; }

        private void InitializeCallendar()
        {

            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            for(int day = 1; day <= days; day++)
            {
                var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);

                if (UserTodos.Any(x => x.TargetDate.Date == date))
                {
                    _days.Add(new Day()
                    {
                        Todos = UserTodos.Where(x => x.TargetDate.Date == date).ToList(),
                        AllCompleted = UserTodos.Where(x => x.TargetDate.Date == date).All(x => x.Completed),
                        Date = date
                    });
                }
                else
                {
                    _days.Add(new Day()
                    {
                        Date = date,
                        Todos = new List<TodoItem>()
                    });
                }
            }

            foreach (var todo in UserTodos.Where(x => !(x.TargetDate.Month == DateTime.Now.Month && x.TargetDate.Year == DateTime.Now.Year)))
            {
                Day day = _days.SingleOrDefault(x => x.Date.Date == todo.TargetDate.Date);

                if (day == null)
                {
                    day = new Day();
                    day.Date = todo.TargetDate;
                    _days.Add(day);
                }

                if (day.Todos == null)
                {
                    day.Todos = new HashSet<TodoItem>();
                }

                day.Todos.Add(todo);

                if (day.AllCompleted && !todo.Completed)
                    day.AllCompleted = false;
            }
        }

        internal void AddTodoCompletedEvent(TodoItem todo)
        {
            if (Events.OfType<TodoCompletedEvent>().Any(x => x.CompletedItem == todo && !x.Hidden))
            {
                throw new Exception("User already have event about completeing this item.");
            }

            AddEvents(new TodoCompletedEvent()
            {
                Hidden = false,
                Date = todo.FinishTime.Value,
                CompletedItem = todo
            });
        }

        public void AddCode(string code)
        {
            UserStatus = UserStatus.New;
            Code = new RegistartionCode()
            {
                Code = code,
                IsConfirmed = false
            };
        }

        /// <summary>
        /// Aktywuj konto użytkownika
        /// </summary>
        /// <param name="code"></param>
        public void Activate(string code)
        {
            if (this.Code.Code == code)
            {
                UserStatus = UserStatus.Confirmed;
                Code.Confirm();
            }
            else
            {
                throw new Exception("Provided activation code is invalid.");
            }
        }

        public void AcceptInvitation(int invitationId)
        {
            var invitation = FriendsInvitations.Single(x => x.Id == invitationId);

            Friends.Add(new Friend()
            {
                FriendId = invitation.InviterId,
                FriendName = invitation.InviterName,
                IsAccepted = true,
            });

            invitation.Accepted = true;
            invitation.AcceptedDate = DateTime.Now;
        }

        public void RejectInvitation(int invitationId)
        {
            var invitation = FriendsInvitations.Single(x => x.Id == invitationId);

            invitation.Rejected = false;
            invitation.RejectedDate = DateTime.Now;
        }

        public FriendsInvitation AddFriendInvitation(string inviterId, string inviterName)
        {
            var invitation = new FriendsInvitation()
            {
                Icon = "fa-user-plus",
                InvitationDate = DateTime.Now,
                InviterId = inviterId,
                InviterName = inviterName,
            };

            FriendsInvitations.Add(invitation);

            return invitation;
        }

        public void AddMyInvitation(string invitedId)
        {
            MyInvitations.Add(new MyInvitation()
            {
                InvitationDate = DateTime.Now,
                InvitedId = invitedId,
            });
        }

        public void MyInvitationAccepted(string invitedId, string friendName)
        {
            var myInvitation = MyInvitations.Single(x => x.InvitedId == invitedId);

            myInvitation.IsAccepted = true;

            Friends.Add(new Friend()
            {
                FriendId = invitedId,
                FriendName = friendName,
                IsAccepted = true,
            });
        }

        public void MyInvitationReject(string invitedId)
        {
            var myInvitation = MyInvitations.Single(x => x.InvitedId == invitedId);

            myInvitation.IsRejected = true;
        }

        internal void AddDailyLoginReward(DateTime currentTime)
        {
            if (Events.OfType<DailyLoginEvent>().Any(x => x.Date.Date == currentTime.Date && !x.Hidden))
                return;

            AddEvents(new DailyLoginEvent()
            {
                Date = currentTime,
                Hidden = false
            });

            if (ShouldAddWeeklyEvent(currentTime))
            {
                AddEvents(new WeeklyLoginEvent()
                {
                    Date = currentTime,
                    Hidden = false
                });
            }
        }

        internal void AddEvents(IEnumerable<BaseEvent> events)
        {
            Events = Events.Concat(events).ToList();
            this.CurrentState = CalculationService.Calculate(Events, DateTime.Now);
        }

        internal void AddEvents(BaseEvent @event)
        {
            Events.Add(@event);
            this.CurrentState = CalculationService.Calculate(Events, DateTime.Now);
        }

        internal void HideEvent(int eventId)
        {
            Events.Single(x => x.Id == eventId).Hidden = true;
            this.CurrentState = CalculationService.Calculate(Events, DateTime.Now);
        }

        private bool ShouldAddWeeklyEvent(DateTime currentTime)
        {
            var expectedDates = new List<DateTime>()
            {
                currentTime,
                currentTime.AddDays(-1),
                currentTime.AddDays(-2),
                currentTime.AddDays(-3),
                currentTime.AddDays(-4),
                currentTime.AddDays(-5),
                currentTime.AddDays(-6),
            };

            var dailyLoginOperations = Events.OfType<DailyLoginEvent>().Where(x => !x.Hidden).Select(x => x.Date);

            return expectedDates.All(x => dailyLoginOperations.Any(y => y.Date == x.Date))
                && !Events.OfType<WeeklyLoginEvent>()
                    .Any(x => 
                        x.Date.Date >= currentTime.AddDays(-7) 
                        && x.Date.Date <= currentTime.Date);
        }

        internal void Calculate()
        {
            CurrentState = CalculationService.Calculate(Events, DateTime.Now);
        }
    }
}
