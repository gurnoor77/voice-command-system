using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Speech.Recognition;
using System.Windows.Forms;
using System.Configuration;

namespace VoiceCommandApp
{
    public partial class Form1 : Form
    {
        private SpeechRecognitionEngine recognizer;
        private string connectionString;
        private Label footer;
        private bool isListening = false;

        public Form1()
        {
            InitializeComponent();
            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            // Footer
            footer = new Label
            {
                Text = "© 2026 Gurnoor Singh | Built with C# · ASP.NET Core · React",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(footer);

            // DataGridView Styling
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.DodgerBlue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.DefaultCellStyle.Padding = new Padding(6);
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.RowTemplate.Height = 35;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Hover Effects
            btnStartListening.MouseEnter += (s, e) => { if (!isListening) btnStartListening.BackColor = Color.DeepSkyBlue; };
            btnStartListening.MouseLeave += (s, e) => { if (!isListening) btnStartListening.BackColor = Color.DodgerBlue; };
            btnViewHistory.MouseEnter += (s, e) => btnViewHistory.BackColor = Color.DimGray;
            btnViewHistory.MouseLeave += (s, e) => btnViewHistory.BackColor = Color.Gray;

            // Connection String
            connectionString = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;

            // Position footer correctly after form is loaded
            footer.Location = new Point(30, this.ClientSize.Height - 25);
        }

        // FIX 1: Dispose recognizer properly when form closes
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (recognizer != null)
            {
                recognizer.RecognizeAsyncStop();
                recognizer.Dispose();
                recognizer = null;
            }
            base.OnFormClosing(e);
        }

        private void btnStartListening_Click(object sender, EventArgs e)
        {
            if (!isListening)
            {
                try
                {
                    recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
                    recognizer.SetInputToDefaultAudioDevice();
                    recognizer.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", 50);

                    Choices commands = new Choices();
                    commands.Add(new string[] {
                        "hello", "exit", "clear",
                        "open notepad", "open chrome", "open calculator",
                        "open paint", "show date", "show time",
                        "lock screen", "minimize window", "maximize window"
                    });

                    Grammar grammar = new Grammar(new GrammarBuilder(commands));
                    recognizer.LoadGrammar(grammar);

                    recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
                    recognizer.RecognizeAsync(RecognizeMode.Multiple);

                    lblStatus.Text = "Status: Listening...";
                    btnStartListening.Text = "Stop Listening";
                    btnStartListening.BackColor = Color.Red;
                    isListening = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error initializing recognizer: " + ex.Message);
                }
            }
            else
            {
                StopListening();
            }
        }

        private void StopListening()
        {
            if (recognizer != null)
            {
                recognizer.RecognizeAsyncStop();
                recognizer.Dispose();
                recognizer = null;
            }
            lblStatus.Text = "Status: Stopped listening.";
            btnStartListening.Text = "Start Listening";
            btnStartListening.BackColor = Color.DodgerBlue;
            isListening = false;
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Must update UI from UI thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => Recognizer_SpeechRecognized(sender, e)));
                return;
            }

            float confidence = e.Result.Confidence;
            string cmd = e.Result.Text.ToLower();

            if (confidence >= 0.6)
            {
                // FIX 2: Show confidence score
                int confidencePct = (int)(confidence * 100);
                txtRecognized.Text = $"Recognized: {cmd}  (Confidence: {confidencePct}%)";
                lblStatus.Text = "Status: Command recognized.";
                SaveCommand(cmd);
                ExecuteCommand(cmd);
            }
            else
            {
                lblStatus.Text = "Status: Voice not clear — try again.";
            }
        }

        private void ExecuteCommand(string cmd)
        {
            switch (cmd)
            {
                case "hello":
                    MessageBox.Show("Hello! How can I help you?");
                    break;

                case "exit":
                    Application.Exit();
                    break;

                case "clear":
                    txtRecognized.Clear();
                    lblStatus.Text = "Status: Cleared.";
                    break;

                case "open notepad":
                    if (Confirm("Open Notepad?"))
                        System.Diagnostics.Process.Start("notepad.exe");
                    break;

                case "open chrome":
                    if (Confirm("Open Google Chrome?"))
                    {
                        // FIX 3: Chrome path fallback
                        try { System.Diagnostics.Process.Start("chrome.exe"); }
                        catch
                        {
                            string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                            if (System.IO.File.Exists(chromePath))
                                System.Diagnostics.Process.Start(chromePath);
                            else
                                MessageBox.Show("Chrome not found on this machine.");
                        }
                    }
                    break;

                case "open calculator":
                    if (Confirm("Open Calculator?"))
                        System.Diagnostics.Process.Start("calc.exe");
                    break;

                case "open paint":
                    if (Confirm("Open Paint?"))
                        System.Diagnostics.Process.Start("mspaint.exe");
                    break;

                case "show date":
                    MessageBox.Show("Today's date is: " + DateTime.Now.ToLongDateString());
                    break;

                case "show time":
                    MessageBox.Show("Current time: " + DateTime.Now.ToShortTimeString());
                    break;

                case "lock screen":
                    if (Confirm("Lock the screen?"))
                        System.Diagnostics.Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
                    break;

                case "minimize window":
                    this.WindowState = FormWindowState.Minimized;
                    break;

                case "maximize window":
                    this.WindowState = FormWindowState.Maximized;
                    break;
            }
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            if (!dataGridView1.Visible)
            {
                LoadHistory();
                dataGridView1.Visible = true;
                btnViewHistory.Text = "Hide History";
            }
            else
            {
                dataGridView1.Visible = false;
                btnViewHistory.Text = "View History";
            }
        }

        // FIX 4: Delete History button handler
        private void btnDeleteHistory_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete ALL command history?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Commands", con))
                            cmd.ExecuteNonQuery();
                    }
                    dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();
                    lblStatus.Text = "Status: History deleted.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting history: " + ex.Message);
                }
            }
        }

        // FIX 5: Copy to clipboard button handler
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRecognized.Text))
            {
                Clipboard.SetText(txtRecognized.Text);
                lblStatus.Text = "Status: Copied to clipboard!";
            }
        }

        private void LoadHistory()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Commands ORDER BY RecognizedAt DESC", con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                        if (dataGridView1.Columns.Count == 3)
                        {
                            dataGridView1.Columns[0].HeaderText = "Command ID";
                            dataGridView1.Columns[1].HeaderText = "Voice Input";
                            dataGridView1.Columns[2].HeaderText = "Timestamp";

                            foreach (DataGridViewColumn col in dataGridView1.Columns)
                            {
                                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                col.FillWeight = 33.33f;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }

        private async void SaveCommand(string command)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7123/");
                    // Ignore SSL certificate for local dev
                    var handler = new System.Net.Http.HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (m, c, ch, e) => true;
                    var secureClient = new System.Net.Http.HttpClient(handler);
                    secureClient.BaseAddress = new Uri("https://localhost:7123/");

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { commandText = command });
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    await secureClient.PostAsync("api/Commands", content);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("API error: " + ex.Message);
            }
        }

        private bool Confirm(string msg)
        {
            return MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}