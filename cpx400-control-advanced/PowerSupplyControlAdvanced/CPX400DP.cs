using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Drawing;

namespace CPX400DP_Control
{
    public class MainForm : Form
    {
        private SerialPort serialPort;
        private System.Windows.Forms.Timer rampTimer;

        // UI Controls - Connection
        private GroupBox grpConnection;
        private ComboBox cmbPorts;
        private Button btnConnect;
        private Button btnDisconnect;
        private Label lblStatus;

        // UI Controls - Output 1
        private GroupBox grpOutput1;
        private NumericUpDown numVoltage1;
        private NumericUpDown numCurrent1;
        private Button btnSetOutput1;
        private Button btnOutput1On;
        private Button btnOutput1Off;
        private Label lblVoltage1Read;
        private Label lblCurrent1Read;
        private Button btnReadOutput1;

        // UI Controls - Output 2
        private GroupBox grpOutput2;
        private NumericUpDown numVoltage2;
        private NumericUpDown numCurrent2;
        private Button btnSetOutput2;
        private Button btnOutput2On;
        private Button btnOutput2Off;
        private Label lblVoltage2Read;
        private Label lblCurrent2Read;
        private Button btnReadOutput2;

        // UI Controls - Both Outputs
        private GroupBox grpBothOutputs;
        private Button btnAllOn;
        private Button btnAllOff;
        private CheckBox chkTrackingMode;
        private NumericUpDown numTrackingRatio;

        // UI Controls - Voltage Ramp
        private GroupBox grpVoltageRamp;
        private NumericUpDown numRampStart;
        private NumericUpDown numRampStep;
        private NumericUpDown numRampInterval;
        private NumericUpDown numRampTarget;
        private ComboBox cmbRampOutput;
        private Button btnStartRamp;
        private Button btnStopRamp;
        private Label lblRampStatus;
        private ProgressBar progressRamp;

        // Ramp variables
        private double currentRampVoltage;
        private double rampTarget;
        private double rampStep;
        private int rampOutput;
        private bool isRamping;

        public MainForm()
        {
            InitializeComponent();
            InitializeSerialPort();
            LoadAvailablePorts();
        }

