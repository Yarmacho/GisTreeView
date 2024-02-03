using System.Windows.Forms;
using Tools;

namespace DynamicForms.Forms
{
    internal class EntityForm : Form, IEntityForm
    {
        internal object Entity { get; }
        public EntityForm(object entity)
        {
            Entity = entity;
        }

        public T GetEntity<T>()
        {
            return TypeTools.Convert<T>(Entity);
        }

        DialogResult IEntityForm.Activate()
        {
            var result = ShowDialog();
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (Container != null))
            {
                Container.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
