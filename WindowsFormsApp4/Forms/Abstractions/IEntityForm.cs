using System;
using System.Windows.Forms;

namespace DynamicForms.Abstractions
{
    public interface IEntityForm<T>
    {
        T Entity { get; }

        DialogResult ShowDialog();
        event Action OnEntityFormClosed;
        void CallOnFormClosedEvents();
    }
}
