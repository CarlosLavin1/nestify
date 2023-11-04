using MimeKit;
using static mvcNestify.EmailServices.EmailSender;

namespace mvcNestify.EmailServices
{
    public class EmailConfiguration
    {
        public string? From { get; set; }
        public string? SmtpServer { get; set; }
        public int Port { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public interface IEmailSender
    {
        void SendEmail(EmailMessage email);
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _configuration;
        public EmailSender(EmailConfiguration emailConfiguration)
        {
            _configuration = emailConfiguration;
        }

        public void SendEmail(EmailMessage email)
        {
            var emailMessage = CreateEmailMessage(email);
            Send(emailMessage);
        }

        public class EmailMessage
        {
            public List<MailboxAddress> To { get; set; }
            public string? Subject { get; set; }
            public string? Content { get; set; }
            public EmailMessage(IEnumerable<string> to, string subject, string content)
            {
                To = new List<MailboxAddress>();
                To.AddRange(to.Select(x => new MailboxAddress(String.Empty, x)));
                Subject = subject;
                Content = content;

            }
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(String.Empty, _configuration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect(_configuration.SmtpServer, _configuration.Port);
                    //client.Authenticate(_emailConfig.UserName, _emailconfig.Password);
                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }

            }

        }
    }
}
