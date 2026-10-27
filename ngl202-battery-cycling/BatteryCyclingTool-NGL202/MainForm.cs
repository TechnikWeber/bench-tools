using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormsTimer = System.Windows.Forms.Timer;

namespace BatteryCyclingTool
{
    public class MainForm : Form
    {
        // UI Controls
        private TextBox txtIpAddress = null!;
        private Button btnConnect = null!;
        private TextBox txtChargeVoltage = null!;
        private TextBox txtChargeCurrent = null!;
        private TextBox txtDischargeVoltage = null!;
        private TextBox txtDischargeCurrent = null!;
        private TextBox txtRestTime = null!;
        private TextBox txtCycleCount = null!;
        private TextBox txtChannel = null!;
        private TextBox txtNominalCapacity = null!;
        private Button btnStart = null!;
        private Button btnStop = null!;
        private Button btnExport = null!;
        private TextBox txtResults = null!;
        private Label lblStatus = null!;
        private FormsTimer statusTimer = null!;

        // Logic
        private NGL202Controller? nglController;
        private CancellationTokenSource? cts;
        private List<CycleData> cycleHistory = new List<CycleData>();
        private bool isRunning = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form Settings
            this.Text = "NGL202 Battery Cycling Tool";
            this.Size = new Size(900, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            int yPos = 20;

            // Connection Panel
            GroupBox grpConnection = new GroupBox
            {
                Text = "Verbindung",
                Location = new Point(10, yPos),
                Size = new Size(860, 60)
            };

            Label lblIp = new Label { Text = "IP-Adresse:", Location = new Point(10, 25), AutoSize = true };
            txtIpAddress = new TextBox { Location = new Point(100, 22), Size = new Size(200, 23), Text = "192.168.1.100" };
            btnConnect = new Button { Text = "Verbinden", Location = new Point(320, 20), Size = new Size(100, 27) };
            btnConnect.Click += BtnConnect_Click;

            grpConnection.Controls.AddRange(new Control[] { lblIp, txtIpAddress, btnConnect });
            this.Controls.Add(grpConnection);

            yPos += 70;

            // Parameters Panel
            GroupBox grpParams = new GroupBox
            {
                Text = "Zyklus-Parameter",
                Location = new Point(10, yPos),
                Size = new Size(860, 250)
            };

            int paramY = 25;
            AddParameter(grpParams, ref paramY, "Ladespannung (V):", out txtChargeVoltage, "4.2");
            AddParameter(grpParams, ref paramY, "Ladestrom (A):", out txtChargeCurrent, "1.0");
            AddParameter(grpParams, ref paramY, "Entladespannung (V):", out txtDischargeVoltage, "2.5");
            AddParameter(grpParams, ref paramY, "Entladestrom (A):", out txtDischargeCurrent, "1.0");
            AddParameter(grpParams, ref paramY, "Ruhephase (Sekunden):", out txtRestTime, "300");
            AddParameter(grpParams, ref paramY, "Anzahl Zyklen:", out txtCycleCount, "200");
            AddParameter(grpParams, ref paramY, "Kanal:", out txtChannel, "1");
            AddParameter(grpParams, ref paramY, "Nennkapazität (mAh):", out txtNominalCapacity, "3000");

            this.Controls.Add(grpParams);

            yPos += 260;

            // Control Panel
            GroupBox grpControl = new GroupBox
            {
                Text = "Steuerung",
                Location = new Point(10, yPos),
                Size = new Size(860, 60)
            };

            btnStart = new Button { Text = "Start", Location = new Point(300, 22), Size = new Size(100, 27), Enabled = false };
            btnStart.Click += BtnStart_Click;
            btnStop = new Button { Text = "Stop", Location = new Point(410, 22), Size = new Size(100, 27), Enabled = false };
            btnStop.Click += BtnStop_Click;
            btnExport = new Button { Text = "Export CSV", Location = new Point(520, 22), Size = new Size(100, 27) };
            btnExport.Click += BtnExport_Click;

            grpControl.Controls.AddRange(new Control[] { btnStart, btnStop, btnExport });
            this.Controls.Add(grpControl);

            yPos += 70;

            // Results Panel
            GroupBox grpResults = new GroupBox
            {
                Text = "Ergebnisse",
                Location = new Point(10, yPos),
                Size = new Size(860, 250)
            };

            txtResults = new TextBox
            {
                Location = new Point(10, 20),
                Size = new Size(840, 220),
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                ReadOnly = true,
                Font = new Font("Consolas", 9)
            };

            grpResults.Controls.Add(txtResults);
            this.Controls.Add(grpResults);

            yPos += 260;

            // Status Panel
            GroupBox grpStatus = new GroupBox
            {
                Text = "Status",
                Location = new Point(10, yPos),
                Size = new Size(860, 50)
            };

            lblStatus = new Label
            {
                Text = "Nicht verbunden",
                Location = new Point(10, 20),
                Size = new Size(840, 20),
                Font = new Font(this.Font, FontStyle.Bold)
            };

            grpStatus.Controls.Add(lblStatus);
            this.Controls.Add(grpStatus);

            // Timer for status updates
            statusTimer = new FormsTimer { Interval = 1000 };
            statusTimer.Tick += StatusTimer_Tick;
        }

