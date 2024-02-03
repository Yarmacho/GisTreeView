using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicForms.Forms
{
    public interface IEntityForm
    {
        T GetEntity<T>();

        DialogResult Activate();
    }
}
