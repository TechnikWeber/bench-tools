using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace CPX400DP_AND_LD400D
{
    public partial class Form1 : Form
    {
        // LD400P Komponenten
        private ComboBox cmbPortsLD400P;
        private ComboBox cmbBaudRateLD400P;
        private Button btnConnectLD400P;
        private Button btnDisconnectLD400P;
        private Button btnLoadOn;
        private Button btnLoadOff;
        private Label lblPortLabelLD400P;
        private Label lblBaudLabelLD400P;
        private Label lblStatusLD400P;
        private GroupBox grpLD400P;
        private GroupBox grpControlLD400P;
        private SerialPort serialPortLD400P;
        private bool isConnectedLD400P = false;

        // CPX400DP Komponenten
        private ComboBox cmbPortsCPX400DP;
        private ComboBox cmbBaudRateCPX400DP;
        private Button btnConnectCPX400DP;
        private Button btnDisconnectCPX400DP;
        private Button btnOutputOn;
        private Button btnOutputOff;
        private Label lblPortLabelCPX400DP;
        private Label lblBaudLabelCPX400DP;
        private Label lblStatusCPX400DP;
        private GroupBox grpCPX400DP;
        private GroupBox grpControlCPX400DP;
        private SerialPort serialPortCPX400DP;
        private bool isConnectedCPX400DP = false;

        // Gemeinsame Komponenten
        private TextBox txtLog;
        private Label lblLogLabel;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadAvailablePorts();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "LD400P & CPX400DP Steuerung";
            this.Size = new Size(520, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ===== LD400P Gruppe =====
            grpLD400P = new GroupBox
            {
                Text = "LD400P (Elektronische Last)",
                Location = new Point(10, 10),
                Size = new Size(490, 150)
            };

            lblPortLabelLD400P = new Label
            {
                Text = "COM-Port:",
                Location = new Point(15, 25),
                Size = new Size(80, 20)
            };

            cmbPortsLD400P = new ComboBox
            {
                Location = new Point(100, 23),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblBaudLabelLD400P = new Label
            {
                Text = "Baudrate:",
                Location = new Point(220, 25),
                Size = new Size(80, 20)
            };

            cmbBaudRateLD400P = new ComboBox
            {
                Location = new Point(300, 23),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbBaudRateLD400P.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            cmbBaudRateLD400P.SelectedIndex = 0;

            btnConnectLD400P = new Button
            {
                Text = "Verbinden",
                Location = new Point(420, 20),
                Size = new Size(55, 30),
                BackColor = Color.LightGreen
            };
            btnConnectLD400P.Click += BtnConnectLD400P_Click;

            btnDisconnectLD400P = new Button
            {
                Text = "Trennen",
                Location = new Point(420, 20),
                Size = new Size(55, 30),
                BackColor = Color.LightCoral,
                Enabled = false,
                Visible = false
            };
            btnDisconnectLD400P.Click += BtnDisconnectLD400P_Click;

            lblStatusLD400P = new Label
            {
                Text = "Status: Nicht verbunden",
                Location = new Point(15, 55),
                Size = new Size(460, 20),
                ForeColor = Color.Red
            };

            grpControlLD400P = new GroupBox
            {
                Text = "Last Steuerung",
                Location = new Point(15, 80),
                Size = new Size(460, 60),
                Enabled = false
            };

            btnLoadOn = new Button
            {
                Text = "Last EIN",
                Location = new Point(15, 20),
                Size = new Size(200, 30),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnLoadOn.Click += BtnLoadOn_Click;

            btnLoadOff = new Button
            {
                Text = "Last AUS",
                Location = new Point(245, 20),
                Size = new Size(200, 30),
                BackColor = Color.LightCoral,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnLoadOff.Click += BtnLoadOff_Click;

            grpControlLD400P.Controls.Add(btnLoadOn);
            grpControlLD400P.Controls.Add(btnLoadOff);

            grpLD400P.Controls.Add(lblPortLabelLD400P);
            grpLD400P.Controls.Add(cmbPortsLD400P);
            grpLD400P.Controls.Add(lblBaudLabelLD400P);
            grpLD400P.Controls.Add(cmbBaudRateLD400P);
            grpLD400P.Controls.Add(btnConnectLD400P);
            grpLD400P.Controls.Add(btnDisconnectLD400P);
            grpLD400P.Controls.Add(lblStatusLD400P);
            grpLD400P.Controls.Add(grpControlLD400P);

            // ===== CPX400DP Gruppe =====
            grpCPX400DP = new GroupBox
            {
                Text = "CPX400DP (Netzteil)",
                Location = new Point(10, 170),
                Size = new Size(490, 150)
            };

            lblPortLabelCPX400DP = new Label
            {
                Text = "COM-Port:",
                Location = new Point(15, 25),
                Size = new Size(80, 20)
            };

            cmbPortsCPX400DP = new ComboBox
            {
                Location = new Point(100, 23),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblBaudLabelCPX400DP = new Label
            {
                Text = "Baudrate:",
                Location = new Point(220, 25),
                Size = new Size(80, 20)
            };

            cmbBaudRateCPX400DP = new ComboBox
            {
                Location = new Point(300, 23),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbBaudRateCPX400DP.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            cmbBaudRateCPX400DP.SelectedIndex = 0;

            btnConnectCPX400DP = new Button
            {
                Text = "Verbinden",
                Location = new Point(420, 20),
                Size = new Size(55, 30),
                BackColor = Color.LightGreen
            };
            btnConnectCPX400DP.Click += BtnConnectCPX400DP_Click;

            btnDisconnectCPX400DP = new Button
            {
                Text = "Trennen",
                Location = new Point(420, 20),
                Size = new Size(55, 30),
                BackColor = Color.LightCoral,
                Enabled = false,
                Visible = false
            };
            btnDisconnectCPX400DP.Click += BtnDisconnectCPX400DP_Click;

            lblStatusCPX400DP = new Label
            {
                Text = "Status: Nicht verbunden",
                Location = new Point(15, 55),
                Size = new Size(460, 20),
                ForeColor = Color.Red
            };

            grpControlCPX400DP = new GroupBox
            {
                Text = "Ausgang 1 Steuerung",
                Location = new Point(15, 80),
                Size = new Size(460, 60),
                Enabled = false
            };

            btnOutputOn = new Button
            {
                Text = "Ausgang 1 EIN",
                Location = new Point(15, 20),
                Size = new Size(200, 30),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnOutputOn.Click += BtnOutputOn_Click;

            btnOutputOff = new Button
            {
                Text = "Ausgang 1 AUS",
                Location = new Point(245, 20),
                Size = new Size(200, 30),
                BackColor = Color.LightCoral,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnOutputOff.Click += BtnOutputOff_Click;

            grpControlCPX400DP.Controls.Add(btnOutputOn);
            grpControlCPX400DP.Controls.Add(btnOutputOff);

            grpCPX400DP.Controls.Add(lblPortLabelCPX400DP);
            grpCPX400DP.Controls.Add(cmbPortsCPX400DP);
            grpCPX400DP.Controls.Add(lblBaudLabelCPX400DP);
            grpCPX400DP.Controls.Add(cmbBaudRateCPX400DP);
            grpCPX400DP.Controls.Add(btnConnectCPX400DP);
            grpCPX400DP.Controls.Add(btnDisconnectCPX400DP);
            grpCPX400DP.Controls.Add(lblStatusCPX400DP);
            grpCPX400DP.Controls.Add(grpControlCPX400DP);

            // ===== Log Bereich =====
            lblLogLabel = new Label
            {
                Text = "Protokoll:",
                Location = new Point(10, 330),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtLog = new TextBox
            {
                Location = new Point(10, 355),
                Size = new Size(490, 320),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Consolas", 8)
            };

            // Komponenten zum Form hinzufügen
            this.Controls.Add(grpLD400P);
            this.Controls.Add(grpCPX400DP);
            this.Controls.Add(lblLogLabel);
            this.Controls.Add(txtLog);

            // SerialPorts initialisieren
            serialPortLD400P = new SerialPort
            {
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };

            serialPortCPX400DP = new SerialPort
            {
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };

            this.FormClosing += Form1_FormClosing;
        }

        private void LoadAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();

            cmbPortsLD400P.Items.Clear();
            cmbPortsCPX400DP.Items.Clear();

            if (ports.Length > 0)
            {
                cmbPortsLD400P.Items.AddRange(ports);
                cmbPortsCPX400DP.Items.AddRange(ports);

                cmbPortsLD400P.SelectedIndex = 0;
                cmbPortsCPX400DP.SelectedIndex = ports.Length > 1 ? 1 : 0;
            }
            else
            {
                MessageBox.Show("Keine COM-Ports gefunden!", "Warnung",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ===== LD400P Ereignisse =====
        private void BtnConnectLD400P_Click(object sender, EventArgs e)
        {
            if (cmbPortsLD400P.SelectedItem == null)
            {
                MessageBox.Show("Bitte wählen Sie einen COM-Port aus!", "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                serialPortLD400P.PortName = cmbPortsLD400P.SelectedItem.ToString();
                serialPortLD400P.BaudRate = int.Parse(cmbBaudRateLD400P.SelectedItem.ToString());
                serialPortLD400P.Open();

                isConnectedLD400P = true;
                UpdateConnectionStatusLD400P(true);
                LogMessage($"[LD400P] Verbunden mit {serialPortLD400P.PortName} @ {serialPortLD400P.BaudRate} Baud");

                SendCommandLD400P("*IDN?");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"LD400P Verbindungsfehler: {ex.Message}", "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogMessage($"[LD400P] FEHLER: {ex.Message}");
            }
        }

        private void BtnDisconnectLD400P_Click(object sender, EventArgs e)
        {
            DisconnectLD400P();
        }

        private void DisconnectLD400P()
        {
            if (serialPortLD400P != null && serialPortLD400P.IsOpen)
            {
                try
                {
                    SendCommandLD400P("INP 0");
                    System.Threading.Thread.Sleep(100);
                    serialPortLD400P.Close();
                    LogMessage("[LD400P] Verbindung getrennt");
                }
                catch (Exception ex)
                {
                    LogMessage($"[LD400P] Fehler beim Trennen: {ex.Message}");
                }
            }
            isConnectedLD400P = false;
            UpdateConnectionStatusLD400P(false);
        }

        private void BtnLoadOn_Click(object sender, EventArgs e)
        {
            SendCommandLD400P("INP 1");
            LogMessage("[LD400P] Befehl gesendet: Last EIN");
        }

        private void BtnLoadOff_Click(object sender, EventArgs e)
        {
            SendCommandLD400P("INP 0");
            LogMessage("[LD400P] Befehl gesendet: Last AUS");
        }

        private void SendCommandLD400P(string command)
        {
            if (!isConnectedLD400P || !serialPortLD400P.IsOpen)
            {
                LogMessage("[LD400P] FEHLER: Keine Verbindung zum Gerät");
                return;
            }

            try
            {
                string commandWithTerminator = command + "\r\n";
                serialPortLD400P.WriteLine(commandWithTerminator.TrimEnd());
                LogMessage($"[LD400P] TX: {command}");

                System.Threading.Thread.Sleep(100);

                if (serialPortLD400P.BytesToRead > 0)
                {
                    try
                    {
                        string response = serialPortLD400P.ReadExisting();
                        if (!string.IsNullOrWhiteSpace(response))
                        {
                            LogMessage($"[LD400P] RX: {response.Trim()}");
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"[LD400P] FEHLER beim Senden: {ex.Message}");
            }
        }

        private void UpdateConnectionStatusLD400P(bool connected)
        {
            if (connected)
            {
                lblStatusLD400P.Text = $"Status: Verbunden mit {serialPortLD400P.PortName}";
                lblStatusLD400P.ForeColor = Color.Green;
                btnConnectLD400P.Visible = false;
                btnDisconnectLD400P.Visible = true;
                btnDisconnectLD400P.Enabled = true;
                grpControlLD400P.Enabled = true;
                cmbPortsLD400P.Enabled = false;
                cmbBaudRateLD400P.Enabled = false;
            }
            else
            {
                lblStatusLD400P.Text = "Status: Nicht verbunden";
                lblStatusLD400P.ForeColor = Color.Red;
                btnConnectLD400P.Visible = true;
                btnDisconnectLD400P.Visible = false;
                grpControlLD400P.Enabled = false;
                cmbPortsLD400P.Enabled = true;
                cmbBaudRateLD400P.Enabled = true;
            }
        }

        // ===== CPX400DP Ereignisse =====
        private void BtnConnectCPX400DP_Click(object sender, EventArgs e)
        {
            if (cmbPortsCPX400DP.SelectedItem == null)
            {
                MessageBox.Show("Bitte wählen Sie einen COM-Port aus!", "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                serialPortCPX400DP.PortName = cmbPortsCPX400DP.SelectedItem.ToString();
                serialPortCPX400DP.BaudRate = int.Parse(cmbBaudRateCPX400DP.SelectedItem.ToString());
                serialPortCPX400DP.Open();

                isConnectedCPX400DP = true;
                UpdateConnectionStatusCPX400DP(true);
                LogMessage($"[CPX400DP] Verbunden mit {serialPortCPX400DP.PortName} @ {serialPortCPX400DP.BaudRate} Baud");

                SendCommandCPX400DP("*IDN?");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CPX400DP Verbindungsfehler: {ex.Message}", "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogMessage($"[CPX400DP] FEHLER: {ex.Message}");
            }
        }

        private void BtnDisconnectCPX400DP_Click(object sender, EventArgs e)
        {
            DisconnectCPX400DP();
        }

        private void DisconnectCPX400DP()
        {
            if (serialPortCPX400DP != null && serialPortCPX400DP.IsOpen)
            {
                try
                {
                    SendCommandCPX400DP("OP1 0");
                    System.Threading.Thread.Sleep(100);
                    serialPortCPX400DP.Close();
                    LogMessage("[CPX400DP] Verbindung getrennt");
                }
                catch (Exception ex)
                {
                    LogMessage($"[CPX400DP] Fehler beim Trennen: {ex.Message}");
                }
            }
            isConnectedCPX400DP = false;
            UpdateConnectionStatusCPX400DP(false);
        }

        private void BtnOutputOn_Click(object sender, EventArgs e)
        {
            SendCommandCPX400DP("OP1 1");
            LogMessage("[CPX400DP] Befehl gesendet: Ausgang 1 EIN");
        }

        private void BtnOutputOff_Click(object sender, EventArgs e)
        {
            SendCommandCPX400DP("OP1 0");
            LogMessage("[CPX400DP] Befehl gesendet: Ausgang 1 AUS");
        }

        private void SendCommandCPX400DP(string command)
        {
            if (!isConnectedCPX400DP || !serialPortCPX400DP.IsOpen)
            {
                LogMessage("[CPX400DP] FEHLER: Keine Verbindung zum Gerät");
                return;
            }

            try
            {
                string commandWithTerminator = command + "\r\n";
                serialPortCPX400DP.WriteLine(commandWithTerminator.TrimEnd());
                LogMessage($"[CPX400DP] TX: {command}");

                System.Threading.Thread.Sleep(100);

                if (serialPortCPX400DP.BytesToRead > 0)
                {
                    try
                    {
                        string response = serialPortCPX400DP.ReadExisting();
                        if (!string.IsNullOrWhiteSpace(response))
                        {
                            LogMessage($"[CPX400DP] RX: {response.Trim()}");
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"[CPX400DP] FEHLER beim Senden: {ex.Message}");
            }
        }

        private void UpdateConnectionStatusCPX400DP(bool connected)
        {
            if (connected)
            {
                lblStatusCPX400DP.Text = $"Status: Verbunden mit {serialPortCPX400DP.PortName}";
                lblStatusCPX400DP.ForeColor = Color.Green;
                btnConnectCPX400DP.Visible = false;
                btnDisconnectCPX400DP.Visible = true;
                btnDisconnectCPX400DP.Enabled = true;
                grpControlCPX400DP.Enabled = true;
                cmbPortsCPX400DP.Enabled = false;
                cmbBaudRateCPX400DP.Enabled = false;
            }
            else
            {
                lblStatusCPX400DP.Text = "Status: Nicht verbunden";
                lblStatusCPX400DP.ForeColor = Color.Red;
                btnConnectCPX400DP.Visible = true;
                btnDisconnectCPX400DP.Visible = false;
                grpControlCPX400DP.Enabled = false;
                cmbPortsCPX400DP.Enabled = true;
                cmbBaudRateCPX400DP.Enabled = true;
            }
        }

        // ===== Gemeinsame Funktionen =====
        private void LogMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            txtLog.AppendText($"[{timestamp}] {message}\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectLD400P();
            DisconnectCPX400DP();
        }
    }
}