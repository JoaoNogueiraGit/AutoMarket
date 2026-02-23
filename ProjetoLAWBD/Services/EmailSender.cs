using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration; // Necessário para IConfiguration
using System.Net;
using System.Net.Mail;

namespace ProjetoLAWBD.Services {
    public class EmailSender : IEmailSender {
        private readonly IConfiguration _configuration;

        // Injetamos IConfiguration para ler o appsettings.json diretamente
        public EmailSender(IConfiguration configuration) {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage) {
            // 1. Ler configurações
            string mailServer = _configuration["EmailSettings:MailServer"];
            int port = int.Parse(_configuration["EmailSettings:MailPort"]);
            string senderEmail = _configuration["EmailSettings:Sender"];
            string password = _configuration["EmailSettings:Password"];

            // 2. Configurar Cliente SMTP
            using (var client = new SmtpClient(mailServer, port)) {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, password);

                // 3. Criar Mensagem
                using (var mailMessage = new MailMessage()) {
                    mailMessage.From = new MailAddress(senderEmail, "AutoMarket");
                    mailMessage.To.Add(email);
                    mailMessage.Subject = subject;
                    mailMessage.Body = htmlMessage;
                    mailMessage.IsBodyHtml = true;

                    try {
                        await client.SendMailAsync(mailMessage);
                    } catch (Exception ex) {
                        
                        throw new InvalidOperationException($"Erro ao enviar email: {ex.Message}");
                    }
                }
            }
        }
    }
}