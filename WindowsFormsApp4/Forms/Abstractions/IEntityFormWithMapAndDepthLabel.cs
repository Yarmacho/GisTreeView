using DynamicForms.Abstractions;
using System.Windows.Forms;

namespace WindowsFormsApp4.Forms.Abstractions
{
    public interface IEntityFormWithMapAndDepthLabel<T> : IEntityFormWithMap<T>
        where T : new()
    {
        Label DepthLabel { get; } 
    }
}
