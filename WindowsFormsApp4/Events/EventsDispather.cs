using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp4.Events.Handlers;

namespace WindowsFormsApp4.Events
{
    internal class EventsDispather
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;

        public EventsDispather(IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await dispatchEvents();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            };
        }

        private async Task dispatchEvents()
        {
            if (!_memoryCache.TryGetValue("EventsBus:Events", out ConcurrentQueue<object> eventsList) ||
                eventsList.Count == 0)
            {
                return;
            }

            var tasks = new List<Task>();
            while (eventsList.TryDequeue(out var @event))
            {
                tasks.Add(handleEvent(@event));
            }

            await Task.WhenAll(tasks);
        }

        private async Task handleEvent(object @event)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var eventType = @event.GetType();
                var handler = scope.ServiceProvider.GetService(typeof(IEventHandler<>).MakeGenericType(eventType));
                if (handler == null)
                {
                    return;
                }

                try
                {
                    await (handler as IEventHandler).Handle(@event);
                }
                catch (Exception ex)
                {
                    Debugger.Log(1, "Event handlers", ex.Message);
                }
            }
        }
    }
}
