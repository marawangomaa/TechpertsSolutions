using Core.DTOs.Delivery;
using TechpertsSolutions.Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IRepository<Delivery> _deliveryrepo;

        public DeliveryService(IRepository<Delivery> deliveryRepo)
        {
            _deliveryrepo = deliveryRepo;
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryDTO>>> GetAllAsync()
        {
            try
            {
                var deliveries = await _deliveryrepo.GetAllAsync();
                return new GeneralResponse<IEnumerable<DeliveryDTO>>
                {
                    Success = true,
                    Message = "Deliveries retrieved successfully.",
                    Data = DeliveryMapper.MapToDeliveryDTOList(deliveries)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<DeliveryDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving deliveries.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<DeliveryDTO>> GetByIdAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "Delivery ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "Invalid Delivery ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var delivery = await _deliveryrepo.GetByIdAsync(id);
                if (delivery == null)
                {
                    return new GeneralResponse<DeliveryDTO>
                    {
                        Success = false,
                        Message = $"Delivery with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<DeliveryDTO>
                {
                    Success = true,
                    Message = "Delivery retrieved successfully.",
                    Data = DeliveryMapper.MapToDeliveryDTO(delivery)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the delivery.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<DeliveryDTO>> AddAsync()
        {
            try
            {
                var entity = DeliveryMapper.MapToDelivery();

                await _deliveryrepo.AddAsync(entity);
                await _deliveryrepo.SaveChangesAsync();

                return new GeneralResponse<DeliveryDTO>
                {
                    Success = true,
                    Message = "Delivery created successfully.",
                    Data = DeliveryMapper.MapToDeliveryDTO(entity)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the delivery.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Delivery ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Delivery ID format. Expected GUID format.",
                    Data = false
                };
            }

            try
            {
                var entity = await _deliveryrepo.GetByIdAsync(id);
                if (entity == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Delivery with ID '{id}' not found.",
                        Data = false
                    };
                }

                _deliveryrepo.Remove(entity);
                await _deliveryrepo.SaveChangesAsync();
                
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Delivery deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while deleting the delivery.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<DeliveryDetailsDTO>> GetDetailsByIdAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<DeliveryDetailsDTO>
                {
                    Success = false,
                    Message = "Delivery ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<DeliveryDetailsDTO>
                {
                    Success = false,
                    Message = "Invalid Delivery ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var entity = await _deliveryrepo.GetByIdAsync(id);
                if (entity == null)
                {
                    return new GeneralResponse<DeliveryDetailsDTO>
                    {
                        Success = false,
                        Message = $"Delivery with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<DeliveryDetailsDTO>
                {
                    Success = true,
                    Message = "Delivery details retrieved successfully.",
                    Data = DeliveryMapper.MapToDeliveryDetailsDTO(entity)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryDetailsDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving delivery details.",
                    Data = null
                };
            }
        }
    }
}
    
