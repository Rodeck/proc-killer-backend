using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface ITodoEngine
    {
        Task StartAsync();

        void AttachTask(Action<IServiceProvider> t);

        void Configure(IServiceCollection sc);

        ITodoEngine Build();
    }

    public class TodoEngine : ITodoEngine
    {
        private List<Task<Action<IServiceProvider>>> _tasks = new List<Task<Action<IServiceProvider>>>();

        private IServiceProvider _serviceProvider;

        private IServiceCollection _services;

        public void AttachTask(Action<IServiceProvider> t)
        {
            _tasks.Add(new Task<Action<IServiceProvider>>(() => t));
        }

        public ITodoEngine Build()
        {
            _serviceProvider = _services.BuildServiceProvider();
            return this;
        }

        public void Configure(IServiceCollection sc)
        {
            _services = sc;
        }

        public async Task StartAsync()
        {
            List<Task> tasks = new List<Task>();

            /*foreach(var action in _tasks)
            {
                tasks.Add(Task.Factory.(() => action(_serviceProvider)));
            }
            */

            Parallel.For(0, tasks.Count, async (x) =>
            {
                await _tasks[x];
            });

            //Task.WaitAll(tasks.ToArray());
        }
    }
}
