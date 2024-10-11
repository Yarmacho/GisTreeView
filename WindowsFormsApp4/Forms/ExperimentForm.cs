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
                label3.Visible = false;
            }

            Entity = experiment;
        }

        private ExperimentForm()
        {
            InitializeComponent();
            AcceptButton = submit;
            AcceptButton.DialogResult = DialogResult.OK;
        }

        public Experiment Entity { get; }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Entity.Name = name.Text;
            Entity.Description = description.Text;
        }
    }
}
