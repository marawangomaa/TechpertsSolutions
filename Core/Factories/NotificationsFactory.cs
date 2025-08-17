using Core.DTOs.NotificationDTOs;
using Core.Enums;
using Core.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using TechpertsSolutions.Core.Entities;

namespace Core.Factories
{
    public static class NotificationsFactory
    {
        // Message templates with placeholders {0}, {1}, etc.
        private static readonly Dictionary<NotificationType, string> Templates = new()
        {
            // Order-related
            { NotificationType.OrderCreated, "Your order #{0} has been created successfully." },
            { NotificationType.OrderStatusChanged, "Order #{0} status has changed to {1}." },
            { NotificationType.OrderAssignedToDelivery, "Order #{0} has been assigned to a delivery person." },

            // Product-related
            { NotificationType.ProductAdded, "Your product '{0}' has been added and is pending approval." },
            { NotificationType.ProductApproved, "Your product '{0}' has been approved." },
            { NotificationType.ProductRejected, "Your product '{0}' has been rejected. Reason: {1}" },
            { NotificationType.ProductPending, "Your product '{0}' is still under review." },

            // Maintenance-related
            { NotificationType.MaintenanceRequestCreated, "Maintenance request #{0} has been created." },
            { NotificationType.MaintenanceRequestAccepted, "Maintenance request #{0} has been accepted by {1}." },
            { NotificationType.MaintenanceRequestCompleted, "Maintenance request #{0} has been completed." },

            // Delivery-related
            { NotificationType.DeliveryOfferCreated, "A new delivery offer for order #{0} has been created." },
            { NotificationType.DeliveryOfferAccepted, "Delivery offer #{0} has been accepted." },
            { NotificationType.DeliveryOfferExpired, "Delivery offer #{0} has expired." },
            { NotificationType.DeliveryOfferCanceled, "Delivery offer #{0} has been canceled." },
            { NotificationType.DeliveryOfferDeclined, "Delivery offer #{0} has been declined." },
            { NotificationType.DeliveryAssigned, "Delivery for order #{0} has been assigned to {1}." },
            { NotificationType.DeliveryPickedUp, "Delivery #{0} has been picked up by {1}." },
            { NotificationType.DeliveryCompleted, "Delivery #{0} has been completed successfully." },

            // System
            { NotificationType.SystemAlert, "System Alert: {0}" }
        };

        /// Creates a single notification entity.
        public static Notification Create(
            string userId,
            NotificationType type,
            string? relatedEntityId = null,
            string? relatedEntityType = null,
            params object[] args)
        {
            if (!Templates.TryGetValue(type, out var template))
                throw new ArgumentException($"No template defined for notification type {type}");

            string message = string.Format(template, args);

            return new Notification
            {
                ReceiverUserId = userId,
                Type = type,
                Message = message,
                RelatedEntityId = relatedEntityId,
                RelatedEntityType = relatedEntityType,
                IsRead = false,
                CreatedAt = DateTime.Now
            };
        }

        /// Creates notifications for a list of users.
        public static List<Notification> CreateForUsers(
            IEnumerable<string> userIds,
            NotificationType type,
            string? relatedEntityId = null,
            string? relatedEntityType = null,
            params object[] args)
        {
            if (userIds == null || !userIds.Any())
                return new List<Notification>();

            return userIds
                .Select(userId => Create(userId, type, relatedEntityId, relatedEntityType, args))
                .ToList();
        }

        /// Converts Notification entity to DTO.
        public static NotificationDTO ToDTO(Notification notification)
        {
            return new NotificationDTO
            {
                Id = notification.Id,
                ReceiverUserId = notification.ReceiverUserId,
                Message = notification.Message,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                RelatedEntityId = notification.RelatedEntityId,
                RelatedEntityType = notification.RelatedEntityType,
                ReceiverName = notification.Receiver?.UserName
            };
        }
    }
}


// ================ HOW TO USE ====================== //
// Single user
//var notif = NotificationsFactory.Create(
//    "user123",
//    NotificationType.OrderStatusChanged,
//    relatedEntityId: "101",
//    relatedEntityType: "Order",
//    args: new object[] { "101", "Shipped" }
//);

//// Multiple users
//var notifList = NotificationsFactory.CreateForUsers(
//    new List<string> { "user123", "user456" },
//    NotificationType.DeliveryAssigned,
//    relatedEntityId: "D-55",
//    relatedEntityType: "Delivery",
//    args: new object[] { "D-55", "Driver A" }
//);
// ================================================ //