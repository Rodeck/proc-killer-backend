using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class UsersContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=systemdb.db");
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<TodoItem> Todos { get; set; }

        public virtual User GetUserById(int id)
        {
            return Users.Include(u => u.UserTodos).Include(u => u.Events).Single(x => x.Id == id);
        }

        public virtual User GetUserForLogin(string username, string password)
        {
            return Users.Include(x => x.Events).SingleOrDefault(x => x.Username == username && x.Password == password);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TodoCompletedEvent>();
            builder.Entity<DailyLoginEvent>();
            builder.Entity<WeeklyLoginEvent>();

            base.OnModelCreating(builder);
        }
    }
}