        private void AddParameter(GroupBox group, ref int yPos, string label, out TextBox textBox, string defaultValue)
        {
            Label lbl = new Label { Text = label, Location = new Point(10, yPos + 3), Size = new Size(180, 20) };
            textBox = new TextBox { Location = new Point(200, yPos), Size = new Size(150, 23), Text = defaultValue };
            group.Controls.AddRange(new Control[] { lbl, textBox });
            yPos += 30;
        }

        private async void BtnConnect_Click(object? sender, EventArgs e)
        {
            if (nglController == null || !nglController.IsConnected)
            {
                try
                {
                    btnConnect.Enabled = false;
                    UpdateStatus("Verbinde...");

                    nglController = new NGL202Controller(txtIpAddress.Text);
                    await Task.Run(() => nglController.Connect());

                    string idn = nglController.GetIdentity();
                    UpdateStatus($"Verbunden: {idn}");
                    btnConnect.Text = "Trennen";
                    btnStart.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Verbindungsfehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus("Verbindung fehlgeschlagen");
                }
                finally
                {
                    btnConnect.Enabled = true;
                }
            }
            else
            {
                nglController.Disconnect();
                UpdateStatus("Getrennt");
                btnConnect.Text = "Verbinden";
                btnStart.Enabled = false;
            }
        }

