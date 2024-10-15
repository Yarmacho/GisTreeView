using DynamicForms.Abstractions;
using Entities;
using Entities.Entities;
using Forms.Forms;
using Tools;

namespace WindowsFormsApp4.Forms.Abstractions
{
    internal class FormsSelector
    {
        public static IEntityForm<T> Select<T>(T entity = default, EditMode editMode = EditMode.Add)
        {
            switch (entity)
            {
                case Experiment experiment:
                    return new ExperimentForm(experiment, editMode) as IEntityForm<T>;
                case Gas gas:
                    return new GasForm(gas, editMode) as IEntityForm<T>;
                case Ship ship:
                    return new ShipForm(ship, editMode) as IEntityForm<T>;
                case Scene scene:
                    return new SceneForm(scene, editMode) as IEntityForm<T>;
                case Route route:
                    return new RoutesForm(route, editMode) as IEntityForm<T>;
                default:
                    throw new System.NotImplementedException("Form for provided entity is not implemented");
            }
        }
    }
}
