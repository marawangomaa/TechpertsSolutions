using Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminReadDTO>> GetAllAsync();
        Task<AdminReadDTO> GetByIdAsync(string id);
        
    }
}
