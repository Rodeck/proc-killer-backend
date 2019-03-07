using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ProcastinationKiller;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            

            var serviceCollection = new ServiceCollection();
            ConfigureSharedServices(serviceCollection);

            var engine = new TodoEngine();
            engine.Configure(serviceCollection);
            engine.AttachTask(CloseUnfinishedTodos);
            engine.AttachTask((sp) => 
            {
                while (true)
                {
                    Console.WriteLine("Some other work to do.");
                    Thread.Sleep(1100);
                }
            });
            engine.AttachTask((sp) =>
            {
                CreateWebHostBuilder(args).Build().Run();
            });

            var build = engine.Build();
            build.StartAsync();

            CreateWebHostBuilder(args).Build().Run();

        }

        private static void CloseUnfinishedTodos(IServiceProvider sp)
        {
            while(true)
            {
                try
                {
                }
                catch(Exception ex)
                {

                }
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(sc =>
                {
                    // configure DI for application services
                    ConfigureSharedServices(sc);
                })
                .UseStartup<Startup>();

        private static void ConfigureSharedServices(IServiceCollection sc)
        {
            sc.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            sc.AddScoped<IUserService, UserService>();
            sc.AddDbContext<UsersContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            sc.AddSingleton(new LogFactory());
        }
    }
}
