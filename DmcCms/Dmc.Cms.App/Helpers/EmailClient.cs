using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class EmailClient
    {
        #region Private Fields

        private readonly string _Host;
        private readonly NetworkCredential _NetworkCredential;

        #endregion

        #region Constructors

        public EmailClient(string smtpHost, string userName, string password, string from)
            : this(smtpHost, userName, password, new MailAddress(from))
        {
            
        }

        public EmailClient(string smtpHost, string userName, string password, MailAddress from)
            : this(smtpHost, new NetworkCredential(userName, password), from)
        {

        }

        public EmailClient(string hostAddress, NetworkCredential credential, MailAddress from)
        {
            if (string.IsNullOrWhiteSpace(hostAddress))
            {
                throw new ArgumentNullException(nameof(hostAddress));
            }

            _Host = hostAddress;
            _NetworkCredential = credential ?? throw new ArgumentNullException(nameof(credential));
            From = from ?? throw new ArgumentNullException(nameof(from));
            EnableSSL = true;
        }

        public EmailClient(string hostAddress, int port, NetworkCredential credential, MailAddress from)
            : this(hostAddress, credential, from)
        {
            SmtpPort = port;
        }

        #endregion

        #region Properties

        public MailAddress From
        {
            get;
            set;
        }

        public int? SmtpPort
        {
            get;
            set;
        }

        public bool EnableSSL
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public bool Send(string to, string subject, string body, bool bodyIsHtml = true)
        {
            return Send(new string[] { to }, subject, body, bodyIsHtml);
        }

        private bool Send(string[] to, string subject, string body, bool bodyIsHtml = true)
        {
            MailMessage message = new MailMessage();

            foreach (string item in to)
            {
                message.To.Add(item);
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = bodyIsHtml;

            return Send(message);
        }

        #endregion

        #region Private Methods 

        private bool Send(MailMessage message)
        {
            message.From = From;
            SmtpClient client = CreateSmtpClient(); // it needs to explode if it's not correct.
            client.EnableSsl = true;

            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                }
            }
        }

        private SmtpClient CreateSmtpClient()
        {
            SmtpClient client = SmtpPort.HasValue
                ? new SmtpClient(_Host, SmtpPort.Value)
                : new SmtpClient(_Host);

            client.Credentials = _NetworkCredential;

            return client;
        }

        #endregion
    }
}
