using DeliveryDrivers.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MotorcycleRental.RabbitService;

namespace DeliveryDrivers.Data
{

    public interface IDriverRepository
    {
        Task SaveDriverInformations(DriverModel driver, CancellationToken cancellationToken);
        Task<bool> UpdateDriver(DriverModel driver, CancellationToken cancellationToken);
        Task<DriverModel> GetById(Guid id, CancellationToken cancellationToken);
    }

    public sealed class DriverRepository : IDriverRepository
    {
        private readonly IMongoCollection<DriverModel> _mongodb;
        private readonly IRabbitBusService _rabbitBus;
        public DriverRepository(IOptions<DeliveryDriverDatabaseSettings> configuration, IRabbitBusService rabbitBus)
        {
            var client = new MongoClient(configuration.Value.ConnectionString);
            var database = client.GetDatabase(configuration.Value.DatabaseName);
            _mongodb = database.GetCollection<DriverModel>(configuration.Value.CollectionNameDriver);
            _rabbitBus = rabbitBus;
        }

        public async Task<DriverModel> GetById(Guid id, CancellationToken cancellationToken)
        {
            // return await _mongodb.Find(Builders<DriverModel>.Filter.Eq("id", id)).FirstOrDefaultAsync();
            return await _mongodb.Find(d => d.Id == id).FirstOrDefaultAsync(cancellationToken);

        }

        public async Task SaveDriverInformations(DriverModel driver, CancellationToken cancellationToken)
        {
            var guid = Guid.NewGuid();
            driver.Id = guid;

            var existingDriver = await GetById(guid, cancellationToken);

            if (existingDriver != null && driver.Id.Equals(existingDriver.Id))
            {
                throw new Exception("Driver already exists");
            }
            await _mongodb.InsertOneAsync(driver, cancellationToken);
            _rabbitBus.Publish(driver, "driver-registered");

        }

        public async Task<bool> UpdateDriver(DriverModel driver, CancellationToken cancellationToken)
        {
            if (!driver.Id.Equals((await GetById(driver.Id, cancellationToken)).Id))
            {
                return false;
            }
            _mongodb.ReplaceOneAsync(x => x.Id == driver.Id, driver, cancellationToken: cancellationToken);
            _rabbitBus.Publish(driver, "driver-updated");
            return true;

        }
    }
}