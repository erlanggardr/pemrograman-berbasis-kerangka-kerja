#nullable enable
namespace KalkulatorApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer? components = null;

        private Label lblExpression = null!;
        private TextBox txtDisplay = null!;
        private Panel displayPanel = null!;
        private TableLayoutPanel buttonLayout = null!;

        private Button btnClear = null!;
        private Button btnClearEntry = null!;
        private Button btnBackspace = null!;
        private Button btnDiv = null!;

        private Button btn7 = null!;
        private Button btn8 = null!;
        private Button btn9 = null!;
        private Button btnMul = null!;

        private Button btn4 = null!;
        private Button btn5 = null!;
        private Button btn6 = null!;
        private Button btnSub = null!;

        private Button btn1 = null!;
        private Button btn2 = null!;
        private Button btn3 = null!;
        private Button btnAdd = null!;

        private Button btnPlusMinus = null!;
        private Button btn0 = null!;
        private Button btnDot = null!;
        private Button btnEquals = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblExpression = new Label();
            txtDisplay = new TextBox();
            displayPanel = new Panel();
            buttonLayout = new TableLayoutPanel();

            btnClear = new Button();
            btnClearEntry = new Button();
            btnBackspace = new Button();
            btnDiv = new Button();

            btn7 = new Button();
            btn8 = new Button();
            btn9 = new Button();
            btnMul = new Button();

            btn4 = new Button();
            btn5 = new Button();
            btn6 = new Button();
            btnSub = new Button();

            btn1 = new Button();
            btn2 = new Button();
            btn3 = new Button();
            btnAdd = new Button();

            btnPlusMinus = new Button();
            btn0 = new Button();
            btnDot = new Button();
            btnEquals = new Button();

            displayPanel.SuspendLayout();
            buttonLayout.SuspendLayout();
            SuspendLayout();

            // 
            // displayPanel
            // 
            displayPanel.BackColor = Color.White;
            displayPanel.BorderStyle = BorderStyle.FixedSingle;
            displayPanel.Controls.Add(lblExpression);
            displayPanel.Controls.Add(txtDisplay);
            displayPanel.Location = new Point(16, 16);
            displayPanel.Name = "displayPanel";
            displayPanel.Padding = new Padding(12, 10, 12, 10);
            displayPanel.Size = new Size(328, 96);
            displayPanel.TabIndex = 0;

            // 
            // lblExpression
            // 
            lblExpression.Dock = DockStyle.Top;
            lblExpression.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblExpression.ForeColor = Color.FromArgb(100, 116, 139);
            lblExpression.Location = new Point(12, 10);
            lblExpression.Name = "lblExpression";
            lblExpression.Size = new Size(302, 24);
            lblExpression.TabIndex = 0;
            lblExpression.Text = "";
            lblExpression.TextAlign = ContentAlignment.MiddleRight;

            // 
            // txtDisplay
            // 
            txtDisplay.BackColor = Color.White;
            txtDisplay.BorderStyle = BorderStyle.None;
            txtDisplay.Dock = DockStyle.Bottom;
            txtDisplay.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point);
            txtDisplay.ForeColor = Color.FromArgb(15, 23, 42);
            txtDisplay.Location = new Point(12, 43);
            txtDisplay.Name = "txtDisplay";
            txtDisplay.ReadOnly = true;
            txtDisplay.Size = new Size(302, 43);
            txtDisplay.TabIndex = 1;
            txtDisplay.Text = "0";
            txtDisplay.TextAlign = HorizontalAlignment.Right;

            // 
            // buttonLayout
            // 
            buttonLayout.ColumnCount = 4;
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            buttonLayout.RowCount = 5;
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

            buttonLayout.Controls.Add(btnClear, 0, 0);
            buttonLayout.Controls.Add(btnClearEntry, 1, 0);
            buttonLayout.Controls.Add(btnBackspace, 2, 0);
            buttonLayout.Controls.Add(btnDiv, 3, 0);

            buttonLayout.Controls.Add(btn7, 0, 1);
            buttonLayout.Controls.Add(btn8, 1, 1);
            buttonLayout.Controls.Add(btn9, 2, 1);
            buttonLayout.Controls.Add(btnMul, 3, 1);

            buttonLayout.Controls.Add(btn4, 0, 2);
            buttonLayout.Controls.Add(btn5, 1, 2);
            buttonLayout.Controls.Add(btn6, 2, 2);
            buttonLayout.Controls.Add(btnSub, 3, 2);

            buttonLayout.Controls.Add(btn1, 0, 3);
            buttonLayout.Controls.Add(btn2, 1, 3);
            buttonLayout.Controls.Add(btn3, 2, 3);
            buttonLayout.Controls.Add(btnAdd, 3, 3);

            buttonLayout.Controls.Add(btnPlusMinus, 0, 4);
            buttonLayout.Controls.Add(btn0, 1, 4);
            buttonLayout.Controls.Add(btnDot, 2, 4);
            buttonLayout.Controls.Add(btnEquals, 3, 4);

            buttonLayout.Location = new Point(16, 124);
            buttonLayout.Name = "buttonLayout";
            buttonLayout.Size = new Size(328, 350);
            buttonLayout.TabIndex = 1;

            // Configure styles for Buttons
            ConfigureButton(btnClear, "C", Color.FromArgb(254, 226, 226), Color.FromArgb(220, 38, 38), ClearAll_Click);
            ConfigureButton(btnClearEntry, "CE", Color.FromArgb(241, 245, 249), Color.FromArgb(71, 85, 105), ClearEntry_Click);
            ConfigureButton(btnBackspace, "⌫", Color.FromArgb(241, 245, 249), Color.FromArgb(71, 85, 105), Backspace_Click);
            ConfigureButton(btnDiv, "÷", Color.FromArgb(224, 231, 255), Color.FromArgb(67, 56, 202), OperatorButton_Click, "/");

            ConfigureButton(btn7, "7", Color.White, Color.FromArgb(30, 41, 59), NumberButton_Click);
            ConfigureButton(btn8, "8", Color.White, Color.FromArgb(30, 41, 59), NumberButton_Click);
            ConfigureButton(btn9, "9", Color.White, Color.FromArgb(30, 41, 59), NumberButton_Click);
            ConfigureButton(btnMul, "×", Color.FromArgb(224, 231, 255), Color.FromArgb(67, 56, 202), OperatorButton_Click, "*");

            ConfigureButton(btn4, "4", Color.White, Color.FromArgb(30, 41, 59), NumberButton_Click);
            ConfigureButton(btn5, "5", Color.White, Color.FromArgb(30, 41, 59), NumberButton_Click);
            ConfigureButton(btn6, "6", Color.White, Color.FromArgb(30, 41, 59), NumberButton_Click);
            ConfigureButton(btnSub, "−", Color.FromArgb(224, 231, 255), Color.FromArgb(67, 56, 202), OperatorButton_Click, "-");

            ConfigureButton(btn1, "1", Color.White, Color.FromArgb(30, 41, 59), NumberButton_Click);
            ConfigureButton(btn2, "2", Color.White, Color.FromArgb(30, 41, 59), NumberButton_Click);
            ConfigureButton(btn3, "3", Color.White, Color.FromArgb(30, 41, 59), NumberButton_Click);
            ConfigureButton(btnAdd, "+", Color.FromArgb(224, 231, 255), Color.FromArgb(67, 56, 202), OperatorButton_Click, "+");

            ConfigureButton(btnPlusMinus, "±", Color.FromArgb(241, 245, 249), Color.FromArgb(71, 85, 105), ToggleSign_Click);
            ConfigureButton(btn0, "0", Color.White, Color.FromArgb(30, 41, 59), NumberButton_Click);
            ConfigureButton(btnDot, ".", Color.FromArgb(241, 245, 249), Color.FromArgb(30, 41, 59), DecimalButton_Click);
            ConfigureButton(btnEquals, "=", Color.FromArgb(37, 99, 235), Color.White, EqualsButton_Click);

            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(360, 492);
            Controls.Add(buttonLayout);
            Controls.Add(displayPanel);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Kalkulator Sederhana - PBKK Pertemuan 3";
            displayPanel.ResumeLayout(false);
            displayPanel.PerformLayout();
            buttonLayout.ResumeLayout(false);
            ResumeLayout(false);
        }

        private static void ConfigureButton(Button btn, string text, Color backColor, Color foreColor, EventHandler handler, string? tag = null)
        {
            btn.Dock = DockStyle.Fill;
            btn.Text = text;
            btn.Tag = tag ?? text;
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold, GraphicsUnit.Point);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Margin = new Padding(3);
            btn.Cursor = Cursors.Hand;
            btn.Click += handler;
        }
    }
}
