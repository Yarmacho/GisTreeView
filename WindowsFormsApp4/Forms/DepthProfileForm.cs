using System.Windows.Forms;

namespace WindowsFormsApp4.Forms
{
    public partial class DepthProfileForm : Form
    {
        public DepthProfileForm(double depthValue, double tempValue, double salinityValue, double soundSpeedValue,
            double absorbtionValue)
        {
            InitializeComponent();

            depth.ReadOnly = true;
            depth.Text = depthValue.ToString();
            temperature.Text = tempValue.ToString();
            salinity.Text = salinityValue.ToString();
            speed.Text = soundSpeedValue.ToString();
            absorbtion.Text = absorbtionValue.ToString();

            AcceptButton = button1;
            AcceptButton.DialogResult = DialogResult.OK;
        }

        public double Depth => depth.Value;
        public double Temperature => temperature.Value;
        public double Salinity => salinity.Value;
        public double SoundSpeed => speed.Value;
        public double Absorbtion => absorbtion.Value;

        private void button1_Click(object sender, System.EventArgs e)
        {

        }
    }
}
