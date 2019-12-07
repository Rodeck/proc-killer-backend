using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public interface IDefinitionsContext
    {
        LevelDefinition GetLevel(int number);
        League GetLeague(int level);
    }

    public class UsersContext : DbContext, IDefinitionsContext
    {
        public UsersContext(DbContextOptions<UsersContext> opt)
            : base(opt)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<BadgeDefinition> BadgeDefinitions { get; set; }

        public virtual DbSet<TodoItem> Todos { get; set; }

        public virtual DbSet<LevelDefinition> Levels { get; set; }

        public virtual DbSet<Badge> Badges { get; set; }

        public virtual User GetUserById(int id)
        { 
            var user = Users.Include(u => u.UserTodos).Include(u => u.Events).Single(x => x.Id == id);
            user.CalculationService = new Services.StateCalculationService(this);
            return user;
        }

        public virtual User GetUserById(string id)
        { 
            var user = Users.Include(u => u.UserTodos).Include(u => u.Events).Single(x => x.UId == id);
            user.CalculationService = new Services.StateCalculationService(this);
            return user;
        }

        public virtual async Task<List<Badge>> GetUserBadges(string userId)
        {
            return await Badges
                .Include(x => x.Conditions)
                .Include(x => x.Definition)
                    .ThenInclude(x => x.Conditions)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public virtual User GetUserForLogin(string username, string password)
        {
           var user = Users
                .Include(x => x.Events)
                .Include(x => x.Code)
                .Include(x => x.CurrentState)
                    .ThenInclude(x => x.Level)
                        .ThenInclude(x => x.Definition)
                            .ThenInclude(x => x.League)
                .SingleOrDefault(x => x.Username == username && x.Password == password);
            if (user != null)
                user.CalculationService = new Services.StateCalculationService(this);
           return user;
        }

        public virtual LevelDefinition GetLevel(int number)
        {
            return Levels
                .Include(x => x.League)
                .Where(x => x.Number == number)
                .SingleOrDefault() ?? throw new Exception($"Could not find definition for level {number}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TodoCompletedEvent>();
            builder.Entity<DailyLoginEvent>();
            builder.Entity<WeeklyLoginEvent>();
            builder.Entity<Badge>().ToTable("Badge");

            base.OnModelCreating(builder);
        }

        public virtual League GetLeague(int level)
        {
            throw new NotImplementedException();
        }
    }
}
