using System.Globalization;

namespace KalkulatorApp
{
    public partial class MainForm : Form
    {
        private double _operand1 = 0;
        private double _operand2 = 0;
        private string _operator = string.Empty;
        private bool _isNewEntry = true;
        private bool _hasError = false;

        public MainForm()
        {
            InitializeComponent();
            ResetCalculator();
        }

        private void ResetCalculator()
        {
            _operand1 = 0;
            _operand2 = 0;
            _operator = string.Empty;
            _isNewEntry = true;
            _hasError = false;
            txtDisplay.Text = "0";
            lblExpression.Text = string.Empty;
        }

        private void NumberButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;

            if (_hasError || _isNewEntry)
            {
                txtDisplay.Text = (btn.Text == ".") ? "0." : btn.Text;
                _isNewEntry = false;
                _hasError = false;
            }
            else
            {
                if (txtDisplay.Text.Length >= 16) return;

                if (txtDisplay.Text == "0" && btn.Text != ".")
                {
                    txtDisplay.Text = btn.Text;
                }
                else
                {
                    txtDisplay.Text += btn.Text;
                }
            }
        }

        private void DecimalButton_Click(object? sender, EventArgs e)
        {
            if (_hasError || _isNewEntry)
            {
                txtDisplay.Text = "0.";
                _isNewEntry = false;
                _hasError = false;
                return;
            }

            if (!txtDisplay.Text.Contains('.'))
            {
                txtDisplay.Text += ".";
            }
        }

        private void OperatorButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (_hasError) return;

            string op = btn.Tag?.ToString() ?? btn.Text;

            try
            {
                double currentVal = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);

                if (!string.IsNullOrEmpty(_operator) && !_isNewEntry)
                {
                    // Hitung akumulasi operasi sebelumnya
                    _operand2 = currentVal;
                    _operand1 = ExecuteCalculation(_operand1, _operand2, _operator);
                    txtDisplay.Text = FormatNumber(_operand1);
                }
                else
                {
                    _operand1 = currentVal;
                }