        private void InitializeComponent()
        {
            this.Text = "CPX400DP Remote Control";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            int yPos = 10;

            // Connection Group
            grpConnection = new GroupBox();
            grpConnection.Text = "Verbindung";
            grpConnection.Location = new Point(10, yPos);
            grpConnection.Size = new Size(860, 80);

            cmbPorts = new ComboBox();
            cmbPorts.Location = new Point(10, 25);
            cmbPorts.Size = new Size(150, 25);
            cmbPorts.DropDownStyle = ComboBoxStyle.DropDownList;

            btnConnect = new Button();
            btnConnect.Text = "Verbinden";
            btnConnect.Location = new Point(170, 23);
            btnConnect.Size = new Size(100, 25);
            btnConnect.Click += BtnConnect_Click;

            btnDisconnect = new Button();
            btnDisconnect.Text = "Trennen";
            btnDisconnect.Location = new Point(280, 23);
            btnDisconnect.Size = new Size(100, 25);
            btnDisconnect.Enabled = false;
            btnDisconnect.Click += BtnDisconnect_Click;

            lblStatus = new Label();
            lblStatus.Text = "Nicht verbunden";
            lblStatus.Location = new Point(10, 55);
            lblStatus.Size = new Size(400, 20);
            lblStatus.ForeColor = Color.Red;

            grpConnection.Controls.AddRange(new Control[] { cmbPorts, btnConnect, btnDisconnect, lblStatus });
            this.Controls.Add(grpConnection);

            yPos += 90;

            // Output 1 Group
            grpOutput1 = new GroupBox();
            grpOutput1.Text = "Ausgang 1";
            grpOutput1.Location = new Point(10, yPos);
            grpOutput1.Size = new Size(420, 180);

            Label lbl = new Label();
            lbl.Text = "Spannung (V):";
            lbl.Location = new Point(10, 25);
            lbl.Size = new Size(100, 20);
            grpOutput1.Controls.Add(lbl);

            numVoltage1 = new NumericUpDown();
            numVoltage1.Location = new Point(120, 23);
            numVoltage1.Size = new Size(100, 25);
            numVoltage1.DecimalPlaces = 2;
            numVoltage1.Minimum = 0;
            numVoltage1.Maximum = 60;
            numVoltage1.Value = 12;
            grpOutput1.Controls.Add(numVoltage1);

            lbl = new Label();
            lbl.Text = "Strom (A):";
            lbl.Location = new Point(10, 55);
            lbl.Size = new Size(100, 20);
            grpOutput1.Controls.Add(lbl);

            numCurrent1 = new NumericUpDown();
            numCurrent1.Location = new Point(120, 53);
            numCurrent1.Size = new Size(100, 25);
            numCurrent1.DecimalPlaces = 2;
            numCurrent1.Minimum = 0;
            numCurrent1.Maximum = 20;
            numCurrent1.Value = 1;
            grpOutput1.Controls.Add(numCurrent1);

            btnSetOutput1 = new Button();
            btnSetOutput1.Text = "Setzen";
            btnSetOutput1.Location = new Point(230, 23);
            btnSetOutput1.Size = new Size(80, 55);
            btnSetOutput1.Click += BtnSetOutput1_Click;
            grpOutput1.Controls.Add(btnSetOutput1);

            btnOutput1On = new Button();
            btnOutput1On.Text = "EIN";
            btnOutput1On.Location = new Point(320, 23);
            btnOutput1On.Size = new Size(80, 25);
            btnOutput1On.BackColor = Color.LightGreen;
            btnOutput1On.Click += BtnOutput1On_Click;
            grpOutput1.Controls.Add(btnOutput1On);

            btnOutput1Off = new Button();
            btnOutput1Off.Text = "AUS";
            btnOutput1Off.Location = new Point(320, 53);
            btnOutput1Off.Size = new Size(80, 25);
            btnOutput1Off.BackColor = Color.LightCoral;
            btnOutput1Off.Click += BtnOutput1Off_Click;
            grpOutput1.Controls.Add(btnOutput1Off);

            lblVoltage1Read = new Label();
            lblVoltage1Read.Text = "Spannung: --- V";
            lblVoltage1Read.Location = new Point(10, 90);
            lblVoltage1Read.Size = new Size(200, 20);
            grpOutput1.Controls.Add(lblVoltage1Read);

            lblCurrent1Read = new Label();
            lblCurrent1Read.Text = "Strom: --- A";
            lblCurrent1Read.Location = new Point(10, 115);
            lblCurrent1Read.Size = new Size(200, 20);
            grpOutput1.Controls.Add(lblCurrent1Read);

            btnReadOutput1 = new Button();
            btnReadOutput1.Text = "Werte lesen";
            btnReadOutput1.Location = new Point(10, 145);
            btnReadOutput1.Size = new Size(120, 25);
            btnReadOutput1.Click += BtnReadOutput1_Click;
            grpOutput1.Controls.Add(btnReadOutput1);

            this.Controls.Add(grpOutput1);

            // Output 2 Group
            grpOutput2 = new GroupBox();
            grpOutput2.Text = "Ausgang 2";
            grpOutput2.Location = new Point(440, yPos);
            grpOutput2.Size = new Size(420, 180);

            lbl = new Label();
            lbl.Text = "Spannung (V):";
            lbl.Location = new Point(10, 25);
            lbl.Size = new Size(100, 20);
            grpOutput2.Controls.Add(lbl);

            numVoltage2 = new NumericUpDown();
            numVoltage2.Location = new Point(120, 23);
            numVoltage2.Size = new Size(100, 25);
            numVoltage2.DecimalPlaces = 2;
            numVoltage2.Minimum = 0;
            numVoltage2.Maximum = 60;
            numVoltage2.Value = 12;
            grpOutput2.Controls.Add(numVoltage2);

            lbl = new Label();
            lbl.Text = "Strom (A):";
            lbl.Location = new Point(10, 55);
            lbl.Size = new Size(100, 20);
            grpOutput2.Controls.Add(lbl);

            numCurrent2 = new NumericUpDown();
            numCurrent2.Location = new Point(120, 53);
            numCurrent2.Size = new Size(100, 25);
            numCurrent2.DecimalPlaces = 2;
            numCurrent2.Minimum = 0;
            numCurrent2.Maximum = 20;
            numCurrent2.Value = 1;
            grpOutput2.Controls.Add(numCurrent2);

            btnSetOutput2 = new Button();
            btnSetOutput2.Text = "Setzen";
            btnSetOutput2.Location = new Point(230, 23);
            btnSetOutput2.Size = new Size(80, 55);
            btnSetOutput2.Click += BtnSetOutput2_Click;
            grpOutput2.Controls.Add(btnSetOutput2);

            btnOutput2On = new Button();
            btnOutput2On.Text = "EIN";
            btnOutput2On.Location = new Point(320, 23);
            btnOutput2On.Size = new Size(80, 25);
            btnOutput2On.BackColor = Color.LightGreen;
            btnOutput2On.Click += BtnOutput2On_Click;
            grpOutput2.Controls.Add(btnOutput2On);

            btnOutput2Off = new Button();
            btnOutput2Off.Text = "AUS";
            btnOutput2Off.Location = new Point(320, 53);
            btnOutput2Off.Size = new Size(80, 25);
            btnOutput2Off.BackColor = Color.LightCoral;
            btnOutput2Off.Click += BtnOutput2Off_Click;
            grpOutput2.Controls.Add(btnOutput2Off);

            lblVoltage2Read = new Label();
            lblVoltage2Read.Text = "Spannung: --- V";
            lblVoltage2Read.Location = new Point(10, 90);
            lblVoltage2Read.Size = new Size(200, 20);
            grpOutput2.Controls.Add(lblVoltage2Read);

            lblCurrent2Read = new Label();
            lblCurrent2Read.Text = "Strom: --- A";
            lblCurrent2Read.Location = new Point(10, 115);
            lblCurrent2Read.Size = new Size(200, 20);
            grpOutput2.Controls.Add(lblCurrent2Read);

            btnReadOutput2 = new Button();
            btnReadOutput2.Text = "Werte lesen";
            btnReadOutput2.Location = new Point(10, 145);
            btnReadOutput2.Size = new Size(120, 25);
            btnReadOutput2.Click += BtnReadOutput2_Click;
            grpOutput2.Controls.Add(btnReadOutput2);

            this.Controls.Add(grpOutput2);

            yPos += 190;

            // Both Outputs Group
            grpBothOutputs = new GroupBox();
            grpBothOutputs.Text = "Beide Ausgänge";
            grpBothOutputs.Location = new Point(10, yPos);
            grpBothOutputs.Size = new Size(860, 80);

            btnAllOn = new Button();
            btnAllOn.Text = "ALLE EIN";
            btnAllOn.Location = new Point(10, 25);
            btnAllOn.Size = new Size(120, 40);
            btnAllOn.BackColor = Color.LightGreen;
            btnAllOn.Click += BtnAllOn_Click;
            grpBothOutputs.Controls.Add(btnAllOn);

            btnAllOff = new Button();
            btnAllOff.Text = "ALLE AUS";
            btnAllOff.Location = new Point(140, 25);
            btnAllOff.Size = new Size(120, 40);
            btnAllOff.BackColor = Color.LightCoral;
            btnAllOff.Click += BtnAllOff_Click;
            grpBothOutputs.Controls.Add(btnAllOff);

            chkTrackingMode = new CheckBox();
            chkTrackingMode.Text = "Tracking-Modus (Ausgang 2 folgt Ausgang 1)";
            chkTrackingMode.Location = new Point(280, 25);
            chkTrackingMode.Size = new Size(300, 25);
            chkTrackingMode.CheckedChanged += ChkTrackingMode_CheckedChanged;
            grpBothOutputs.Controls.Add(chkTrackingMode);

            lbl = new Label();
            lbl.Text = "Ratio (%):";
            lbl.Location = new Point(280, 50);
            lbl.Size = new Size(70, 20);
            grpBothOutputs.Controls.Add(lbl);

            numTrackingRatio = new NumericUpDown();
            numTrackingRatio.Location = new Point(355, 48);
            numTrackingRatio.Size = new Size(80, 25);
            numTrackingRatio.Minimum = 0;
            numTrackingRatio.Maximum = 100;
            numTrackingRatio.Value = 100;
            numTrackingRatio.Enabled = false;
            grpBothOutputs.Controls.Add(numTrackingRatio);

            this.Controls.Add(grpBothOutputs);

            yPos += 90;

            // Voltage Ramp Group
            grpVoltageRamp = new GroupBox();
            grpVoltageRamp.Text = "Spannungs-Rampe";
            grpVoltageRamp.Location = new Point(10, yPos);
            grpVoltageRamp.Size = new Size(860, 180);

            lbl = new Label();
            lbl.Text = "Ausgang:";
            lbl.Location = new Point(10, 25);
            lbl.Size = new Size(80, 20);
            grpVoltageRamp.Controls.Add(lbl);

            cmbRampOutput = new ComboBox();
            cmbRampOutput.Location = new Point(100, 23);
            cmbRampOutput.Size = new Size(100, 25);
            cmbRampOutput.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRampOutput.Items.AddRange(new object[] { "Ausgang 1", "Ausgang 2" });
            cmbRampOutput.SelectedIndex = 0;
            grpVoltageRamp.Controls.Add(cmbRampOutput);

            lbl = new Label();
            lbl.Text = "Start (V):";
            lbl.Location = new Point(220, 25);
            lbl.Size = new Size(80, 20);
            grpVoltageRamp.Controls.Add(lbl);

            numRampStart = new NumericUpDown();
            numRampStart.Location = new Point(310, 23);
            numRampStart.Size = new Size(100, 25);
            numRampStart.DecimalPlaces = 2;
            numRampStart.Minimum = 0;
            numRampStart.Maximum = 60;
            numRampStart.Value = 12;
            grpVoltageRamp.Controls.Add(numRampStart);

            lbl = new Label();
            lbl.Text = "Ziel (V):";
            lbl.Location = new Point(430, 25);
            lbl.Size = new Size(80, 20);
            grpVoltageRamp.Controls.Add(lbl);

            numRampTarget = new NumericUpDown();
            numRampTarget.Location = new Point(520, 23);
            numRampTarget.Size = new Size(100, 25);
            numRampTarget.DecimalPlaces = 2;
            numRampTarget.Minimum = 0;
            numRampTarget.Maximum = 60;
            numRampTarget.Value = 24;
            grpVoltageRamp.Controls.Add(numRampTarget);

            lbl = new Label();
            lbl.Text = "Schritt (V):";
            lbl.Location = new Point(10, 55);
            lbl.Size = new Size(80, 20);
            grpVoltageRamp.Controls.Add(lbl);

            numRampStep = new NumericUpDown();
            numRampStep.Location = new Point(100, 53);
            numRampStep.Size = new Size(100, 25);
            numRampStep.DecimalPlaces = 2;
            numRampStep.Minimum = 0.01M;
            numRampStep.Maximum = 10;
            numRampStep.Value = 0.5M;
            numRampStep.Increment = 0.1M;
            grpVoltageRamp.Controls.Add(numRampStep);

            lbl = new Label();
            lbl.Text = "Intervall (Sek):";
            lbl.Location = new Point(220, 55);
            lbl.Size = new Size(80, 20);
            grpVoltageRamp.Controls.Add(lbl);

            numRampInterval = new NumericUpDown();
            numRampInterval.Location = new Point(310, 53);
            numRampInterval.Size = new Size(100, 25);
            numRampInterval.Minimum = 1;
            numRampInterval.Maximum = 3600;
            numRampInterval.Value = 120;
            grpVoltageRamp.Controls.Add(numRampInterval);

            btnStartRamp = new Button();
            btnStartRamp.Text = "Rampe starten";
            btnStartRamp.Location = new Point(10, 90);
            btnStartRamp.Size = new Size(120, 35);
            btnStartRamp.BackColor = Color.LightGreen;
            btnStartRamp.Click += BtnStartRamp_Click;
            grpVoltageRamp.Controls.Add(btnStartRamp);

            btnStopRamp = new Button();
            btnStopRamp.Text = "Rampe stoppen";
            btnStopRamp.Location = new Point(140, 90);
            btnStopRamp.Size = new Size(120, 35);
            btnStopRamp.BackColor = Color.LightCoral;
            btnStopRamp.Enabled = false;
            btnStopRamp.Click += BtnStopRamp_Click;
            grpVoltageRamp.Controls.Add(btnStopRamp);

            lblRampStatus = new Label();
            lblRampStatus.Text = "Bereit";
            lblRampStatus.Location = new Point(10, 135);
            lblRampStatus.Size = new Size(500, 20);
            grpVoltageRamp.Controls.Add(lblRampStatus);

            progressRamp = new ProgressBar();
            progressRamp.Location = new Point(10, 155);
            progressRamp.Size = new Size(840, 20);
            grpVoltageRamp.Controls.Add(progressRamp);

            this.Controls.Add(grpVoltageRamp);
        }

