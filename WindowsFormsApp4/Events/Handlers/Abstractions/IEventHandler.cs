using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4.Events.Handlers
{
    internal interface IEventHandler
    {
        ValueTask Handle(object state);
    }

    internal interface IEventHandler<T> : IEventHandler
    {
        ValueTask Handle(T state);
    }
}
