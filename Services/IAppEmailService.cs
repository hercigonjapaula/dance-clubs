using MimeKit;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Services
{
    public interface IAppEmailService : IEmailService
    {
        Task SendAsync(MimeMessage message);
        Task SendAsync(EmailRequest emailRequest);
    }
}
