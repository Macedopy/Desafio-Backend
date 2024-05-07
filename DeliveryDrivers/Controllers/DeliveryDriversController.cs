using Amazon.S3.Model;
using DeliveryDrivers.Data;
using DeliveryDrivers.Infrastructure;
using DeliveryDrivers.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryDrivers.Controllers
{
    [ApiController]
    [Route("mottu/drivers")]
    public class DeliveryDriversController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDriverRepository _driverRepository;

        public DeliveryDriversController(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var getId = await _driverRepository.GetById(id, HttpContext.RequestAborted);
            return Ok(getId);
        }
        [HttpPost]
        public async Task<IActionResult> SaveDriverInformations(DriverModel driver, IFormFile file)
        {
            DriverimageS3 driverimageS3 = new DriverimageS3();
            try
                {
                    await driverimageS3.SendFileToS3(file);
                } catch(Exception ex)
                {
                    _logger.LogError("The Image CNH File have an error", ex.Message);
                }
            
            await _driverRepository.SaveDriverInformations(driver, HttpContext.RequestAborted);
            return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDriverInformations(DriverModel driver)
        {
            await _driverRepository.UpdateDriver(driver, HttpContext.RequestAborted);
            return Ok();
        }
    }
}