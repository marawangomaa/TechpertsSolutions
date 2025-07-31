using Core.DTOs.DeliveryPersonDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class DeliveryPersonService : IDeliveryPersonService
    {
        private readonly IRepository<DeliveryPerson> _deliveryPersonRepo;

        public DeliveryPersonService(IRepository<DeliveryPerson> deliveryPersonRepo)
        {
            _deliveryPersonRepo = deliveryPersonRepo;
        }

        public async Task<GeneralResponse<DeliveryPersonReadDTO>> CreateAsync(DeliveryPersonCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.RoleId))
            {
                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = false,
                    Message = "Invalid delivery person data.",
                    Data = null
                };
            }

            var deliveryPerson = DeliveryPersonMapper.ToEntity(dto);
            await _deliveryPersonRepo.AddAsync(deliveryPerson);
            await _deliveryPersonRepo.SaveChangesAsync();

            return new GeneralResponse<DeliveryPersonReadDTO>
            {
                Success = true,
                Message = "Delivery person created successfully.",
                Data = DeliveryPersonMapper.ToReadDTO(deliveryPerson)
            };
        }

        public async Task<GeneralResponse<DeliveryPersonReadDTO>> GetByIdAsync(string id)
        {
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = false,
                    Message = "DeliveryPerson ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = false,
                    Message = "Invalid DeliveryPerson ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Comprehensive includes for detailed delivery person view with user and role information
                var deliveryPerson = await _deliveryPersonRepo.GetByIdWithIncludesAsync(id, 
                    dp => dp.User, 
                    dp => dp.Role);

                if (deliveryPerson == null)
                {
                    return new GeneralResponse<DeliveryPersonReadDTO>
                    {
                        Success = false,
                        Message = $"DeliveryPerson with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = true,
                    Message = "DeliveryPerson retrieved successfully.",
                    Data = DeliveryPersonMapper.MapToDeliveryPersonReadDTO(deliveryPerson)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the delivery person.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>> GetAllAsync()
        {
            try
            {
                // Optimized includes for delivery person listing with user and role information
                var deliveryPersons = await _deliveryPersonRepo.GetAllWithIncludesAsync(
                    dp => dp.User, 
                    dp => dp.Role);

                var deliveryPersonDtos = deliveryPersons.Select(DeliveryPersonMapper.MapToDeliveryPersonReadDTO).ToList();

                return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                {
                    Success = true,
                    Message = "DeliveryPersons retrieved successfully.",
                    Data = deliveryPersonDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving delivery persons.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<DeliveryPersonReadDTO>> UpdateAsync(string id, DeliveryPersonUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id) || dto == null)
            {
                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = false,
                    Message = "Invalid input.",
                    Data = null
                };
            }

            var deliveryPerson = await _deliveryPersonRepo.GetByIdAsync(id);

            if (deliveryPerson == null)
            {
                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = false,
                    Message = $"Delivery person with ID '{id}' not found.",
                    Data = null
                };
            }

            DeliveryPersonMapper.UpdateEntity(deliveryPerson, dto);

            _deliveryPersonRepo.Update(deliveryPerson);
            await _deliveryPersonRepo.SaveChangesAsync();

            return new GeneralResponse<DeliveryPersonReadDTO>
            {
                Success = true,
                Message = "Delivery person updated successfully.",
                Data = DeliveryPersonMapper.ToReadDTO(deliveryPerson)
            };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "ID cannot be null or empty.",
                    Data = false
                };
            }

            var deliveryPerson = await _deliveryPersonRepo.GetByIdAsync(id);

            if (deliveryPerson == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Delivery person with ID '{id}' not found.",
                    Data = false
                };
            }

            _deliveryPersonRepo.Remove(deliveryPerson);
            await _deliveryPersonRepo.SaveChangesAsync();

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Delivery person deleted successfully.",
                Data = true
            };
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>> GetAvailableDeliveryPersonsAsync()
        {
            var availableDeliveryPersons = await _deliveryPersonRepo.FindWithIncludesAsync(
                dp => dp.IsAvailable, 
                dp => dp.User, 
                dp => dp.Role
            );

            return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
            {
                Success = true,
                Message = "Available delivery persons retrieved successfully.",
                Data = availableDeliveryPersons.Select(DeliveryPersonMapper.ToReadDTO)
            };
        }
    }
} 