        private void InitializeSerialPort()
        {
            serialPort = new SerialPort();
            serialPort.BaudRate = 9600;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 1000;

            rampTimer = new System.Windows.Forms.Timer();
            rampTimer.Tick += RampTimer_Tick;
        }

        private void LoadAvailablePorts()
        {
            cmbPorts.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            cmbPorts.Items.AddRange(ports);
            if (ports.Length > 0)
                cmbPorts.SelectedIndex = 0;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbPorts.SelectedItem == null)
                {
                    MessageBox.Show("Bitte wählen Sie einen COM-Port aus.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                serialPort.PortName = cmbPorts.SelectedItem.ToString();
                serialPort.Open();

                lblStatus.Text = "Verbunden mit " + serialPort.PortName;
                lblStatus.ForeColor = Color.Green;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                // Request interface lock
                SendCommand("IFLOCK");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Verbinden: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (isRamping)
                    StopRamp();

                // Release interface lock
                SendCommand("IFUNLOCK");

                serialPort.Close();
                lblStatus.Text = "Nicht verbunden";
                lblStatus.ForeColor = Color.Red;
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Trennen: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSetOutput1_Click(object sender, EventArgs e)
        {
            SendCommand($"V1 {numVoltage1.Value}");
            SendCommand($"I1 {numCurrent1.Value}");
            MessageBox.Show("Werte für Ausgang 1 gesetzt", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSetOutput2_Click(object sender, EventArgs e)
        {
            SendCommand($"V2 {numVoltage2.Value}");
            SendCommand($"I2 {numCurrent2.Value}");
            MessageBox.Show("Werte für Ausgang 2 gesetzt", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnOutput1On_Click(object sender, EventArgs e)
        {
            SendCommand("OP1 1");
        }

        private void BtnOutput1Off_Click(object sender, EventArgs e)
        {
            SendCommand("OP1 0");
        }

        private void BtnOutput2On_Click(object sender, EventArgs e)
        {
            SendCommand("OP2 1");
        }

        private void BtnOutput2Off_Click(object sender, EventArgs e)
        {
            SendCommand("OP2 0");
        }

        private void BtnAllOn_Click(object sender, EventArgs e)
        {
            SendCommand("OPALL 1");
        }

        private void BtnAllOff_Click(object sender, EventArgs e)
        {
            SendCommand("OPALL 0");
        }

        private void BtnReadOutput1_Click(object sender, EventArgs e)
        {
            string voltage = SendQuery("V1O?");
            string current = SendQuery("I1O?");

            lblVoltage1Read.Text = $"Spannung: {voltage}";
            lblCurrent1Read.Text = $"Strom: {current}";
        }

        private void BtnReadOutput2_Click(object sender, EventArgs e)
        {
            string voltage = SendQuery("V2O?");
            string current = SendQuery("I2O?");

            lblVoltage2Read.Text = $"Spannung: {voltage}";
            lblCurrent2Read.Text = $"Strom: {current}";
        }

        private void ChkTrackingMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTrackingMode.Checked)
            {
                SendCommand("CONFIG 0"); // Tracking mode
                SendCommand($"RATIO {numTrackingRatio.Value}");
                numTrackingRatio.Enabled = true;
            }
            else
            {
                SendCommand("CONFIG 2"); // Independent mode
                numTrackingRatio.Enabled = false;
            }
        }

        private void BtnStartRamp_Click(object sender, EventArgs e)
        {
            if (numRampStart.Value >= numRampTarget.Value)
            {
                MessageBox.Show("Die Zielspannung muss höher als die Startspannung sein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rampOutput = cmbRampOutput.SelectedIndex + 1;
            currentRampVoltage = (double)numRampStart.Value;
            rampTarget = (double)numRampTarget.Value;
            rampStep = (double)numRampStep.Value;

            // Set initial voltage
            SendCommand($"V{rampOutput} {currentRampVoltage.ToString("F2")}");

            // Calculate progress bar
            int totalSteps = (int)Math.Ceiling((rampTarget - currentRampVoltage) / rampStep);
            progressRamp.Maximum = totalSteps;
            progressRamp.Value = 0;

            // Start timer
            rampTimer.Interval = (int)(numRampInterval.Value * 1000);
            rampTimer.Start();
            isRamping = true;

            btnStartRamp.Enabled = false;
            btnStopRamp.Enabled = true;

            UpdateRampStatus();
        }

        private void BtnStopRamp_Click(object sender, EventArgs e)
        {
            StopRamp();
        }

        private void StopRamp()
        {
            rampTimer.Stop();
            isRamping = false;
            btnStartRamp.Enabled = true;
            btnStopRamp.Enabled = false;
            lblRampStatus.Text = "Rampe gestoppt";
        }

        private void RampTimer_Tick(object sender, EventArgs e)
        {
            currentRampVoltage += rampStep;

            if (currentRampVoltage >= rampTarget)
            {
                currentRampVoltage = rampTarget;
                SendCommand($"V{rampOutput} {currentRampVoltage.ToString("F2")}");
                StopRamp();
                lblRampStatus.Text = $"Rampe abgeschlossen bei {currentRampVoltage:F2}V";
                progressRamp.Value = progressRamp.Maximum;
                MessageBox.Show("Spannungsrampe abgeschlossen!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                SendCommand($"V{rampOutput} {currentRampVoltage.ToString("F2")}");
                progressRamp.Value++;
                UpdateRampStatus();
            }
        }

        private void UpdateRampStatus()
        {
            double remaining = rampTarget - currentRampVoltage;
            int stepsRemaining = (int)Math.Ceiling(remaining / rampStep);
            int timeRemaining = stepsRemaining * (int)numRampInterval.Value;

            lblRampStatus.Text = $"Aktuell: {currentRampVoltage:F2}V | Ziel: {rampTarget:F2}V | Verbleibend: {remaining:F2}V | Zeit: {timeRemaining}s";
        }

        private void SendCommand(string command)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("Keine Verbindung zum Gerät.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                serialPort.WriteLine(command);
                System.Threading.Thread.Sleep(50); // Short delay for device processing
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Senden: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string SendQuery(string query)
        {
            if (!serialPort.IsOpen)
            {
                return "Nicht verbunden";
            }

            try
            {
                serialPort.DiscardInBuffer();
                serialPort.WriteLine(query);
                System.Threading.Thread.Sleep(100);
                string response = serialPort.ReadLine().Trim();
                return response;
            }
            catch (Exception ex)
            {
                return $"Fehler: {ex.Message}";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isRamping)
            {
                rampTimer.Stop();
            }
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    SendCommand("IFUNLOCK");
                    serialPort.Close();
                }
                catch { }
            }
            if (rampTimer != null)
            {
                rampTimer.Dispose();
            }
            base.OnFormClosing(e);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}