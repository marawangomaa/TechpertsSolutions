using Core.DTOs.DeliveryDTOs;
using Core.DTOs;
using Core.Enums;
using TechpertsSolutions.Core.Entities;
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
                // Optimized includes for delivery listing with essential related data
                var deliveries = await _deliveryrepo.GetAllWithIncludesAsync(
                    d => d.Customer,
                    d => d.Customer.User,
                    d => d.DeliveryPerson,
                    d => d.DeliveryPerson.User);

                var deliveryDtos = deliveries.Select(DeliveryMapper.MapToDeliveryDTO).ToList();

                return new GeneralResponse<IEnumerable<DeliveryDTO>>
                {
                    Success = true,
                    Message = "Deliveries retrieved successfully.",
                    Data = deliveryDtos
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
                // Comprehensive includes for detailed delivery view
                var delivery = await _deliveryrepo.GetByIdWithIncludesAsync(id,
                    d => d.Customer,
                    d => d.Customer.User,
                    d => d.DeliveryPerson,
                    d => d.DeliveryPerson.User);

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

        public async Task<GeneralResponse<DeliveryDTO>> AddAsync(DeliveryCreateDTO dto)
        {
            try
            {
                var entity = DeliveryMapper.MapToDelivery(dto);

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

        public async Task<GeneralResponse<DeliveryDTO>> UpdateAsync(string id, DeliveryUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "Delivery ID cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var entity = await _deliveryrepo.GetByIdAsync(id);
                if (entity == null)
                {
                    return new GeneralResponse<DeliveryDTO>
                    {
                        Success = false,
                        Message = $"Delivery with ID '{id}' not found.",
                        Data = null
                    };
                }

                DeliveryMapper.UpdateDelivery(entity, dto);
                _deliveryrepo.Update(entity);
                await _deliveryrepo.SaveChangesAsync();

                return new GeneralResponse<DeliveryDTO>
                {
                    Success = true,
                    Message = "Delivery updated successfully.",
                    Data = DeliveryMapper.MapToDeliveryDTO(entity)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the delivery.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            
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
                var delivery = await _deliveryrepo.GetByIdWithIncludesAsync(id,
                    d => d.DeliveryPerson,
                    d => d.Customer,
                    d => d.TechCompanies
                );

                if (delivery == null)
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
                    Data = DeliveryMapper.MapToDeliveryDetailsDTO(delivery)
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

        public async Task<GeneralResponse<IEnumerable<DeliveryDTO>>> GetByDeliveryPersonIdAsync(string deliveryPersonId)
        {
            if (string.IsNullOrWhiteSpace(deliveryPersonId))
            {
                return new GeneralResponse<IEnumerable<DeliveryDTO>>
                {
                    Success = false,
                    Message = "Delivery Person ID cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var deliveries = await _deliveryrepo.FindWithIncludesAsync(
                    d => d.DeliveryPersonId == deliveryPersonId,
                    d => d.DeliveryPerson,
                    d => d.Customer
                );

                return new GeneralResponse<IEnumerable<DeliveryDTO>>
                {
                    Success = true,
                    Message = "Deliveries for delivery person retrieved successfully.",
                    Data = DeliveryMapper.MapToDeliveryDTOList(deliveries)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<DeliveryDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving deliveries for delivery person.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryDTO>>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return new GeneralResponse<IEnumerable<DeliveryDTO>>
                {
                    Success = false,
                    Message = "Status cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                // Parse the string status to enum
                if (Enum.TryParse<DeliveryStatus>(status, out var deliveryStatus))
                {
                    var deliveries = await _deliveryrepo.FindWithIncludesAsync(
                        d => d.Status == deliveryStatus,
                        d => d.DeliveryPerson,
                        d => d.Customer
                    );

                    return new GeneralResponse<IEnumerable<DeliveryDTO>>
                    {
                        Success = true,
                        Message = $"Deliveries with status '{status}' retrieved successfully.",
                        Data = DeliveryMapper.MapToDeliveryDTOList(deliveries)
                    };
                }
                else
                {
                    return new GeneralResponse<IEnumerable<DeliveryDTO>>
                    {
                        Success = false,
                        Message = $"Invalid status: {status}",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<DeliveryDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving deliveries by status.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<DeliveryDTO>> AssignDeliveryToPersonAsync(string deliveryId, string deliveryPersonId)
        {
            if (string.IsNullOrWhiteSpace(deliveryId) || string.IsNullOrWhiteSpace(deliveryPersonId))
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "Delivery ID and Delivery Person ID cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var delivery = await _deliveryrepo.GetByIdAsync(deliveryId);
                if (delivery == null)
                {
                    return new GeneralResponse<DeliveryDTO>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                        Data = null
                    };
                }

                delivery.DeliveryPersonId = deliveryPersonId;
                delivery.Status = DeliveryStatus.Assigned;
                _deliveryrepo.Update(delivery);
                await _deliveryrepo.SaveChangesAsync();

                return new GeneralResponse<DeliveryDTO>
                {
                    Success = true,
                    Message = "Delivery assigned to delivery person successfully.",
                    Data = DeliveryMapper.MapToDeliveryDTO(delivery)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while assigning delivery.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<DeliveryDTO>> UpdateDeliveryStatusAsync(string deliveryId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(deliveryId) || string.IsNullOrWhiteSpace(newStatus))
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "Delivery ID and new status cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var delivery = await _deliveryrepo.GetByIdAsync(deliveryId);
                if (delivery == null)
                {
                    return new GeneralResponse<DeliveryDTO>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                        Data = null
                    };
                }

                // Parse the string status to enum
                if (Enum.TryParse<DeliveryStatus>(newStatus, out var status))
                {
                    delivery.Status = status;
                }
                else
                {
                    return new GeneralResponse<DeliveryDTO>
                    {
                        Success = false,
                        Message = $"Invalid status: {newStatus}",
                        Data = null
                    };
                }
                _deliveryrepo.Update(delivery);
                await _deliveryrepo.SaveChangesAsync();

                return new GeneralResponse<DeliveryDTO>
                {
                    Success = true,
                    Message = $"Delivery status updated to '{newStatus}' successfully.",
                    Data = DeliveryMapper.MapToDeliveryDTO(delivery)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating delivery status.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<DeliveryDTO>> CompleteDeliveryAsync(string deliveryId, string deliveryPersonId)
        {
            if (string.IsNullOrWhiteSpace(deliveryId) || string.IsNullOrWhiteSpace(deliveryPersonId))
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "Delivery ID and Delivery Person ID cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var delivery = await _deliveryrepo.GetByIdAsync(deliveryId);
                if (delivery == null)
                {
                    return new GeneralResponse<DeliveryDTO>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                        Data = null
                    };
                }

                if (delivery.DeliveryPersonId != deliveryPersonId)
                {
                    return new GeneralResponse<DeliveryDTO>
                    {
                        Success = false,
                        Message = "This delivery is not assigned to you.",
                        Data = null
                    };
                }

                delivery.Status = DeliveryStatus.Delivered;
                delivery.ActualDeliveryDate = DateTime.UtcNow;
                _deliveryrepo.Update(delivery);
                await _deliveryrepo.SaveChangesAsync();

                return new GeneralResponse<DeliveryDTO>
                {
                    Success = true,
                    Message = "Delivery completed successfully.",
                    Data = DeliveryMapper.MapToDeliveryDTO(delivery)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while completing delivery.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryDTO>>> GetAvailableDeliveriesAsync()
        {
            try
            {
                var deliveries = await _deliveryrepo.FindWithIncludesAsync(
                    d => d.Status == DeliveryStatus.Assigned && d.DeliveryPersonId == null,
                    d => d.Customer
                );

                return new GeneralResponse<IEnumerable<DeliveryDTO>>
                {
                    Success = true,
                    Message = "Available deliveries retrieved successfully.",
                    Data = DeliveryMapper.MapToDeliveryDTOList(deliveries)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<DeliveryDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving available deliveries.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<DeliveryDTO>> AcceptDeliveryAsync(string deliveryId, string deliveryPersonId)
        {
            if (string.IsNullOrWhiteSpace(deliveryId) || string.IsNullOrWhiteSpace(deliveryPersonId))
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "Delivery ID and Delivery Person ID cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var delivery = await _deliveryrepo.GetByIdAsync(deliveryId);
                if (delivery == null)
                {
                    return new GeneralResponse<DeliveryDTO>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                        Data = null
                    };
                }

                if (delivery.Status != DeliveryStatus.Assigned || delivery.DeliveryPersonId != null)
                {
                    return new GeneralResponse<DeliveryDTO>
                    {
                        Success = false,
                        Message = "This delivery is not available for acceptance.",
                        Data = null
                    };
                }

                delivery.DeliveryPersonId = deliveryPersonId;
                delivery.Status = DeliveryStatus.PickedUp;
                _deliveryrepo.Update(delivery);
                await _deliveryrepo.SaveChangesAsync();

                return new GeneralResponse<DeliveryDTO>
                {
                    Success = true,
                    Message = "Delivery accepted successfully.",
                    Data = DeliveryMapper.MapToDeliveryDTO(delivery)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while accepting delivery.",
                    Data = null
                };
            }
        }
    }
}
    
