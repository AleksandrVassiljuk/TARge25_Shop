
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;

namespace TARge25Shop.ApplicationServices.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration _config;

        public EmailServices(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(EmailDto dto)
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(
                _config["EmailSettings:UserName"]));

            email.To.Add(MailboxAddress.Parse(dto.To));
            email.Subject = dto.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = System.Net.WebUtility.HtmlEncode(dto.Body)
                    .Replace("\r\n", "\n")
                    .Replace("\n", "<br />")
            };

            foreach (var file in dto.Attachments)
            {
                if (file.Length > 0 && file.Length < 10485760)
                {
                    using var stream = new MemoryStream();
                    file.CopyTo(stream);

                    builder.Attachments.Add(
                        file.FileName,
                        stream.ToArray());
                }
            }

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            smtp.Connect(
                _config["EmailSettings:Host"],
                587,
                SecureSocketOptions.StartTls);

            smtp.Authenticate(
                _config["EmailSettings:UserName"],
                _config["EmailSettings:Password"]);

            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public void SendEmailToken(EmailTokenDto dto, string token)
        {
            dto.Token = token;
            SendEmail(dto);
        }
    }
}
