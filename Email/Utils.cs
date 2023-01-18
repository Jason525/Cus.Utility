using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Utility.Email
{
    public static class Utils
    {
        #region Method : Create Mail Message
        public static MailMessage ToMailMessage(Mail mail)
        {
            return ToMailMessage(mail.From, mail.To, mail.Subject, mail.HtmlBody, mail.TextBody, mail.CC, mail.BCC, mail.ReplyTo, mail.Attach);
        }

        public static MailMessage ToMailMessage(string mailFrom, string mailTo, string mailSubject, string htmlMailBody, string textMailBody, string mailCC, string mailBCC, string mailReplyTo, string attachFiles)
        {
            // Create the message. 
            MailMessage mailMsg = new MailMessage();

            // Assign the "from" address.
            mailMsg.From = new MailAddress(mailFrom);

            // Assign the "to" address(es).
            String[] tos = mailTo.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string addr in tos.Where(SafeConversion.IsEmail))
            {
                mailMsg.To.Add(new MailAddress(addr));
            }

            // Assign the "cc" address(es).
            if (!string.IsNullOrEmpty(mailCC))
            {
                String[] ccs = mailCC.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string addr in ccs.Where(SafeConversion.IsEmail))
                {
                    mailMsg.CC.Add(new MailAddress(addr));
                }
            }

            // Assign the "bcc" address(es).
            if (!string.IsNullOrEmpty(mailBCC))
            {
                String[] bccs = null;
                bccs = mailBCC.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string addr in bccs.Where(SafeConversion.IsEmail))
                {
                    mailMsg.Bcc.Add(new MailAddress(addr));
                }
            }

            // Assign the reply-to address.
            if (mailReplyTo != null)
            {
                mailReplyTo.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(addr => {
                    mailMsg.ReplyToList.Add(new MailAddress(addr));
                });
            }

            // Assign the subject line.
            mailMsg.Subject = mailSubject;

            // Assign the message body.
            if (htmlMailBody != null && textMailBody != null)
            {
                // Create the Plain Text part
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(textMailBody, Encoding.UTF8, "text/plain");
                // Create the Html part
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlMailBody, Encoding.UTF8, "text/html");
                mailMsg.AlternateViews.Add(plainView);
                mailMsg.AlternateViews.Add(htmlView);
            }
            else if (htmlMailBody != null)
            {
                mailMsg.Body = htmlMailBody;
                mailMsg.IsBodyHtml = true;
            }
            else
            {
                mailMsg.Body = textMailBody;
                // The message body type is plain text by default, so no need to set it.
            }

            // Attach files.
            if (attachFiles != null)
            {
                String[] files = null;
                files = attachFiles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String filename in files)
                {
                    // Create the file attachment for this e-mail message.
                    Attachment data = new Attachment(filename, System.Net.Mime.MediaTypeNames.Application.Octet);

                    // Add time stamp information for the file.
                    System.Net.Mime.ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(filename);
                    disposition.ModificationDate = File.GetLastWriteTime(filename);
                    disposition.ReadDate = File.GetLastAccessTime(filename);

                    // Add the file attachment to this e-mail message.
                    mailMsg.Attachments.Add(data);
                }
            }

            return mailMsg;
        }
        #endregion

        #region Method : Send Mail
        public static SendingResult SendWithCredentials(Mail mail, Smtp server)
        {
            return SendWithCredentials(ToMailMessage(mail), server);
        }

        public static SendingResult SendWithCredentials(Mail mail)
        {
            return SendWithCredentials(ToMailMessage(mail));
        }

        public static SendingResult SendWithCredentials(MailMessage mail, Smtp server)
        {
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

            var results = new SendingResult();
            var client = new SmtpClient(server.RelayServer, server.RelayPort)
            {
                EnableSsl = server.UseSSL,
                Credentials = new NetworkCredential(server.UserName ?? "", server.Password ?? "")
            };

            try
            {
                results.First = new SingleResult().BeginSend();
                client.Send(mail);
                results.First.EndSend();
            }
            catch (SmtpException ex)
            {
                results.First.EndSend(ex);

                if (ex.Message.ToLower().IndexOf("operation has timed out") >= 0)
                {
                    try
                    {
                        results.TryAgain = new SingleResult().BeginSend();
                        client.Send(mail);
                    }
                    catch (SmtpException ex2)
                    {
                        results.TryAgain.EndSend(ex2);
                    }
                }
            }
            finally
            {
                mail.Dispose();
                client.Dispose();
            }

            return results;
        }

        public static SendingResult SendWithCredentials(MailMessage mail)
        {
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

            var results = new SendingResult();
            var client = new SmtpClient();

            try
            {
                results.First = new SingleResult().BeginSend();
				mail.BodyEncoding = System.Text.Encoding.UTF8;
                client.Send(mail);
                results.First.EndSend();
            }
            catch (SmtpException ex)
            {
                results.First.EndSend(ex);

                if (ex.Message.ToLower().IndexOf("operation has timed out") >= 0)
                {
                    try
                    {
                        results.TryAgain = new SingleResult().BeginSend();
                        client.Send(mail);
                    }
                    catch (SmtpException ex2)
                    {
                        results.TryAgain.EndSend(ex2);
                    }
                }
            }
            finally
            {
                mail.Dispose();
                client.Dispose();
            }

            return results;
        }
        #endregion

        #region Method : Validates the server certificate.
        /// <summary>
        /// Validates the server certificate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        #endregion
    }
}
