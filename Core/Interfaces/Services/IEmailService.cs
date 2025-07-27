using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string resetLink);
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
} 