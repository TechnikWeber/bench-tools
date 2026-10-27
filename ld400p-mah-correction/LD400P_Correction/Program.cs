using System;
using System.Drawing;
using System.Windows.Forms;

namespace LD400PCalc
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CalcForm());
        }
    }

    public class CalcForm : Form
    {
        private TextBox txtMah;
        private TextBox txtDeviation;
        private ComboBox cmbDirection;
        private Label lblPromptMah;
        private Label lblPromptDeviation;
        private Label lblPromptDirection;
        private Label lblResult;
        private Button btnCalculate;

        public CalcForm()
        {
            this.Text = "LD400P mAh-Korrektur";
            this.Size = new Size(500, 220);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Prompt Label für mAh
            lblPromptMah = new Label();
            lblPromptMah.Text = "Gemessene mAh eingeben:";
            lblPromptMah.Location = new Point(12, 20);
            lblPromptMah.AutoSize = true;
            this.Controls.Add(lblPromptMah);

            // TextBox für mAh
            txtMah = new TextBox();
            txtMah.Location = new Point(250, 17);
            txtMah.Size = new Size(100, 22);
            this.Controls.Add(txtMah);

            // Prompt Label für Abweichung
            lblPromptDeviation = new Label();
            lblPromptDeviation.Text = "Abweichung pro Stunde (Sek.):";
            lblPromptDeviation.Location = new Point(12, 60);
            lblPromptDeviation.AutoSize = true;
            this.Controls.Add(lblPromptDeviation);

            // TextBox für Abweichung
            txtDeviation = new TextBox();
            txtDeviation.Location = new Point(250, 57);
            txtDeviation.Size = new Size(100, 22);
            txtDeviation.Text = "23"; // Standardwert
            this.Controls.Add(txtDeviation);

            // Prompt Label für Richtung
            lblPromptDirection = new Label();
            lblPromptDirection.Text = "Richtung der Abweichung:";
            lblPromptDirection.Location = new Point(12, 100);
            lblPromptDirection.AutoSize = true;
            this.Controls.Add(lblPromptDirection);

            // ComboBox für Richtung
            cmbDirection = new ComboBox();
            cmbDirection.Location = new Point(250, 97);
            cmbDirection.Size = new Size(100, 22);
            cmbDirection.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDirection.Items.Add("Zu langsam");
            cmbDirection.Items.Add("Zu schnell");
            cmbDirection.SelectedIndex = 0; // Standard: Zu langsam
            this.Controls.Add(cmbDirection);

            // Button
            btnCalculate = new Button();
            btnCalculate.Text = "Berechnen";
            btnCalculate.Location = new Point(370, 57);
            btnCalculate.Size = new Size(90, 25);
            btnCalculate.Click += BtnCalculate_Click;
            this.Controls.Add(btnCalculate);

            // Ergebnis Label
            lblResult = new Label();
            lblResult.Location = new Point(12, 140);
            lblResult.Size = new Size(460, 50);
            lblResult.Font = new Font(lblResult.Font.FontFamily, 10, FontStyle.Bold);
            this.Controls.Add(lblResult);
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtMah.Text, out double mah))
            {
                MessageBox.Show("Bitte gültigen mAh-Wert eingeben.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(txtDeviation.Text, out double deviationSeconds))
            {
                MessageBox.Show("Bitte gültige Abweichung in Sekunden eingeben.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Richtung berücksichtigen
            double factor = 1.0;
            if (cmbDirection.SelectedItem.ToString() == "Zu langsam")
            {
                factor = 1 + (deviationSeconds / 3600.0);
            }
            else // Zu schnell
            {
                factor = 1 - (deviationSeconds / 3600.0);
            }

            double corrected = mah * factor;

            lblResult.Text = $"Korrigiert: {corrected:F2} mAh\n" +
                             $"Abweichung: {deviationSeconds} s/h ({cmbDirection.SelectedItem})";
        }
    }
}
