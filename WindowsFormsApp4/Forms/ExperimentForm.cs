using DynamicForms.Abstractions;
using Entities;
using System.Windows.Forms;
using Tools;

namespace Forms.Forms
{
    public partial class ExperimentForm : Form, IEntityForm<Experiment>
    {
        public ExperimentForm(EditMode editMode) 
            : this(new Experiment(), editMode)
        {
            
        }

        public ExperimentForm(Experiment experiment, EditMode editMode) : this()
        {
            if (editMode == EditMode.Add)
            {
                gasId.Visible = false;
            }

            Entity = experiment;
        }

        private ExperimentForm()
        {
            InitializeComponent();
        }

        public Experiment Entity { get; }
    }
}
