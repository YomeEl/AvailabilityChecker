using System.Net;
using System.Net.Mail;
using System.Collections.Generic;

namespace AvailabilityChecker.Email
{
    public record Reciever(string Name, string Address);

    public static class MailSender
    {
        private const string SENDER_NAME = "EasyPoll";
        private const string SENDER_ADDRESS = "noreply.easypoll@gmail.com";
        private const string SENDER_PASSWORD = "pwd123123";

        private const string SMTP_HOST = "smtp.gmail.com";
        private const int SMTP_PORT = 587;

        public static void SendEmails(
            IEnumerable<Reciever> recievers,
            string subject,
            string messageText,
            IEnumerable<string> attachmentPaths)
        {
            using SmtpClient smtp = new()
            {
                Host = SMTP_HOST,
                Port = SMTP_PORT,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(SENDER_ADDRESS, SENDER_PASSWORD)
            };
            MailAddress senderEmail = new(SENDER_ADDRESS, SENDER_NAME);

            foreach (var reciever in recievers)
            {
                MailAddress recieverEmail = new(reciever.Address, reciever.Name);
                using var message = new MailMessage(senderEmail, recieverEmail)
                {
                    Subject = subject,
                    Body = messageText,
                };
                foreach (var attachment in attachmentPaths)
                {
                    Attachment file = new(attachment);
                    message.Attachments.Add(file);
                }
                smtp.Send(message);
            };
        }
    }
}
