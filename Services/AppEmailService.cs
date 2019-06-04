using MimeKit;
using NETCore.MailKit;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Services
{
    public class AppEmailService : EmailService, IAppEmailService
    {
        public AppEmailService(IMailKitProvider mailKitProvider) : base(mailKitProvider)
        {
            _mailKitProvider = mailKitProvider;
        }

        private IMailKitProvider _mailKitProvider { get; set; }

        public async Task SendAsync(MimeMessage message)
        {
            message.From.Add(new MailboxAddress(_mailKitProvider.Options.SenderEmail));
            using (var emailClient = _mailKitProvider.SmtpClient)
            {
                if (!emailClient.IsConnected)
                {
                    await emailClient.AuthenticateAsync(_mailKitProvider.Options.Account,
                    _mailKitProvider.Options.Password);
                    await emailClient.ConnectAsync(_mailKitProvider.Options.Server,
                    _mailKitProvider.Options.Port, _mailKitProvider.Options.Security);
                }
                await emailClient.SendAsync(message);
                await emailClient.DisconnectAsync(true);
            }
        }
        public async Task SendAsync(EmailRequest emailRequest)
        {
            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.To.Add(new MailboxAddress(emailRequest.ToAddress));
            mimeMessage.Subject = emailRequest.Subject;
            var builder = new BodyBuilder { HtmlBody = emailRequest.Body };
            if (emailRequest.Attachment != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await emailRequest.Attachment.CopyToAsync(memoryStream);
                    builder.Attachments.Add(emailRequest.FileName, memoryStream.ToArray());
                }


            }
            mimeMessage.Body = builder.ToMessageBody();
            await SendAsync(mimeMessage);
        }
    }
}