                _operator = op;
                lblExpression.Text = $"{FormatNumber(_operand1)} {GetOperatorSymbol(op)}";
                _isNewEntry = true;
            }
            catch (DivideByZeroException ex)
            {
                HandleError(ex.Message);
            }
            catch (OverflowException ex)
            {
                HandleError(ex.Message);
            }
            catch (Exception ex)
            {
                HandleError($"Input tidak valid: {ex.Message}");
            }
        }

        private void EqualsButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_operator) || _hasError) return;

            try
            {
                _operand2 = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                lblExpression.Text = $"{FormatNumber(_operand1)} {GetOperatorSymbol(_operator)} {FormatNumber(_operand2)} =";

                double result = ExecuteCalculation(_operand1, _operand2, _operator);
                txtDisplay.Text = FormatNumber(result);
                _operand1 = result;
                _operator = string.Empty;
                _isNewEntry = true;
            }
            catch (DivideByZeroException ex)
            {
                HandleError(ex.Message);
            }
            catch (OverflowException ex)
            {
                HandleError(ex.Message);
            }
            catch (Exception ex)
            {
                HandleError($"Gagal melakukan perhitungan: {ex.Message}");
            }
        }

        public static double ExecuteCalculation(double a, double b, string op)
        {
            double result = op switch
            {
                "+" => a + b,
                "-" => a - b,
                "*" => a * b,
                "/" => b == 0
                    ? throw new DivideByZeroException("Kesalahan Aritmatika: Pembagian dengan angka nol tidak terdefinisi!")
                    : a / b,
                "%" => b == 0
                    ? throw new DivideByZeroException("Kesalahan Aritmatika: Modulo dengan angka nol tidak terdefinisi!")
                    : a % b,
                _ => b
            };

            if (double.IsInfinity(result) || double.IsNaN(result))
            {
                throw new OverflowException("Hasil perhitungan melebihi kapasitas batas angka yang didukung (Overflow).");
            }

            return result;
        }

        private void HandleError(string message)
        {
            _hasError = true;
            txtDisplay.Text = "Error";
            MessageBox.Show(
                message,
                "Peringatan Perhitungan",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void ClearAll_Click(object? sender, EventArgs e)
        {
            ResetCalculator();
        }

        private void ClearEntry_Click(object? sender, EventArgs e)
        {
            if (_hasError)
            {
                ResetCalculator();
                return;
            }
            txtDisplay.Text = "0";
            _isNewEntry = true;
        }

        private void Backspace_Click(object? sender, EventArgs e)
        {
            if (_hasError || _isNewEntry) return;

            if (txtDisplay.Text.Length > 1)
            {
                txtDisplay.Text = txtDisplay.Text[..^1];
                if (txtDisplay.Text == "-") txtDisplay.Text = "0";
            }
            else
            {
                txtDisplay.Text = "0";
                _isNewEntry = true;
            }
        }

        private void ToggleSign_Click(object? sender, EventArgs e)
        {
            if (_hasError) return;

            if (double.TryParse(txtDisplay.Text, CultureInfo.InvariantCulture, out double val))
            {
                if (val == 0) return;
                val = -val;
                txtDisplay.Text = FormatNumber(val);
            }
        }

        public static string FormatNumber(double val)
        {
            if (double.IsInfinity(val) || double.IsNaN(val)) return "Error";
            return val.ToString("G15", CultureInfo.InvariantCulture);
        }

        private static string GetOperatorSymbol(string op)
        {
            return op switch
            {
                "*" => "×",
                "/" => "÷",
                _ => op
            };
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool hasShift = (keyData & Keys.Shift) == Keys.Shift;
            Keys keyCode = keyData & Keys.KeyCode;

            if (hasShift)
            {
                switch (keyCode)
                {
                    case Keys.Oemplus: // Shift + = adalah '+'
                        btnAdd.PerformClick();
                        return true;
                    case Keys.D8: // Shift + 8 adalah '*'
                        btnMul.PerformClick();
                        return true;
                }
            }
            else
            {
                switch (keyCode)
                {
                    case Keys.D0 or Keys.NumPad0:
                        btn0.PerformClick();
                        return true;
                    case Keys.D1 or Keys.NumPad1:
                        btn1.PerformClick();
                        return true;
                    case Keys.D2 or Keys.NumPad2:
                        btn2.PerformClick();
                        return true;
                    case Keys.D3 or Keys.NumPad3:
                        btn3.PerformClick();
                        return true;
                    case Keys.D4 or Keys.NumPad4:
                        btn4.PerformClick();
                        return true;
                    case Keys.D5 or Keys.NumPad5:
                        btn5.PerformClick();
                        return true;
                    case Keys.D6 or Keys.NumPad6:
                        btn6.PerformClick();
                        return true;
                    case Keys.D7 or Keys.NumPad7:
                        btn7.PerformClick();
                        return true;
                    case Keys.D8 or Keys.NumPad8:
                        btn8.PerformClick();
                        return true;
                    case Keys.D9 or Keys.NumPad9:
                        btn9.PerformClick();
                        return true;
                    case Keys.Add:
                        btnAdd.PerformClick();
                        return true;
                    case Keys.OemMinus or Keys.Subtract:
                        btnSub.PerformClick();
                        return true;
                    case Keys.Multiply:
                        btnMul.PerformClick();
                        return true;
                    case Keys.Divide or Keys.OemQuestion:
                        btnDiv.PerformClick();
                        return true;
                    case Keys.Enter or Keys.Oemplus: // Enter atau '=' fisik
                        btnEquals.PerformClick();
                        return true;
                    case Keys.Escape:
                        btnClear.PerformClick();
                        return true;
                    case Keys.Back:
                        btnBackspace.PerformClick();
                        return true;
                    case Keys.Decimal or Keys.OemPeriod or Keys.Oemcomma:
                        btnDot.PerformClick();
                        return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
