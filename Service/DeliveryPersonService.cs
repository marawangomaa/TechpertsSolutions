using System.Transactions;
using Core.DTOs;
using Core.DTOs.DeliveryDTOs;
using Core.DTOs.DeliveryPersonDTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Utilities;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class DeliveryPersonService : IDeliveryPersonService
    {
        private readonly IRepository<DeliveryPerson> _deliveryPersonRepo;
        private readonly IRepository<DeliveryOffer> _deliveryOfferRepo;
        private readonly IRepository<Delivery> _deliveryRepo;
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationService _notificationService;
        private readonly IFileService _fileService;

        public DeliveryPersonService(
            IRepository<DeliveryPerson> deliveryPersonRepo,
            IRepository<DeliveryOffer> deliveryOfferRepo,
            IRepository<Delivery> deliveryRepo,
            IServiceProvider serviceProvider,
            INotificationService notificationService,
            IFileService fileService
        )
        {
            _deliveryPersonRepo = deliveryPersonRepo;
            _deliveryOfferRepo = deliveryOfferRepo;
            _deliveryRepo = deliveryRepo;
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
            _fileService = fileService;
        }

        private IDeliveryClusterService _clusterService =>
            _serviceProvider.GetRequiredService<IDeliveryClusterService>();

        public async Task<GeneralResponse<DeliveryPersonReadDTO>> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = false,
                    Message = "DeliveryPerson ID cannot be null or empty.",
                    Data = null,
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = false,
                    Message = "Invalid DeliveryPerson ID format. Expected GUID format.",
                    Data = null,
                };
            }

            try
            {
                var deliveryPerson = await _deliveryPersonRepo.GetByIdWithIncludesAsync(
                    id,
                    dp => dp.User,
                    dp => dp.Role
                );

                if (deliveryPerson == null)
                {
                    return new GeneralResponse<DeliveryPersonReadDTO>
                    {
                        Success = false,
                        Message = $"DeliveryPerson with ID '{id}' not found.",
                        Data = null,
                    };
                }

                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = true,
                    Message = "DeliveryPerson retrieved successfully.",
                    Data = DeliveryPersonMapper.ToReadDTO(deliveryPerson),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryPersonReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the delivery person.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>> GetAllAsync()
        {
            try
            {
                var deliveryPersons = await _deliveryPersonRepo.GetAllWithIncludesAsync(
                    dp => dp.User,
                    dp => dp.Role
                );

                var deliveryPersonDtos = deliveryPersons
                    .Select(DeliveryPersonMapper.ToReadDTO)
                    .ToList();

                return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                {
                    Success = true,
                    Message = "DeliveryPersons retrieved successfully.",
                    Data = deliveryPersonDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                {
                    Success = false,
                    Message =
                        $"An unexpected error occurred while retrieving delivery persons. {ex}",
                    Data = null,
                };
            }
        }

        public async Task<DeliveryPersonReadDTO?> UpdateAsync(
            string id,
            DeliveryPersonUpdateDTO dto,
            IFormFile? vehicleImage
        )
        {
            var entity = await _deliveryPersonRepo.GetByIdWithIncludesAsync(id, dp => dp.User);
            if (entity == null)
                return null;

            // Map basic fields from DTO
            DeliveryPersonMapper.UpdateEntity(entity, dto);

            // Handle vehicle image (from DTO or from parameter)
            var imageToUse = vehicleImage ?? vehicleImage;

            if (imageToUse != null)
            {
                if (!string.IsNullOrEmpty(entity.VehicleImage))
                    await _fileService.DeleteImageAsync(entity.VehicleImage);

                entity.VehicleImage = await _fileService.UploadImageAsync(
                    imageToUse,
                    "vehicle-images"
                );
            }

            _deliveryPersonRepo.Update(entity);
            return DeliveryPersonMapper.ToReadDTO(entity);
        }

        public async Task<DeliveryPersonReadDTO?> UpdateAdminAsync(
            string id,
            DeliveryPersonAdminUpdateDTO dto
        )
        {
            var entity = await _deliveryPersonRepo.GetByIdWithIncludesAsync(id, dp => dp.User);
            if (entity == null)
                return null;

            DeliveryPersonMapper.UpdateAdminEntity(
                entity,
                dto.AccountStatus,
                new DeliveryPersonUpdateDTO
                {
                    VehicleNumber = dto.VehicleNumber,
                    VehicleType = dto.VehicleType,
                    PhoneNumber = dto.PhoneNumber,
                    City = dto.City,
                    Country = dto.Country,
                    IsAvailable = dto.IsAvailable,
                }
            );

            _deliveryPersonRepo.Update(entity);
            return DeliveryPersonMapper.ToReadDTO(entity);
        }

        public async Task<
            GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
        > GetAvailableDeliveryPersonsAsync()
        {
            try
            {
                var availableDeliveryPersons = await _deliveryPersonRepo.FindWithIncludesAsync(
                    dp =>
                        dp.AccountStatus == DeliveryPersonStatus.Accepted
                        && dp.IsAvailable == true
                        && dp.User.Latitude != null
                        && dp.User.Longitude != null,
                    dp => dp.User,
                    dp => dp.Role
                );

                var dtoList = availableDeliveryPersons
                    .Select(DeliveryPersonMapper.ToReadDTO)
                    .ToList();

                return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                {
                    Success = true,
                    Message = "Available delivery persons retrieved successfully.",
                    Data = dtoList,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                {
                    Success = false,
                    Message =
                        $"An unexpected error occurred while retrieving available delivery persons.{ex}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryOfferDTO>>> GetAllOffersAsync(
            string driverId
        )
        {
            if (string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<IEnumerable<DeliveryOfferDTO>>
                {
                    Success = false,
                    Message = "Driver ID required.",
                };

            var offers = await _deliveryOfferRepo.FindWithIncludesAsync(
                o => o.DeliveryPersonId == driverId,
                o => o.Delivery,
                o => o.Delivery.TechCompanies
            );

            var dtoList = offers.Select(DeliveryPersonMapper.ToDTO).ToList();
            return new GeneralResponse<IEnumerable<DeliveryOfferDTO>>
            {
                Success = true,
                Message = "All offers retrieved.",
                Data = dtoList,
            };
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryOfferDTO>>> GetPendingOffersAsync(
            string driverId
        )
        {
            if (string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<IEnumerable<DeliveryOfferDTO>>
                {
                    Success = false,
                    Message = "Driver ID required.",
                };

            var offers = await _deliveryOfferRepo.FindWithIncludesAsync(
                o =>
                    o.DeliveryPersonId == driverId
                    && o.Status == DeliveryOfferStatus.Pending
                    && o.IsActive,
                o => o.Delivery,
                o => o.DeliveryPerson.User,
                o => o.Delivery.TechCompanies
            );

            var dtoList = offers.Select(DeliveryPersonMapper.ToOfferDTO).ToList();
            return new GeneralResponse<IEnumerable<DeliveryOfferDTO>>
            {
                Success = true,
                Message = "Pending offers retrieved.",
                Data = dtoList,
            };
        }

        public async Task<GeneralResponse<bool>> AcceptOfferAsync(string offerId, string driverId)
        {
            if (string.IsNullOrWhiteSpace(offerId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Offer ID and Driver ID are required.",
                };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var offer = await _deliveryOfferRepo.GetByIdAsync(offerId);
                if (
                    offer == null
                    || offer.DeliveryPersonId != driverId
                    || offer.Status != DeliveryOfferStatus.Pending
                )
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Offer not valid or already handled.",
                    };

                offer.Status = DeliveryOfferStatus.Accepted;
                offer.IsActive = false;
                _deliveryOfferRepo.Update(offer);

                var otherOffers = await _deliveryOfferRepo.FindWithIncludesAsync(
                    o =>
                        o.ClusterId == offer.ClusterId
                        && o.Id != offer.Id
                        && o.Status == DeliveryOfferStatus.Pending,
                    o => o.DeliveryPerson,
                    o => o.Delivery.TechCompanies
                );

                foreach (var o in otherOffers)
                {
                    o.Status = DeliveryOfferStatus.Expired;
                    o.IsActive = false;
                    _deliveryOfferRepo.Update(o);

                    if (o.DeliveryPerson != null)
                    {
                        await _notificationService.SendNotificationAsync(
                            o.DeliveryPerson.UserId,
                            NotificationType.DeliveryOfferExpired,
                            o.DeliveryId,
                            "Delivery",
                            $"Delivery offer #{o.DeliveryId} expired."
                        );
                    }
                }

                var assignResult = await _clusterService.AssignDriverAsync(
                    offer.ClusterId,
                    driverId
                );
                if (!assignResult.Success)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = assignResult.Message,
                    };

                var delivery = await _deliveryRepo.GetByIdWithIncludesAsync(offer.DeliveryId);

                var driver = await _deliveryPersonRepo.GetByIdWithIncludesAsync(
                    driverId,
                    d => d.User
                );

                delivery.DeliveryPerson = driver;
                delivery.DeliveryPersonId = driver.Id;
                delivery.Status = DeliveryStatus.Assigned;

                _deliveryRepo.Update(delivery);
                await _deliveryRepo.SaveChangesAsync();

                // Safe notification
                if (delivery.DeliveryPerson?.User != null)
                {
                    await _notificationService.SendNotificationAsync(
                        delivery.DeliveryPerson.UserId,
                        NotificationType.DeliveryOfferAccepted,
                        "Delivery",
                        delivery.TrackingNumber ?? delivery.Id,
                        "New Order Assigned"
                    );
                }

                scope.Complete();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Offer accepted successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}.",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeclineOfferAsync(string offerId, string driverId)
        {
            if (string.IsNullOrWhiteSpace(offerId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Offer ID and Driver ID are required.",
                };

            try
            {
                var offer = await _deliveryOfferRepo.GetByIdWithIncludesAsync(
                    offerId,
                    o => o.DeliveryPerson,
                    o => o.Delivery.TechCompanies
                );
                if (
                    offer == null
                    || offer.DeliveryPersonId != driverId
                    || offer.Status != DeliveryOfferStatus.Pending
                )
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Offer not valid or already handled.",
                    };

                offer.Status = DeliveryOfferStatus.Declined;
                offer.IsActive = false;
                _deliveryOfferRepo.Update(offer);
                await _deliveryOfferRepo.SaveChangesAsync();

                await _notificationService.SendNotificationAsync(
                    offer.DeliveryPerson.UserId,
                    NotificationType.DeliveryOfferDeclined,
                    "Delivery",
                    offer.DeliveryId,
                    "Order Have Been Declined."
                );

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Offer declined successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<bool>> CancelOfferAsync(string offerId, string driverId)
        {
            if (string.IsNullOrWhiteSpace(offerId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Offer ID and Driver ID are required.",
                };

            try
            {
                var offer = await _deliveryOfferRepo.GetByIdWithIncludesAsync(
                    offerId,
                    o => o.DeliveryPerson,
                    o => o.Delivery.TechCompanies
                );
                if (
                    offer == null
                    || offer.DeliveryPersonId != driverId
                    || offer.Status != DeliveryOfferStatus.Accepted
                )
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Offer not valid or not accepted yet.",
                    };

                offer.Status = DeliveryOfferStatus.Canceled;
                offer.IsActive = false;
                _deliveryOfferRepo.Update(offer);

                var delivery = await _deliveryRepo.GetByIdWithIncludesAsync(
                    offer.DeliveryId,
                    o => o.DeliveryPerson
                );
                delivery.Status = DeliveryStatus.Pending;
                delivery.DeliveryPersonId = null;
                _deliveryRepo.Update(delivery);

                await _deliveryOfferRepo.SaveChangesAsync();
                await _deliveryRepo.SaveChangesAsync();

                await _notificationService.SendNotificationAsync(
                    delivery.DeliveryPerson.UserId,
                    NotificationType.DeliveryOfferCanceled,
                    "Delivery",
                    delivery.TrackingNumber ?? delivery.Id,
                    "Order Have Been Canceled."
                );

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Offer canceled successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Data = false,
                };
            }
        }
    }
}
