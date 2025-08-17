using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechpertsSolutions.Core.Entities;

public class DeliveryReassignmentService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DeliveryReassignmentService> _logger;
    private readonly DeliveryAssignmentSettings _settings;

    public DeliveryReassignmentService(
        IServiceScopeFactory scopeFactory,
        ILogger<DeliveryReassignmentService> logger,
        IOptions<DeliveryAssignmentSettings> settings)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "DeliveryReassignmentService started with settings: Interval={IntervalSeconds}s, MaxRetries={MaxRetries}, RetryDelay={RetryDelaySeconds}s",
            _settings.CheckIntervalSeconds,
            _settings.MaxRetries,
            _settings.RetryDelaySeconds
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var clusterService = scope.ServiceProvider.GetRequiredService<IDeliveryClusterService>();
                var deliveryRepo = scope.ServiceProvider.GetRequiredService<IRepository<Delivery>>();
                var deliveryService = scope.ServiceProvider.GetRequiredService<IDeliveryService>();
                var deliveryPersonService = scope.ServiceProvider.GetRequiredService<IDeliveryPersonService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Get unassigned clusters
                var unassignedClustersResponse = await clusterService.GetUnassignedClustersAsync();
                if (!unassignedClustersResponse.Success || unassignedClustersResponse.Data == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_settings.CheckIntervalSeconds), stoppingToken);
                    continue;
                }

                foreach (var clusterDto in unassignedClustersResponse.Data)
                {
                    // Retry limits
                    if (clusterDto.RetryCount >= _settings.MaxRetries)
                    {
                        await notificationService.SendNotificationToRoleAsync(
                            "Admin",
                            NotificationType.SystemAlert,
                            clusterDto.Id,
                            "DeliveryCluster",
                            $"Cluster #{clusterDto.Id} could not be assigned after {_settings.MaxRetries} attempts."
                        );
                        continue;
                    }

                    if (clusterDto.LastRetryTime.HasValue && (DateTime.Now - clusterDto.LastRetryTime.Value).TotalSeconds < _settings.RetryDelaySeconds)
                        continue;

                    var availableDriversResponse = await deliveryPersonService.GetAvailableDeliveryPersonsAsync();
                    if (!availableDriversResponse.Success || availableDriversResponse.Data == null || !availableDriversResponse.Data.Any())
                    {
                        await notificationService.SendNotificationToRoleAsync(
                            "Admin",
                            NotificationType.SystemAlert,
                            clusterDto.Id,
                            "DeliveryCluster",
                            $"No available drivers for cluster #{clusterDto.Id}."
                        );
                        continue;
                    }

                    var chosenDriver = availableDriversResponse.Data.FirstOrDefault();
                    if (chosenDriver == null)
                        continue;

                    // Load full Delivery entity with includes
                    var deliveryEntity = await deliveryRepo.GetByIdWithIncludesAsync(
                        clusterDto.DeliveryId,
                        d => d.Offers,
                        d => d.TechCompanies,
                        d => d.SubDeliveries,
                        d => d.DeliveryPerson
                    );

                    if (deliveryEntity == null)
                    {
                        _logger.LogWarning("Delivery {DeliveryId} not found for cluster {ClusterId}", clusterDto.DeliveryId, clusterDto.Id);
                        continue;
                    }

                    // Auto assign driver (creates offers)
                    await deliveryService.AutoAssignDriverAsync(deliveryEntity, clusterDto.Id);

                    // Update retry info
                    clusterDto.RetryCount++;
                    clusterDto.LastRetryTime = DateTime.Now;
                    await clusterService.UpdateClusterAsync(clusterDto.Id, clusterDto);

                    // Notify Admin
                    await notificationService.SendNotificationToRoleAsync(
                        "Admin",
                        NotificationType.SystemAlert,
                        clusterDto.Id,
                        "DeliveryCluster",
                        $"Cluster #{clusterDto.Id} has been assigned to driver {chosenDriver.UserFullName}."
                    );

                    // Notify chosen driver
                    await notificationService.SendNotificationAsync(
                        chosenDriver.Id,
                        NotificationType.DeliveryAssigned,
                        clusterDto.Id,
                        "DeliveryCluster",
                        $"You have been assigned to delivery cluster #{clusterDto.Id}."
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cluster reassignment service");
            }

            await Task.Delay(TimeSpan.FromSeconds(_settings.CheckIntervalSeconds), stoppingToken);
        }
    }
}