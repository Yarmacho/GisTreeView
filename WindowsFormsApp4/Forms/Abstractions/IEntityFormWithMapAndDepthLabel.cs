using DynamicForms.Abstractions;
using System.Windows.Forms;

namespace WindowsFormsApp4.Forms.Abstractions
{
    public interface IEntityFormWithMapAndDepthLabel<T> : IEntityFormWithMap<T>
    {
        Label DepthLabel { get; } 
    }
}
