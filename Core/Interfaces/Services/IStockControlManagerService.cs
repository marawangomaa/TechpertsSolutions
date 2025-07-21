using Core.DTOs.StockControlManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface IStockControlManagerService
    {
        Task<GeneralResponse<StockControlManagerReadDTO>> CreateAsync(StockControlManagerCreateDTO dto);
        Task<GeneralResponse<StockControlManagerReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<StockControlManagerReadDTO>>> GetAllAsync();
        Task<GeneralResponse<StockControlManagerReadDTO>> UpdateAsync(string id, StockControlManagerUpdateDTO dto);
        Task<GeneralResponse<string>> DeleteAsync(string id);
    }
}
