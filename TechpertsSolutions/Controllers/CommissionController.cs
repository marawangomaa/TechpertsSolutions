using Core.DTOs.CommissionDTOs;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Enums;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommissionController : ControllerBase
    {
        private readonly ICommissionService _commissionService;

        public CommissionController(ICommissionService commissionService)
        {
            _commissionService = commissionService;
        }

        [HttpPost("plans")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GeneralResponse<CommissionPlanDTO>>> CreateCommissionPlan([FromBody] CommissionPlanCreateDTO dto)
        {
            var result = await _commissionService.CreateCommissionPlanAsync(dto);
            return Ok(result);
        }

        [HttpPut("plans/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GeneralResponse<CommissionPlanDTO>>> UpdateCommissionPlan(string id, [FromBody] CommissionPlanUpdateDTO dto)
        {
            var result = await _commissionService.UpdateCommissionPlanAsync(id, dto);
            return Ok(result);
        }

        [HttpGet("plans/{id}")]
        public async Task<ActionResult<GeneralResponse<CommissionPlanDTO>>> GetCommissionPlan(string id)
        {
            var result = await _commissionService.GetCommissionPlanByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("plans")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<CommissionPlanDTO>>>> GetAllCommissionPlans()
        {
            var result = await _commissionService.GetAllCommissionPlansAsync();
            return Ok(result);
        }

        [HttpGet("plans/default")]
        public async Task<ActionResult<GeneralResponse<CommissionPlanDTO>>> GetDefaultCommissionPlan()
        {
            var result = await _commissionService.GetDefaultCommissionPlanAsync();
            return Ok(result);
        }

        [HttpDelete("plans/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GeneralResponse<bool>>> DeleteCommissionPlan(string id)
        {
            var result = await _commissionService.DeleteCommissionPlanAsync(id);
            return Ok(result);
        }

        [HttpPost("transactions")]
        public async Task<ActionResult<GeneralResponse<CommissionTransactionDTO>>> CreateCommissionTransaction([FromBody] CommissionTransactionCreateDTO dto, [FromQuery] ServiceType serviceType)
        {
            // Create a new DTO with the serviceType from query parameter
            var dtoWithServiceType = new CommissionTransactionCreateDTO
            {
                OrderId = dto.OrderId,
                MaintenanceId = dto.MaintenanceId,
                PCAssemblyId = dto.PCAssemblyId,
                DeliveryId = dto.DeliveryId,
                ServiceType = serviceType,
                ServiceAmount = dto.ServiceAmount,
                TechCompanyId = dto.TechCompanyId,
                DeliveryPersonId = dto.DeliveryPersonId
            };

            var result = await _commissionService.CreateCommissionTransactionAsync(dtoWithServiceType);
            return Ok(result);
        }

        [HttpGet("transactions/{id}")]
        public async Task<ActionResult<GeneralResponse<CommissionTransactionDTO>>> GetCommissionTransaction(string id)
        {
            var result = await _commissionService.GetCommissionTransactionByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("transactions/techcompany/{techCompanyId}")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<CommissionTransactionDTO>>>> GetCommissionTransactionsByTechCompany(string techCompanyId)
        {
            var result = await _commissionService.GetCommissionTransactionsByTechCompanyAsync(techCompanyId);
            return Ok(result);
        }

        [HttpGet("transactions/deliveryperson/{deliveryPersonId}")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<CommissionTransactionDTO>>>> GetCommissionTransactionsByDeliveryPerson(string deliveryPersonId)
        {
            var result = await _commissionService.GetCommissionTransactionsByDeliveryPersonAsync(deliveryPersonId);
            return Ok(result);
        }

        [HttpPut("transactions/{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GeneralResponse<CommissionTransactionDTO>>> UpdateCommissionStatus(string id, [FromQuery] CommissionStatus status)
        {
            var result = await _commissionService.UpdateCommissionStatusAsync(id, status.ToString());
            return Ok(result);
        }

        [HttpGet("summary/{techCompanyId}")]
        public async Task<ActionResult<GeneralResponse<CommissionSummaryDTO>>> GetCommissionSummary(
            string techCompanyId, 
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var result = await _commissionService.GetCommissionSummaryAsync(techCompanyId, startDate, endDate);
            return Ok(result);
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<GeneralResponse<decimal>>> CalculateCommission(
            [FromQuery] decimal amount, 
            [FromQuery] ServiceType serviceType, 
            [FromQuery] string techCompanyId)
        {
            var result = await _commissionService.CalculateCommissionAsync(amount, serviceType.ToString(), techCompanyId);
            return Ok(result);
        }

        [HttpPost("transactions/{id}/payout")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GeneralResponse<bool>>> ProcessPayout(string id)
        {
            var result = await _commissionService.ProcessPayoutAsync(id);
            return Ok(result);
        }
    }
} 