        private async void BtnStart_Click(object? sender, EventArgs e)
        {
            if (isRunning) return;

            try
            {
                CycleParameters param = new CycleParameters
                {
                    ChargeVoltage = double.Parse(txtChargeVoltage.Text),
                    ChargeCurrent = double.Parse(txtChargeCurrent.Text),
                    DischargeVoltage = double.Parse(txtDischargeVoltage.Text),
                    DischargeCurrent = double.Parse(txtDischargeCurrent.Text),
                    RestTime = int.Parse(txtRestTime.Text),
                    CycleCount = int.Parse(txtCycleCount.Text),
                    Channel = int.Parse(txtChannel.Text),
                    NominalCapacity = double.Parse(txtNominalCapacity.Text)
                };

                // Validation
                if (param.ChargeVoltage < 0 || param.ChargeVoltage > 60)
                {
                    MessageBox.Show("Ladespannung muss zwischen 0 und 60V liegen!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                isRunning = true;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                cycleHistory.Clear();
                txtResults.Clear();

                cts = new CancellationTokenSource();
                statusTimer.Start();

                await Task.Run(() => RunCycleTest(param, cts.Token));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Start: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isRunning = false;
                statusTimer.Stop();
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            cts?.Cancel();
            UpdateStatus("Wird gestoppt...");
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "CSV Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*",
                    FileName = $"BatteryCycles_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Zyklus,Ladekapazität (mAh),Entladekapazität (mAh),Kapazitätserhalt (%),SOH (%),Datum");

                    foreach (var cycle in cycleHistory)
                    {
                        sb.AppendLine($"{cycle.CycleNumber},{cycle.ChargeCapacity:F2},{cycle.DischargeCapacity:F2},{cycle.CapacityRetention:F2},{cycle.SOH:F2},{cycle.Timestamp:yyyy-MM-dd HH:mm:ss}");
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show($"Daten erfolgreich exportiert!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export-Fehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunCycleTest(CycleParameters param, CancellationToken token)
        {
            try
            {
                if (nglController == null) return;

                nglController.SetChannel(param.Channel);
                nglController.ConfigureOutput(param.ChargeVoltage, param.ChargeCurrent);

                for (int i = 1; i <= param.CycleCount && !token.IsCancellationRequested; i++)
                {
                    UpdateStatus($"Zyklus {i}/{param.CycleCount} - Laden...");
                    double chargeCapacity = PerformCharge(param, token);

                    if (token.IsCancellationRequested) break;

                    UpdateStatus($"Zyklus {i}/{param.CycleCount} - Ruhephase nach Laden...");
                    Thread.Sleep(param.RestTime * 1000);

                    if (token.IsCancellationRequested) break;

                    UpdateStatus($"Zyklus {i}/{param.CycleCount} - Entladen...");
                    double dischargeCapacity = PerformDischarge(param, token);

                    double capacityRetention = (dischargeCapacity / chargeCapacity) * 100;
                    double soh = (dischargeCapacity / param.NominalCapacity) * 100;

                    CycleData data = new CycleData
                    {
                        CycleNumber = i,
                        ChargeCapacity = chargeCapacity,
                        DischargeCapacity = dischargeCapacity,
                        CapacityRetention = capacityRetention,
                        SOH = soh,
                        Timestamp = DateTime.Now
                    };

                    cycleHistory.Add(data);
                    AppendResult($"Zyklus {i}: Ladung={chargeCapacity:F2}mAh, Entladung={dischargeCapacity:F2}mAh, Erhalt={capacityRetention:F1}%, SOH={soh:F1}%");

                    if (token.IsCancellationRequested) break;

                    UpdateStatus($"Zyklus {i}/{param.CycleCount} - Ruhephase nach Entladen...");
                    Thread.Sleep(param.RestTime * 1000);
                }

                nglController.SetOutput(false);

                if (!token.IsCancellationRequested)
                {
                    GenerateSummary(param);
                    UpdateStatus("Test abgeschlossen");
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Batteriezyklus-Test erfolgreich abgeschlossen!", "Fertig", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    });
                }
                else
                {
                    UpdateStatus("Test abgebrochen");
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Testfehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        private double PerformCharge(CycleParameters param, CancellationToken token)
        {
            if (nglController == null) return 0;

            nglController.ConfigureOutput(param.ChargeVoltage, param.ChargeCurrent);
            nglController.SetOutput(true);

            double totalCharge = 0;
            DateTime startTime = DateTime.Now;

            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                var meas = nglController.Measure();

                double current = meas.Current;
                totalCharge += (current * 1000) / 3600.0; // mAh

                // Ladeende-Kriterium: Strom < 50mA oder Spannung erreicht
                if (current < 0.05 || meas.Voltage >= param.ChargeVoltage - 0.01)
                {
                    break;
                }

                // Timeout nach 5 Stunden
                if ((DateTime.Now - startTime).TotalHours > 5)
                {
                    AppendResult("WARNUNG: Lade-Timeout nach 5 Stunden!");
                    break;
                }
            }

            nglController.SetOutput(false);
            return totalCharge;
        }

        private double PerformDischarge(CycleParameters param, CancellationToken token)
        {
            if (nglController == null) return 0;

            // HINWEIS: NGL202 ist ein Netzgerät, keine elektronische Last
            // Entladung wird durch niedrige Sollspannung simuliert
            // Für präzise Entladung wird eine separate E-Last empfohlen

            nglController.ConfigureOutput(param.DischargeVoltage, param.DischargeCurrent);
            nglController.SetOutput(true);

            double totalDischarge = 0;
            DateTime startTime = DateTime.Now;

            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                var meas = nglController.Measure();

                double current = meas.Current;
                totalDischarge += (current * 1000) / 3600.0; // mAh

                // Entladeende: Spannung erreicht Minimum
                if (meas.Voltage <= param.DischargeVoltage + 0.1)
                {
                    break;
                }

                // Timeout nach 5 Stunden
                if ((DateTime.Now - startTime).TotalHours > 5)
                {
                    AppendResult("WARNUNG: Entlade-Timeout nach 5 Stunden!");
                    break;
                }
            }

            nglController.SetOutput(false);
            return totalDischarge;
        }

        private void GenerateSummary(CycleParameters param)
        {
            if (cycleHistory.Count == 0) return;

            double initialCapacity = cycleHistory[0].DischargeCapacity;
            double finalCapacity = cycleHistory[cycleHistory.Count - 1].DischargeCapacity;
            double degradation = ((initialCapacity - finalCapacity) / initialCapacity) * 100;
            double avgSOH = cycleHistory.Average(c => c.SOH);

            StringBuilder summary = new StringBuilder();
            summary.AppendLine("");
            summary.AppendLine("========================================");
            summary.AppendLine("           ZUSAMMENFASSUNG              ");
            summary.AppendLine("========================================");
            summary.AppendLine($"Anzahl Zyklen:        {cycleHistory.Count}");
            summary.AppendLine($"Anfangskapazität:     {initialCapacity:F2} mAh");
            summary.AppendLine($"Endkapazität:         {finalCapacity:F2} mAh");
            summary.AppendLine($"Kapazitätsverlust:    {degradation:F2}%");
            summary.AppendLine($"Finaler SOH:          {cycleHistory[cycleHistory.Count - 1].SOH:F2}%");
            summary.AppendLine($"Durchschn. SOH:       {avgSOH:F2}%");
            summary.AppendLine("========================================");

            AppendResult(summary.ToString());
        }

        private void StatusTimer_Tick(object? sender, EventArgs e)
        {
            if (nglController != null && nglController.IsConnected && isRunning)
            {
                try
                {
                    var meas = nglController.Measure();
                    UpdateStatus($"U={meas.Voltage:F3}V, I={meas.Current:F3}A, P={meas.Power:F3}W");
                }
                catch { }
            }
        }

        private void UpdateStatus(string message)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke((MethodInvoker)delegate { lblStatus.Text = message; });
            }
            else
            {
                lblStatus.Text = message;
            }
        }

        private void AppendResult(string message)
        {
            if (txtResults.InvokeRequired)
            {
                txtResults.Invoke((MethodInvoker)delegate
                {
                    txtResults.AppendText(message + Environment.NewLine);
                    txtResults.SelectionStart = txtResults.Text.Length;
                    txtResults.ScrollToCaret();
                });
            }
            else
            {
                txtResults.AppendText(message + Environment.NewLine);
                txtResults.SelectionStart = txtResults.Text.Length;
                txtResults.ScrollToCaret();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                statusTimer?.Stop();
                statusTimer?.Dispose();
                nglController?.Disconnect();
                cts?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // NGL202 Controller Class
    public class NGL202Controller
    {
        private TcpClient? client;
        private NetworkStream? stream;
        private string ipAddress;
        private int port = 5025; // Standard SCPI Port
        private int currentChannel = 1;

        public bool IsConnected => client?.Connected ?? false;

        public NGL202Controller(string ip, int port = 5025)
        {
            this.ipAddress = ip;
            this.port = port;
        }

        public void Connect()
        {
            client = new TcpClient();
            client.Connect(ipAddress, port);
            stream = client.GetStream();
            Thread.Sleep(500);
        }

        public void Disconnect()
        {
            try
            {
                SendCommand("OUTPut OFF");
                stream?.Close();
                client?.Close();
            }
            catch { }
        }

        public string GetIdentity()
        {
            return Query("*IDN?");
        }

        public void SetChannel(int channel)
        {
            currentChannel = channel;
            SendCommand($"INSTrument:NSELect {channel}");
        }

        public void ConfigureOutput(double voltage, double current)
        {
            SendCommand($"VOLTage {voltage:F3}");
            SendCommand($"CURRent {current:F3}");
        }

        public void SetOutput(bool on)
        {
            SendCommand(on ? "OUTPut ON" : "OUTPut OFF");
        }

        public (double Voltage, double Current, double Power) Measure()
        {
            double voltage = double.Parse(Query("MEASure:VOLTage?").Trim());
            double current = double.Parse(Query("MEASure:CURRent?").Trim());
            double power = voltage * current;
            return (voltage, current, power);
        }

        private void SendCommand(string command)
        {
            if (stream == null) return;
            byte[] data = Encoding.ASCII.GetBytes(command + "\n");
            stream.Write(data, 0, data.Length);
            Thread.Sleep(50);
        }

        private string Query(string query)
        {
            if (stream == null) return "";
            SendCommand(query);
            Thread.Sleep(100);

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
    }

    // Data Classes
    public class CycleParameters
    {
        public double ChargeVoltage { get; set; }
        public double ChargeCurrent { get; set; }
        public double DischargeVoltage { get; set; }
        public double DischargeCurrent { get; set; }
        public int RestTime { get; set; }
        public int CycleCount { get; set; }
        public int Channel { get; set; }
        public double NominalCapacity { get; set; }
    }

    public class CycleData
    {
        public int CycleNumber { get; set; }
        public double ChargeCapacity { get; set; }
        public double DischargeCapacity { get; set; }
        public double CapacityRetention { get; set; }
        public double SOH { get; set; }
        public DateTime Timestamp { get; set; }
    }
}