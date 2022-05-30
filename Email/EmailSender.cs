using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;

using AvailabilityChecker.Logging;
using AvailabilityChecker.Email.Exceptions;

namespace AvailabilityChecker.Email
{
    public class EmailSender
    {
        private readonly Sender _sender;
        private readonly MailAddress _senderAddress;
        private readonly IEnumerable<MailAddress> _receivers;
        private readonly ILogger _logger;

        public EmailSender(Sender sender, IEnumerable<Receiver> receivers, ILogger logger = null)
        {
            _logger = logger;
            bool isSenderCorrect = MailAddress.TryCreate(sender.Address, sender.Name, out _senderAddress);
            if (!isSenderCorrect)
            {
                throw new IncorrectSenderEmailAddressFormatException(sender.Address);
            }
            _sender = sender;
            _receivers = ParseReceivers(receivers);
        }

        public void Send(string subject, string text, IEnumerable<string> attachmentPaths)
        {
            using SmtpClient smtpClient = InitClient();
            foreach (var receiver in _receivers)
            {
                using MailMessage message = InitMessage(text, subject, receiver);
                AddAttachmets(message, attachmentPaths);
                SendMessage(smtpClient, message);
            }
        }

        private IEnumerable<MailAddress> ParseReceivers(IEnumerable<Receiver> receivers)
        {
            foreach (var receiver in receivers)
            {
                bool result = MailAddress.TryCreate(receiver.Address, receiver.Name, out MailAddress address);
                if (result)
                {
                    yield return address;
                }
                else
                {
                    _logger?.Log($"Incorrect receiver adress: {receiver}");
                }
            }
        }

        private SmtpClient InitClient()
        {
            SmtpClient smtpClient = new()
            {
                Host = _sender.Host,
                Port = _sender.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_sender.Address, _sender.Password)
            };
            return smtpClient;
        }

        private MailMessage InitMessage(string messageText, string subject, MailAddress receiverAddress)
        {
            MailMessage message = new(_senderAddress, receiverAddress)
            {
                Subject = subject,
                Body = messageText,
            };
            return message;
        }

        private static void AddAttachmets(MailMessage message, IEnumerable<string> attachmentPaths)
        {
            foreach (var attachment in attachmentPaths)
            {
                Attachment file = new(attachment);
                message.Attachments.Add(file);
            }
        }

        private void SendMessage(SmtpClient client, MailMessage message)
        {
            try
            {
                client.Send(message);
            }
            catch (Exception e)
            {
                _logger?.Log(e.Message);
            }
        }
    }
}
