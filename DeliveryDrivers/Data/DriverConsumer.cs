using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryDrivers.Models;
using MassTransit;
using MotorcycleRental.Data;
using RabbitMQ.Client;

namespace DeliveryDrivers.Data
{
    public class DriverConsumer : IConsumer<DriverModel>
    {
        private readonly IDriverRepository _driverRepository;
        private readonly ILogger<MotorcycleConsumer> logger;

        public DriverConsumer(IDriverRepository driverRepository, ILogger _logger)
        {
            _driverRepository = driverRepository;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<DriverModel> context)
        {
            var message = context.Message;
            try
            {
                await _driverRepository.SaveDriverInformations(message, context.CancellationToken);

                logger.LogInformation($"Driver {message.Name} registered");
            } catch(Exception ex)
            {
                logger.LogError(ex, $"Error registering Driver {message.Name}", message);
            }

        }
    }
}