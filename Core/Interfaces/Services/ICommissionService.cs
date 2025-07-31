using Core.DTOs.CommissionDTOs;
using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ICommissionService
    {
        Task<GeneralResponse<CommissionPlanDTO>> CreateCommissionPlanAsync(CommissionPlanCreateDTO dto);
        Task<GeneralResponse<CommissionPlanDTO>> UpdateCommissionPlanAsync(string id, CommissionPlanUpdateDTO dto);
        Task<GeneralResponse<CommissionPlanDTO>> GetCommissionPlanByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<CommissionPlanDTO>>> GetAllCommissionPlansAsync();
        Task<GeneralResponse<CommissionPlanDTO>> GetDefaultCommissionPlanAsync();
        Task<GeneralResponse<bool>> DeleteCommissionPlanAsync(string id);
        
        Task<GeneralResponse<CommissionTransactionDTO>> CreateCommissionTransactionAsync(CommissionTransactionCreateDTO dto);
        Task<GeneralResponse<CommissionTransactionDTO>> GetCommissionTransactionByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<CommissionTransactionDTO>>> GetCommissionTransactionsByTechCompanyAsync(string techCompanyId);
        Task<GeneralResponse<IEnumerable<CommissionTransactionDTO>>> GetCommissionTransactionsByDeliveryPersonAsync(string deliveryPersonId);
        Task<GeneralResponse<CommissionTransactionDTO>> UpdateCommissionStatusAsync(string id, string status);
        
        Task<GeneralResponse<CommissionSummaryDTO>> GetCommissionSummaryAsync(string techCompanyId, DateTime startDate, DateTime endDate);
        Task<GeneralResponse<decimal>> CalculateCommissionAsync(decimal amount, string serviceType, string techCompanyId);
        Task<GeneralResponse<bool>> ProcessPayoutAsync(string transactionId);
    }
} 