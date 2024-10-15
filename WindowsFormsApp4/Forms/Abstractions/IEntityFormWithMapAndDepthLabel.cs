using DynamicForms.Abstractions;
using System;
using System.Windows.Forms;
using WindowsFormsApp4.Initializers;

namespace WindowsFormsApp4.Forms.Abstractions
{
    public interface IEntityFormWithMapAndDepthLabel
    {
        Map Map { get; }
        Label DepthLabel { get; }

        event Action<double, double> OnMouseMoveOnMap;
    }

    public interface IEntityFormWithMapAndDepthLabel<T> : IEntityFormWithMap<T>, IEntityFormWithMapAndDepthLabel
        where T : new()
    {
    }
}
