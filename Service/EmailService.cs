using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendPasswordResetEmailAsync(
            string email,
            string resetToken,
            string resetLink
        )
        {
            var body = GeneratePasswordResetEmailBody(resetToken, resetLink);
            return await SendEmailAsync(
                email,
                "Password Reset Request - Techperts Solutions",
                body
            );
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            var smtpServer = _configuration["Email:SmtpServer"] ?? "smtp.gmail.com";
            var smtpPort = int.TryParse(_configuration["Email:SmtpPort"], out int port)
                ? port
                : 587;
            var smtpUsername = _configuration["Email:Username"];
            var smtpPassword = _configuration["Email:Password"];
            var fromEmail = _configuration["Email:FromEmail"] ?? smtpUsername;
            var fromName = _configuration["Email:FromName"] ?? "Techperts Solutions";

            if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
                throw new InvalidOperationException(
                    "Email credentials are missing in appsettings.json"
                );

            try
            {
                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true;

                    var message = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                    };
                    message.To.Add(to);

                    await client.SendMailAsync(message).ConfigureAwait(false);
                }

                return true;
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
                if (smtpEx.InnerException != null)
                    Console.WriteLine($"Inner Exception: {smtpEx.InnerException.Message}");
                throw; // Let the controller return exact error to Swagger
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                throw; // Let the controller handle it
            }
        }

        private string GeneratePasswordResetEmailBody(string resetToken, string resetLink)
        {
            var encodedToken = HttpUtility.UrlEncode(resetToken);
            var fullLink = $"{resetLink}?token={encodedToken}";

            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 20px;'>
                    <div style='max-width: 600px; margin: auto; background: #fff; padding: 20px; border-radius: 8px;'>
                        <h2 style='color: #007bff;'>Password Reset Request</h2>
                        <p>Hello,</p>
                        <p>We received a request to reset your password for your <strong>Techperts Solutions</strong> account.</p>
                        <p>
                            <a href='{fullLink}' style='display:inline-block; padding:10px 20px;
                            background-color:#007bff; color:white; text-decoration:none;
                            border-radius:5px;'>Reset Password</a>
                        </p>
                        <p>If the button above does not work, copy and paste this link into your browser:</p>
                        <p><a href='{fullLink}'>{fullLink}</a></p>
                        <p>This link will expire in 1 hour.</p>
                    </div>
                </body>
                </html>";
        }
    }
}
