using System.Threading.Tasks;

namespace WindowsFormsApp4.Events.Handlers
{
    internal abstract class EventHandlerBase<T> : IEventHandler<T>
    {
        public abstract ValueTask Handle(T state);

        ValueTask IEventHandler.Handle(object state)
        {
            return Handle((T)state);
        }
    }
}
