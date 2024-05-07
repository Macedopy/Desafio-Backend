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
    public class MotorcycleConsumer : IConsumer<MotorCycleModel>
    {
        private readonly IMotorcycleService _motorcycleService;
        private readonly ILogger<MotorcycleConsumer> logger;

        public MotorcycleConsumer(IMotorcycleService motorcycleService, ILogger _logger)
        {
            _motorcycleService = motorcycleService;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<MotorCycleModel> context)
        {
            var message = context.Message;
            try
            {
                await _motorcycleService.RegisterMotorcycle(message, context.CancellationToken);

                logger.LogInformation($"Motorcycle {message.Plate} registered");
            } catch(Exception ex)
            {
                logger.LogError(ex, $"Error registering motorcycle {message.Plate}", message);
            }

        }
    }
}