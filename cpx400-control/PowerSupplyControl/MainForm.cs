using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Drawing;
using System.Text;

namespace PowerSupplyControl
{
    public class MainForm : Form
    {
        private SerialPort serialPort;
        private ComboBox cmbPort, cmbBaudRate, cmbOutput;
        private NumericUpDown numVoltage, numCurrent, numOVP, numOCP;
        private NumericUpDown numDeltaV, numDeltaI, numRatio;
        private Button btnConnect, btnSetVoltage, btnSetCurrent, btnSetOVP, btnSetOCP;
        private Button btnOutputOn, btnOutputOff, btnAllOn, btnAllOff;
        private Button btnReadVoltage, btnReadCurrent, btnReadOutput;
        private Button btnIncV, btnDecV, btnIncI, btnDecI;
        private Button btnSetDelta, btnTripReset, btnSave, btnRecall;
        private CheckBox chkVerify;
        private ComboBox cmbConfig, cmbTripConfig, cmbMemory;
        private TextBox txtResponse;
        private Label lblConnectionStatus, lblOutputVoltage, lblOutputCurrent;

        public MainForm()
        {
            InitializeComponent();
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.Text = "Netzgerät Fernsteuerung";
            this.Size = new Size(900, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int leftCol = 20;
            int rightCol = 480;
            int yPos = 20;

            // ===== Verbindungseinstellungen =====
            var grpConnection = new GroupBox
            {
                Text = "Verbindung",
                Location = new Point(leftCol, yPos),
                Size = new Size(420, 120)
            };

            var lblPort = new Label { Text = "COM-Port:", Location = new Point(10, 25), AutoSize = true };
            cmbPort = new ComboBox { Location = new Point(120, 22), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPort.Items.AddRange(SerialPort.GetPortNames());
            if (cmbPort.Items.Count > 0) cmbPort.SelectedIndex = 0;

            var lblBaud = new Label { Text = "Baudrate:", Location = new Point(10, 55), AutoSize = true };
            cmbBaudRate = new ComboBox { Location = new Point(120, 52), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBaudRate.Items.AddRange(new object[] { 9600, 19200, 38400, 57600, 115200 });
            cmbBaudRate.SelectedIndex = 4;

            btnConnect = new Button { Text = "Verbinden", Location = new Point(240, 22), Size = new Size(100, 60) };
            btnConnect.Click += BtnConnect_Click;

            lblConnectionStatus = new Label
            {
                Text = "Nicht verbunden",
                Location = new Point(10, 85),
                AutoSize = true,
                ForeColor = Color.Red
            };

            grpConnection.Controls.AddRange(new Control[] { lblPort, cmbPort, lblBaud, cmbBaudRate, btnConnect, lblConnectionStatus });
            this.Controls.Add(grpConnection);

            yPos += 130;

            // ===== Ausgangsauswahl =====
            var grpOutput = new GroupBox
            {
                Text = "Ausgangsauswahl",
                Location = new Point(leftCol, yPos),
                Size = new Size(420, 60)
            };

            var lblOutput = new Label { Text = "Ausgang:", Location = new Point(10, 25), AutoSize = true };
            cmbOutput = new ComboBox { Location = new Point(120, 22), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbOutput.Items.AddRange(new object[] { "Ausgang 1 (Master)", "Ausgang 2 (Slave)" });
            cmbOutput.SelectedIndex = 0;

            grpOutput.Controls.AddRange(new Control[] { lblOutput, cmbOutput });
            this.Controls.Add(grpOutput);

            yPos += 70;

            // ===== Spannungs- und Stromeinstellungen =====
            var grpSettings = new GroupBox
            {
                Text = "Parameter einstellen",
                Location = new Point(leftCol, yPos),
                Size = new Size(420, 200)
            };

            var lblVoltage = new Label { Text = "Spannung (V):", Location = new Point(10, 25), AutoSize = true };
            numVoltage = new NumericUpDown { Location = new Point(120, 22), Width = 100, DecimalPlaces = 2, Maximum = 100, Increment = 0.1m };
            btnSetVoltage = new Button { Text = "Setzen", Location = new Point(230, 20), Size = new Size(80, 25) };
            btnSetVoltage.Click += (s, e) => SendCommand($"V{GetOutputNumber()} {numVoltage.Value}");
            chkVerify = new CheckBox { Text = "Mit Verify", Location = new Point(320, 22), AutoSize = true };

            var lblCurrent = new Label { Text = "Strom (A):", Location = new Point(10, 55), AutoSize = true };
            numCurrent = new NumericUpDown { Location = new Point(120, 52), Width = 100, DecimalPlaces = 3, Maximum = 50, Increment = 0.01m };
            btnSetCurrent = new Button { Text = "Setzen", Location = new Point(230, 50), Size = new Size(80, 25) };
            btnSetCurrent.Click += (s, e) => SendCommand($"I{GetOutputNumber()} {numCurrent.Value}");

            var lblOVP = new Label { Text = "OVP (V):", Location = new Point(10, 85), AutoSize = true };
            numOVP = new NumericUpDown { Location = new Point(120, 82), Width = 100, DecimalPlaces = 2, Maximum = 100, Increment = 0.1m };
            btnSetOVP = new Button { Text = "Setzen", Location = new Point(230, 80), Size = new Size(80, 25) };
            btnSetOVP.Click += (s, e) => SendCommand($"OVP{GetOutputNumber()} {numOVP.Value}");

            var lblOCP = new Label { Text = "OCP (A):", Location = new Point(10, 115), AutoSize = true };
            numOCP = new NumericUpDown { Location = new Point(120, 112), Width = 100, DecimalPlaces = 3, Maximum = 50, Increment = 0.01m };
            btnSetOCP = new Button { Text = "Setzen", Location = new Point(230, 110), Size = new Size(80, 25) };
            btnSetOCP.Click += (s, e) => SendCommand($"OCP{GetOutputNumber()} {numOCP.Value}");

            var lblDeltaV = new Label { Text = "Delta V:", Location = new Point(10, 145), AutoSize = true };
            numDeltaV = new NumericUpDown { Location = new Point(120, 142), Width = 100, DecimalPlaces = 3, Maximum = 10, Increment = 0.01m, Value = 0.1m };

            var lblDeltaI = new Label { Text = "Delta I:", Location = new Point(10, 170), AutoSize = true };
            numDeltaI = new NumericUpDown { Location = new Point(120, 167), Width = 100, DecimalPlaces = 3, Maximum = 10, Increment = 0.01m, Value = 0.01m };

            btnSetDelta = new Button { Text = "Delta setzen", Location = new Point(230, 140), Size = new Size(80, 55) };
            btnSetDelta.Click += BtnSetDelta_Click;

            grpSettings.Controls.AddRange(new Control[] {
                lblVoltage, numVoltage, btnSetVoltage, chkVerify,
                lblCurrent, numCurrent, btnSetCurrent,
                lblOVP, numOVP, btnSetOVP,
                lblOCP, numOCP, btnSetOCP,
                lblDeltaV, numDeltaV, lblDeltaI, numDeltaI, btnSetDelta
            });
            this.Controls.Add(grpSettings);

            yPos += 210;

            // ===== Ausgangskontrolle =====
            var grpOutputCtrl = new GroupBox
            {
                Text = "Ausgangssteuerung",
                Location = new Point(leftCol, yPos),
                Size = new Size(420, 90)
            };

            btnOutputOn = new Button { Text = "Ausgang EIN", Location = new Point(10, 25), Size = new Size(95, 50) };
            btnOutputOn.Click += (s, e) => SendCommand($"OP{GetOutputNumber()} 1");

            btnOutputOff = new Button { Text = "Ausgang AUS", Location = new Point(110, 25), Size = new Size(95, 50) };
            btnOutputOff.Click += (s, e) => SendCommand($"OP{GetOutputNumber()} 0");

            btnAllOn = new Button { Text = "ALLE EIN", Location = new Point(215, 25), Size = new Size(95, 50), BackColor = Color.LightGreen };
            btnAllOn.Click += (s, e) => SendCommand("OPALL 1");

            btnAllOff = new Button { Text = "ALLE AUS", Location = new Point(315, 25), Size = new Size(95, 50), BackColor = Color.LightCoral };
            btnAllOff.Click += (s, e) => SendCommand("OPALL 0");

            grpOutputCtrl.Controls.AddRange(new Control[] { btnOutputOn, btnOutputOff, btnAllOn, btnAllOff });
            this.Controls.Add(grpOutputCtrl);

            // ===== Rechte Spalte =====
            yPos = 20;

            // ===== Inkrement/Dekrement =====
            var grpIncDec = new GroupBox
            {
                Text = "Schrittweise Änderung",
                Location = new Point(rightCol, yPos),
                Size = new Size(380, 90)
            };

            btnIncV = new Button { Text = "V +", Location = new Point(10, 25), Size = new Size(80, 50) };
            btnIncV.Click += (s, e) => SendCommand($"INCV{GetOutputNumber()}{(chkVerify.Checked ? "V" : "")}");

            btnDecV = new Button { Text = "V -", Location = new Point(100, 25), Size = new Size(80, 50) };
            btnDecV.Click += (s, e) => SendCommand($"DECV{GetOutputNumber()}{(chkVerify.Checked ? "V" : "")}");

            btnIncI = new Button { Text = "I +", Location = new Point(200, 25), Size = new Size(80, 50) };
            btnIncI.Click += (s, e) => SendCommand($"INCI{GetOutputNumber()}");

            btnDecI = new Button { Text = "I -", Location = new Point(290, 25), Size = new Size(80, 50) };
            btnDecI.Click += (s, e) => SendCommand($"DECI{GetOutputNumber()}");

            grpIncDec.Controls.AddRange(new Control[] { btnIncV, btnDecV, btnIncI, btnDecI });
            this.Controls.Add(grpIncDec);

            yPos += 100;

            // ===== Messwerte auslesen =====
            var grpReadings = new GroupBox
            {
                Text = "Messwerte",
                Location = new Point(rightCol, yPos),
                Size = new Size(380, 150)
            };

            btnReadVoltage = new Button { Text = "V? (Einstellung)", Location = new Point(10, 25), Size = new Size(110, 30) };
            btnReadVoltage.Click += (s, e) => SendCommand($"V{GetOutputNumber()}?");

            btnReadCurrent = new Button { Text = "I? (Einstellung)", Location = new Point(10, 60), Size = new Size(110, 30) };
            btnReadCurrent.Click += (s, e) => SendCommand($"I{GetOutputNumber()}?");

            btnReadOutput = new Button { Text = "VO? / IO? (Ist)", Location = new Point(130, 25), Size = new Size(110, 65) };
            btnReadOutput.Click += BtnReadOutput_Click;

            lblOutputVoltage = new Label { Text = "Spannung: ---", Location = new Point(250, 30), Size = new Size(120, 20) };
            lblOutputCurrent = new Label { Text = "Strom: ---", Location = new Point(250, 60), Size = new Size(120, 20) };

            var btnReadStatus = new Button { Text = "Status lesen", Location = new Point(10, 100), Size = new Size(110, 35) };
            btnReadStatus.Click += (s, e) => SendCommand($"OP{GetOutputNumber()}?");

            btnTripReset = new Button { Text = "Trip Reset", Location = new Point(130, 100), Size = new Size(110, 35) };
            btnTripReset.Click += (s, e) => SendCommand("TRIPRST");

            grpReadings.Controls.AddRange(new Control[] {
                btnReadVoltage, btnReadCurrent, btnReadOutput,
                lblOutputVoltage, lblOutputCurrent, btnReadStatus, btnTripReset
            });
            this.Controls.Add(grpReadings);

            yPos += 160;

            // ===== Konfiguration =====
            var grpConfig = new GroupBox
            {
                Text = "Konfiguration",
                Location = new Point(rightCol, yPos),
                Size = new Size(380, 130)
            };

            var lblConfig = new Label { Text = "Betriebsmodus:", Location = new Point(10, 25), AutoSize = true };
            cmbConfig = new ComboBox { Location = new Point(120, 22), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbConfig.Items.AddRange(new object[] { "0 - Voltage Tracking", "2 - Unabhängig" });
            cmbConfig.SelectedIndex = 1;
            var btnSetConfig = new Button { Text = "Setzen", Location = new Point(280, 20), Size = new Size(80, 25) };
            btnSetConfig.Click += (s, e) => SendCommand($"CONFIG {cmbConfig.SelectedIndex * 2}");

            var lblRatio = new Label { Text = "Tracking Ratio (%):", Location = new Point(10, 55), AutoSize = true };
            numRatio = new NumericUpDown { Location = new Point(120, 52), Width = 100, Maximum = 100, Value = 100 };
            var btnSetRatio = new Button { Text = "Setzen", Location = new Point(280, 50), Size = new Size(80, 25) };
            btnSetRatio.Click += (s, e) => SendCommand($"RATIO {numRatio.Value}");

            var lblTripConfig = new Label { Text = "Trip Config:", Location = new Point(10, 85), AutoSize = true };
            cmbTripConfig = new ComboBox { Location = new Point(120, 82), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTripConfig.Items.AddRange(new object[] { "0 - Unabhängig", "1 - Beide abschalten" });
            cmbTripConfig.SelectedIndex = 0;
            var btnSetTripConfig = new Button { Text = "Setzen", Location = new Point(280, 80), Size = new Size(80, 25) };
            btnSetTripConfig.Click += (s, e) => SendCommand($"TRIPCONFIG {cmbTripConfig.SelectedIndex}");

            grpConfig.Controls.AddRange(new Control[] {
                lblConfig, cmbConfig, btnSetConfig,
                lblRatio, numRatio, btnSetRatio,
                lblTripConfig, cmbTripConfig, btnSetTripConfig
            });
            this.Controls.Add(grpConfig);

            yPos += 140;

            // ===== Speicher =====
            var grpMemory = new GroupBox
            {
                Text = "Speicherverwaltung",
                Location = new Point(rightCol, yPos),
                Size = new Size(380, 70)
            };

            var lblMemory = new Label { Text = "Speicherplatz:", Location = new Point(10, 25), AutoSize = true };
            cmbMemory = new ComboBox { Location = new Point(120, 22), Width = 80, DropDownStyle = ComboBoxStyle.DropDownList };
            for (int i = 0; i <= 9; i++) cmbMemory.Items.Add(i);
            cmbMemory.SelectedIndex = 0;

            btnSave = new Button { Text = "Speichern", Location = new Point(210, 20), Size = new Size(80, 30) };
            btnSave.Click += (s, e) => SendCommand($"SAV{GetOutputNumber()} {cmbMemory.SelectedItem}");

            btnRecall = new Button { Text = "Laden", Location = new Point(295, 20), Size = new Size(75, 30) };
            btnRecall.Click += (s, e) => SendCommand($"RCL{GetOutputNumber()} {cmbMemory.SelectedItem}");

            grpMemory.Controls.AddRange(new Control[] { lblMemory, cmbMemory, btnSave, btnRecall });
            this.Controls.Add(grpMemory);

            yPos += 80;

            // ===== Status-Anzeige =====
            var grpStatus = new GroupBox
            {
                Text = "Antworten / Status",
                Location = new Point(rightCol, yPos),
                Size = new Size(380, 120)
            };

            txtResponse = new TextBox
            {
                Location = new Point(10, 20),
                Size = new Size(360, 90),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };

            grpStatus.Controls.Add(txtResponse);
            this.Controls.Add(grpStatus);

            // ===== Menüleiste für zusätzliche Befehle =====
            var menuStrip = new MenuStrip();
            var toolsMenu = new ToolStripMenuItem("Tools");

            var identMenu = new ToolStripMenuItem("Identifikation (*IDN?)");
            identMenu.Click += (s, e) => SendCommand("*IDN?");

            var resetMenu = new ToolStripMenuItem("Reset (*RST)");
            resetMenu.Click += (s, e) => SendCommand("*RST");

            var clearMenu = new ToolStripMenuItem("Status löschen (*CLS)");
            clearMenu.Click += (s, e) => SendCommand("*CLS");

            var localMenu = new ToolStripMenuItem("Local Mode");
            localMenu.Click += (s, e) => SendCommand("LOCAL");

            toolsMenu.DropDownItems.AddRange(new[] { identMenu, resetMenu, clearMenu, localMenu });
            menuStrip.Items.Add(toolsMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private int GetOutputNumber()
        {
            return cmbOutput.SelectedIndex + 1;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.PortName = cmbPort.Text;
                    serialPort.BaudRate = int.Parse(cmbBaudRate.Text);
                    serialPort.DataBits = 8;
                    serialPort.Parity = Parity.None;
                    serialPort.StopBits = StopBits.One;
                    serialPort.Handshake = Handshake.XOnXOff;
                    serialPort.Encoding = Encoding.ASCII;
                    serialPort.NewLine = "\n";

                    serialPort.Open();

                    lblConnectionStatus.Text = "Verbunden";
                    lblConnectionStatus.ForeColor = Color.Green;
                    btnConnect.Text = "Trennen";

                    AppendResponse("Verbindung hergestellt.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Verbinden: {ex.Message}", "Verbindungsfehler",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                serialPort.Close();
                lblConnectionStatus.Text = "Nicht verbunden";
                lblConnectionStatus.ForeColor = Color.Red;
                btnConnect.Text = "Verbinden";
                AppendResponse("Verbindung getrennt.");
            }
        }

        private void SendCommand(string command)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("Keine Verbindung zum Gerät!", "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (command.Contains("V") && command.Contains(" ") && chkVerify.Checked &&
                    (command.StartsWith("V1") || command.StartsWith("V2")))
                {
                    command = command.Replace(" ", "V ");
                }

                serialPort.WriteLine(command);
                AppendResponse($">>> {command}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Senden: {ex.Message}", "Sendefehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSetDelta_Click(object sender, EventArgs e)
        {
            int output = GetOutputNumber();
            SendCommand($"DELTAV{output} {numDeltaV.Value}");
            SendCommand($"DELTAI{output} {numDeltaI.Value}");
        }

        private void BtnReadOutput_Click(object sender, EventArgs e)
        {
            int output = GetOutputNumber();
            SendCommand($"V{output}O?");
            SendCommand($"I{output}O?");
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string response = serialPort.ReadExisting();
                this.Invoke(new Action(() =>
                {
                    AppendResponse($"<<< {response.Trim()}");

                    // Parse Messwerte
                    if (response.Contains("V") && !response.Contains(" "))
                    {
                        string[] parts = response.Trim().Split('V');
                        if (parts.Length > 0 && decimal.TryParse(parts[0], out decimal voltage))
                        {
                            lblOutputVoltage.Text = $"Spannung: {voltage:F2} V";
                        }
                    }
                    else if (response.Contains("A"))
                    {
                        string[] parts = response.Trim().Split('A');
                        if (parts.Length > 0 && decimal.TryParse(parts[0], out decimal current))
                        {
                            lblOutputCurrent.Text = $"Strom: {current:F3} A";
                        }
                    }
                }));
            }
            catch { }
        }

        private void AppendResponse(string text)
        {
            txtResponse.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}\r\n");
        }
    }
}