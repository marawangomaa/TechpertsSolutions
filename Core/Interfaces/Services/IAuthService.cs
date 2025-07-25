using Core.DTOs.LoginDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.DTOs.LoginDTOs;
using TechpertsSolutions.Core.DTOs.RegisterDTOs;


namespace Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<GeneralResponse<string>> RegisterAsync(RegisterDTO dto);
        Task<GeneralResponse<LoginResultDTO>> LoginAsync(LoginDTO dto);
        Task<GeneralResponse<string>> ForgotPasswordAsync(ForgotPasswordDTO dto);
        Task<GeneralResponse<string>> ResetPasswordAsync(ResetPasswordDTO dto);
        Task<GeneralResponse<string>> DeleteAccountAsync(DeleteAccountDTO dto, string userId);
    }
}
