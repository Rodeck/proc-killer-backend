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

        public virtual DbSet<TodoItem> Todos { get; set; }

        public virtual DbSet<LevelDefinition> Levels { get; set; }

        public virtual User GetUserById(int id)
        { 
            var user = Users.Include(u => u.UserTodos).Include(u => u.Events).Single(x => x.Id == id);
            user.CalculationService = new Services.StateCalculationService(this);
            return user;
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

            base.OnModelCreating(builder);
        }

        public virtual League GetLeague(int level)
        {
            throw new NotImplementedException();
        }
    }
}
