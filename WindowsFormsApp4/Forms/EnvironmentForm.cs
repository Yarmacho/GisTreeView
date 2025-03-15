using Entities.Entities;
using System.Windows.Forms;

namespace WindowsFormsApp4.Forms
{
    public partial class EnvironmentForm : Form
    {
        public EnvironmentForm(ExperimentEnvironment environment)
        {
            InitializeComponent();
            AcceptButton = button1;
            AcceptButton.DialogResult = DialogResult.OK;

            if (environment != null)
            {
                numericTextBox1.Text = environment.ReflectionCoef.ToString();
            }
        }

        public double ReflectionCoef => numericTextBox1.Value;

        private void button1_Click(object sender, System.EventArgs e)
        {

        }
    }
}
