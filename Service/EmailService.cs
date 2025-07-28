using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string resetLink)
        {
            var subject = "Password Reset Request - Techperts Solutions";
            var body = GeneratePasswordResetEmailBody(resetToken, resetLink);
            
            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                
                var smtpServer = _configuration["Email:SmtpServer"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
                var smtpUsername = _configuration["Email:Username"];
                var smtpPassword = _configuration["Email:Password"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"];

                
                if (string.IsNullOrEmpty(smtpServer))
                {
                    Console.WriteLine($"=== EMAIL WOULD BE SENT ===");
                    Console.WriteLine($"To: {to}");
                    Console.WriteLine($"Subject: {subject}");
                    Console.WriteLine($"Body: {body}");
                    Console.WriteLine($"==========================");
                    return true; 
                }

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true;

                    var message = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    message.To.Add(to);

                    await client.SendMailAsync(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }

        private string GeneratePasswordResetEmailBody(string resetToken, string resetLink)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f8f9fa; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; }}
                        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Password Reset Request</h1>
                        </div>
                        <div class='content'>
                            <p>Hello,</p>
                            <p>We received a request to reset your password for your Techperts Solutions account.</p>
                            <p>Click the button below to reset your password:</p>
                            <p style='text-align: center;'>
                                <a href='{resetLink}?token={resetToken}' class='button'>Reset Password</a>
                            </p>
                            <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                            <p style='word-break: break-all;'>{resetLink}?token={resetToken}</p>
                            <p><strong>Important:</strong></p>
                            <ul>
                                <li>This link will expire in 1 hour for security reasons.</li>
                                <li>If you didn't request this password reset, please ignore this email.</li>
                                <li>Your password will remain unchanged if you don't click the reset link.</li>
                            </ul>
                            <p>If you have any questions, please contact our support team.</p>
                        </div>
                        <div class='footer'>
                            <p>This is an automated message from Techperts Solutions. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
} 
