
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.IO;
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

        private void SendMessage(MimeMessage email)
        {
            string host = _config["EmailSettings:Host"]
                ?? throw new Exception("SMTP Host puudub");

            string username = _config["EmailSettings:UserName"]
                ?? throw new Exception("SMTP UserName puudub");

            string password = _config["EmailSettings:Password"]
                ?? throw new Exception("SMTP Password puudub");

            using var smtp = new SmtpClient();

            smtp.Timeout = 30000;

            try
            {
                Console.WriteLine("SMTP: Ühendan Gmailiga...");

                smtp.Connect(
                    host,
                    587,
                    SecureSocketOptions.StartTls);

                Console.WriteLine("SMTP: Ühendus loodud.");

                // Kasutame ainult LOGIN autentimist
                smtp.AuthenticationMechanisms.Clear();
                smtp.AuthenticationMechanisms.Add("LOGIN");

                Console.WriteLine("SMTP: Alustan autentimist...");

                smtp.Authenticate(username, password);

                Console.WriteLine("SMTP: Autentimine õnnestus.");

                smtp.Send(email);

                Console.WriteLine("SMTP: Kiri edukalt saadetud.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("========== SMTP VIGA ==========");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("===============================");

                throw;
            }
            finally
            {
                if (smtp.IsConnected)
                {
                    try
                    {
                        smtp.Disconnect(true);
                    }
                    catch (Exception)
                    {
                        // Ühendus võib olla serveri poolt suletud.
                    }
                }
            }
        }

        public void SendEmail(EmailDto dto)
        {
            var email = new MimeMessage();

            string sender = _config["EmailSettings:UserName"]
                ?? throw new Exception("Saatja puudub");

            email.From.Add(MailboxAddress.Parse(sender));
            email.To.Add(MailboxAddress.Parse(dto.To));

            email.Subject = dto.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = dto.Body
            };

            if (dto.Attachments != null)
            {
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
            }

            email.Body = builder.ToMessageBody();

            SendMessage(email);
        }

        public void SendEmailToken(EmailTokenDto dto, string token)
        {
            dto.Token = token;

            var email = new MimeMessage();

            string sender = _config["EmailSettings:UserName"]
                ?? throw new Exception("Saatja puudub");

            email.From.Add(MailboxAddress.Parse(sender));
            email.To.Add(MailboxAddress.Parse(dto.To));

            email.Subject = dto.Subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = dto.Body
            }.ToMessageBody();

            SendMessage(email);
        }
    }
}
