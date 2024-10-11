using MapWinGIS;
using System;
using System.Linq;

namespace WindowsFormsApp4.Extensions
{
    internal static class CallFormEventsExtensions
    {
        public static void CallAllSubsribers<T1, T2>(this Action<T1, T2> func, T1 arg1, T2 arg2)
        {
            if (func == null)
            {
                return;
            }

            var moveEvents = func.GetInvocationList()
                .OfType<Action<T1, T2>>().ToList();

            foreach (var moveEvent in moveEvents)
            {
                moveEvent.Invoke(arg1, arg2);
            }
        }

        public static void CallAllSubsribers<T1>(this Action<T1> func, T1 arg1)
        {
            if (func == null)
            {
                return;
            }

            var moveEvents = func.GetInvocationList()
                .OfType<Action<T1>>().ToList();

            foreach (var moveEvent in moveEvents)
            {
                moveEvent.Invoke(arg1);
            }
        }

        public static bool CallAllSubsribers<T1, T2>(this Func<T1, T2, bool> predicate, T1 arg1, T2 arg2)
        {
            if (predicate == null)
            {
                return true;
            }

            var moveEvents = predicate.GetInvocationList()
                .OfType<Func<T1, T2, bool>>().ToList();

            foreach (var moveEvent in moveEvents)
            {
                if (!moveEvent.Invoke(arg1, arg2))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
