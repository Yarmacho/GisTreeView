using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4.Events
{
    public interface IEventBus
    {
        void Publish(object @event);
    }

    internal class EventBus : IEventBus
    {
        private readonly IMemoryCache _cache;

        public EventBus(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Publish(object @event)
        {
            if (!_cache.TryGetValue("EventsBus:Events", out ConcurrentQueue<object> eventsList))
            {
                eventsList = new ConcurrentQueue<object>();
                _cache.Set("EventsBus:Events", eventsList);
            }

            eventsList.Enqueue(@event);
        }
    }
}
