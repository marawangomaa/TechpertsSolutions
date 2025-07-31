using Core.DTOs.CommissionDTOs;
using Core.DTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using TechpertsSolutions.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class CommissionService : ICommissionService
    {
        private readonly IRepository<CommissionPlan> _commissionPlanRepo;
        private readonly IRepository<CommissionTransaction> _commissionTransactionRepo;
        private readonly IRepository<TechCompany> _techCompanyRepo;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Maintenance> _maintenanceRepo;
        private readonly IRepository<PCAssembly> _pcAssemblyRepo;
        private readonly IRepository<Delivery> _deliveryRepo;

        public CommissionService(
            IRepository<CommissionPlan> commissionPlanRepo,
            IRepository<CommissionTransaction> commissionTransactionRepo,
            IRepository<TechCompany> techCompanyRepo,
            IRepository<Order> orderRepo,
            IRepository<Maintenance> maintenanceRepo,
            IRepository<PCAssembly> pcAssemblyRepo,
            IRepository<Delivery> deliveryRepo)
        {
            _commissionPlanRepo = commissionPlanRepo;
            _commissionTransactionRepo = commissionTransactionRepo;
            _techCompanyRepo = techCompanyRepo;
            _orderRepo = orderRepo;
            _maintenanceRepo = maintenanceRepo;
            _pcAssemblyRepo = pcAssemblyRepo;
            _deliveryRepo = deliveryRepo;
        }

        public async Task<GeneralResponse<CommissionPlanDTO>> CreateCommissionPlanAsync(CommissionPlanCreateDTO dto)
        {
            try
            {
                var commissionPlan = new CommissionPlan
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = dto.Name,
                    Description = dto.Description,
                    ProductSaleCommission = dto.ProductSaleCommission,
                    MaintenanceCommission = dto.MaintenanceCommission,
                    PCAssemblyCommission = dto.PCAssemblyCommission,
                    DeliveryCommission = dto.DeliveryCommission,
                    MonthlySubscriptionFee = dto.MonthlySubscriptionFee,
                    IsDefault = dto.IsDefault
                };

                if (dto.IsDefault)
                {
                    // Remove default from other plans
                    var existingDefault = await _commissionPlanRepo.GetFirstOrDefaultAsync(p => p.IsDefault);
                    if (existingDefault != null)
                    {
                        existingDefault.IsDefault = false;
                        _commissionPlanRepo.Update(existingDefault);
                    }
                }

                await _commissionPlanRepo.AddAsync(commissionPlan);
                await _commissionPlanRepo.SaveChangesAsync();

                return new GeneralResponse<CommissionPlanDTO>
                {
                    Success = true,
                    Message = "Commission plan created successfully.",
                    Data = MapToCommissionPlanDTO(commissionPlan)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CommissionPlanDTO>
                {
                    Success = false,
                    Message = $"Error creating commission plan: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<CommissionPlanDTO>> UpdateCommissionPlanAsync(string id, CommissionPlanUpdateDTO dto)
        {
            try
            {
                var commissionPlan = await _commissionPlanRepo.GetByIdAsync(id);
                if (commissionPlan == null)
                {
                    return new GeneralResponse<CommissionPlanDTO>
                    {
                        Success = false,
                        Message = "Commission plan not found.",
                        Data = null
                    };
                }

                if (!string.IsNullOrWhiteSpace(dto.Name))
                    commissionPlan.Name = dto.Name;
                if (!string.IsNullOrWhiteSpace(dto.Description))
                    commissionPlan.Description = dto.Description;
                if (dto.ProductSaleCommission.HasValue)
                    commissionPlan.ProductSaleCommission = dto.ProductSaleCommission.Value;
                if (dto.MaintenanceCommission.HasValue)
                    commissionPlan.MaintenanceCommission = dto.MaintenanceCommission.Value;
                if (dto.PCAssemblyCommission.HasValue)
                    commissionPlan.PCAssemblyCommission = dto.PCAssemblyCommission.Value;
                if (dto.DeliveryCommission.HasValue)
                    commissionPlan.DeliveryCommission = dto.DeliveryCommission.Value;
                if (dto.MonthlySubscriptionFee.HasValue)
                    commissionPlan.MonthlySubscriptionFee = dto.MonthlySubscriptionFee.Value;
                if (dto.IsActive.HasValue)
                    commissionPlan.IsActive = dto.IsActive.Value;
                if (dto.IsDefault.HasValue)
                {
                    commissionPlan.IsDefault = dto.IsDefault.Value;
                    if (dto.IsDefault.Value)
                    {
                        // Remove default from other plans
                        var existingDefault = await _commissionPlanRepo.GetFirstOrDefaultAsync(p => p.IsDefault && p.Id != id);
                        if (existingDefault != null)
                        {
                            existingDefault.IsDefault = false;
                            _commissionPlanRepo.Update(existingDefault);
                        }
                    }
                }

                _commissionPlanRepo.Update(commissionPlan);
                await _commissionPlanRepo.SaveChangesAsync();

                return new GeneralResponse<CommissionPlanDTO>
                {
                    Success = true,
                    Message = "Commission plan updated successfully.",
                    Data = MapToCommissionPlanDTO(commissionPlan)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CommissionPlanDTO>
                {
                    Success = false,
                    Message = $"Error updating commission plan: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<CommissionPlanDTO>> GetCommissionPlanByIdAsync(string id)
        {
            try
            {
                var commissionPlan = await _commissionPlanRepo.GetByIdAsync(id);
                if (commissionPlan == null)
                {
                    return new GeneralResponse<CommissionPlanDTO>
                    {
                        Success = false,
                        Message = "Commission plan not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<CommissionPlanDTO>
                {
                    Success = true,
                    Message = "Commission plan retrieved successfully.",
                    Data = MapToCommissionPlanDTO(commissionPlan)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CommissionPlanDTO>
                {
                    Success = false,
                    Message = $"Error retrieving commission plan: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<CommissionPlanDTO>>> GetAllCommissionPlansAsync()
        {
            try
            {
                var commissionPlans = await _commissionPlanRepo.GetAllAsync();
                var dtos = commissionPlans.Select(MapToCommissionPlanDTO);

                return new GeneralResponse<IEnumerable<CommissionPlanDTO>>
                {
                    Success = true,
                    Message = "Commission plans retrieved successfully.",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<CommissionPlanDTO>>
                {
                    Success = false,
                    Message = $"Error retrieving commission plans: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<CommissionPlanDTO>> GetDefaultCommissionPlanAsync()
        {
            try
            {
                var commissionPlan = await _commissionPlanRepo.GetFirstOrDefaultAsync(p => p.IsDefault && p.IsActive);
                if (commissionPlan == null)
                {
                    return new GeneralResponse<CommissionPlanDTO>
                    {
                        Success = false,
                        Message = "No default commission plan found.",
                        Data = null
                    };
                }

                return new GeneralResponse<CommissionPlanDTO>
                {
                    Success = true,
                    Message = "Default commission plan retrieved successfully.",
                    Data = MapToCommissionPlanDTO(commissionPlan)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CommissionPlanDTO>
                {
                    Success = false,
                    Message = $"Error retrieving default commission plan: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteCommissionPlanAsync(string id)
        {
            try
            {
                var commissionPlan = await _commissionPlanRepo.GetByIdAsync(id);
                if (commissionPlan == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Commission plan not found.",
                        Data = false
                    };
                }

                if (commissionPlan.IsDefault)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Cannot delete default commission plan.",
                        Data = false
                    };
                }

                _commissionPlanRepo.Remove(commissionPlan);
                await _commissionPlanRepo.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Commission plan deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting commission plan: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<CommissionTransactionDTO>> CreateCommissionTransactionAsync(CommissionTransactionCreateDTO dto)
        {
            try
            {
                var techCompany = await _techCompanyRepo.GetByIdAsync(dto.TechCompanyId);
                if (techCompany == null)
                {
                    return new GeneralResponse<CommissionTransactionDTO>
                    {
                        Success = false,
                        Message = "Tech company not found.",
                        Data = null
                    };
                }

                var commissionPlan = techCompany.CommissionPlanId != null 
                    ? await _commissionPlanRepo.GetByIdAsync(techCompany.CommissionPlanId)
                    : await _commissionPlanRepo.GetFirstOrDefaultAsync(p => p.IsDefault);

                if (commissionPlan == null)
                {
                    return new GeneralResponse<CommissionTransactionDTO>
                    {
                        Success = false,
                        Message = "No commission plan found for tech company.",
                        Data = null
                    };
                }

                decimal commissionRate = GetCommissionRate(commissionPlan, dto.ServiceType);
                decimal commissionAmount = dto.ServiceAmount * commissionRate;
                decimal vendorPayout = dto.ServiceAmount - commissionAmount;

                var transaction = new CommissionTransaction
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = dto.OrderId,
                    MaintenanceId = dto.MaintenanceId,
                    PCAssemblyId = dto.PCAssemblyId,
                    DeliveryId = dto.DeliveryId,
                    ServiceType = dto.ServiceType,
                    ServiceAmount = dto.ServiceAmount,
                    CommissionAmount = commissionAmount,
                    VendorPayout = vendorPayout,
                    PlatformFee = commissionAmount,
                    TechCompanyId = dto.TechCompanyId,
                    DeliveryPersonId = dto.DeliveryPersonId,
                    Status = CommissionStatus.Pending
                };

                await _commissionTransactionRepo.AddAsync(transaction);
                await _commissionTransactionRepo.SaveChangesAsync();

                return new GeneralResponse<CommissionTransactionDTO>
                {
                    Success = true,
                    Message = "Commission transaction created successfully.",
                    Data = MapToCommissionTransactionDTO(transaction)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CommissionTransactionDTO>
                {
                    Success = false,
                    Message = $"Error creating commission transaction: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<decimal>> CalculateCommissionAsync(decimal amount, string serviceType, string techCompanyId)
        {
            try
            {
                var techCompany = await _techCompanyRepo.GetByIdAsync(techCompanyId);
                if (techCompany == null)
                {
                    return new GeneralResponse<decimal>
                    {
                        Success = false,
                        Message = "Tech company not found.",
                        Data = 0
                    };
                }

                var commissionPlan = techCompany.CommissionPlanId != null 
                    ? await _commissionPlanRepo.GetByIdAsync(techCompany.CommissionPlanId)
                    : await _commissionPlanRepo.GetFirstOrDefaultAsync(p => p.IsDefault);

                if (commissionPlan == null)
                {
                    return new GeneralResponse<decimal>
                    {
                        Success = false,
                        Message = "No commission plan found.",
                        Data = 0
                    };
                }

                var serviceTypeEnum = Enum.Parse<ServiceType>(serviceType);
                decimal commissionRate = GetCommissionRate(commissionPlan, serviceTypeEnum);
                decimal commissionAmount = amount * commissionRate;

                return new GeneralResponse<decimal>
                {
                    Success = true,
                    Message = "Commission calculated successfully.",
                    Data = commissionAmount
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<decimal>
                {
                    Success = false,
                    Message = $"Error calculating commission: {ex.Message}",
                    Data = 0
                };
            }
        }

        private decimal GetCommissionRate(CommissionPlan plan, ServiceType serviceType)
        {
            return serviceType switch
            {
                ServiceType.ProductSale => plan.ProductSaleCommission,
                ServiceType.Maintenance => plan.MaintenanceCommission,
                ServiceType.PCAssembly => plan.PCAssemblyCommission,
                ServiceType.Delivery => plan.DeliveryCommission,
                _ => 0.10m // Default 10%
            };
        }

        private CommissionPlanDTO MapToCommissionPlanDTO(CommissionPlan entity)
        {
            return new CommissionPlanDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ProductSaleCommission = entity.ProductSaleCommission,
                MaintenanceCommission = entity.MaintenanceCommission,
                PCAssemblyCommission = entity.PCAssemblyCommission,
                DeliveryCommission = entity.DeliveryCommission,
                MonthlySubscriptionFee = entity.MonthlySubscriptionFee,
                IsActive = entity.IsActive,
                IsDefault = entity.IsDefault
            };
        }

        private CommissionTransactionDTO MapToCommissionTransactionDTO(CommissionTransaction entity)
        {
            return new CommissionTransactionDTO
            {
                Id = entity.Id,
                OrderId = entity.OrderId,
                MaintenanceId = entity.MaintenanceId,
                PCAssemblyId = entity.PCAssemblyId,
                DeliveryId = entity.DeliveryId,
                ServiceType = entity.ServiceType,
                ServiceAmount = entity.ServiceAmount,
                CommissionAmount = entity.CommissionAmount,
                VendorPayout = entity.VendorPayout,
                PlatformFee = entity.PlatformFee,
                TechCompanyId = entity.TechCompanyId,
                DeliveryPersonId = entity.DeliveryPersonId,
                Status = entity.Status,
                PayoutDate = entity.PayoutDate,
                PayoutReference = entity.PayoutReference,
                CreatedAt = entity.CreatedAt
            };
        }

        // Additional methods implementation...
        public async Task<GeneralResponse<CommissionTransactionDTO>> GetCommissionTransactionByIdAsync(string id)
        {
            // Implementation
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse<IEnumerable<CommissionTransactionDTO>>> GetCommissionTransactionsByTechCompanyAsync(string techCompanyId)
        {
            // Implementation
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse<IEnumerable<CommissionTransactionDTO>>> GetCommissionTransactionsByDeliveryPersonAsync(string deliveryPersonId)
        {
            // Implementation
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse<CommissionTransactionDTO>> UpdateCommissionStatusAsync(string id, string status)
        {
            // Implementation
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse<CommissionSummaryDTO>> GetCommissionSummaryAsync(string techCompanyId, DateTime startDate, DateTime endDate)
        {
            // Implementation
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse<bool>> ProcessPayoutAsync(string transactionId)
        {
            // Implementation
            throw new NotImplementedException();
        }
    }
} 