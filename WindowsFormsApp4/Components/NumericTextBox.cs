using System;
using System.ComponentModel;
using System.Windows.Forms;
using Tools;

namespace WindowsFormsApp4.Components
{
    public partial class NumericTextBox : TextBox
    {
        private bool _allowDecimals = true;
        private string _previousValue = string.Empty;
        private readonly char _decimalSeparator;

        public double Value => TypeTools.Convert<double>(this.Text);

        public NumericTextBox()
        {
            InitializeComponent();
        }

        public NumericTextBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            _decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            this.KeyPress += numericTextBox_KeyPress;
            this.TextChanged += numericTextBox_TextChanged;
        }

        [DefaultValue(true)]
        public bool AllowDecimals
        {
            get { return _allowDecimals; }
            set { _allowDecimals = value; }
        }

        private void numericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (handleKeyPress(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private bool handleKeyPress(char keyChar)
        {
            if (char.IsControl(keyChar))
            {
                return false;
            }

            if (char.IsDigit(keyChar))
            {
                return false;
            }

            if (_allowDecimals && keyChar == _decimalSeparator && !Text.Contains(_decimalSeparator.ToString()))
            {
                return false;
            }

            if (keyChar == '-' && SelectionStart == 0 && !Text.Contains("-"))
            {
                return false;
            }

            return true;
        }

        private void numericTextBox_TextChanged(object sender, EventArgs e)
        {
            if (validateNumericString(this.Text))
            {
                _previousValue = this.Text;
            }
            else
            {
                int selectionStart = this.SelectionStart - 1;
                this.Text = _previousValue;
                if (selectionStart >= 0)
                {
                    this.SelectionStart = selectionStart;
                }
            }
        }

        private bool validateNumericString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            if (text == "-")
            {
                return true;
            }

            if (_allowDecimals)
            {
                return decimal.TryParse(text, out _);
            }

            return int.TryParse(text, out _);
        }
    }
}
