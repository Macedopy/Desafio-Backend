using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace DeliveryDrivers.Infrastructure
{
    public interface IDriverimageS3
    {
        Task SendFileToS3(IFormFile file);

    };
    public sealed class DriverimageS3 : IDriverimageS3
    {
        private readonly IAmazonS3 _s3Client;
        public async Task SendFileToS3(IFormFile file)
        {
            var request = new PutObjectRequest()
                {
                    BucketName = "mottu-driver-image",
                    Key = file.FileName,
                    InputStream = file.OpenReadStream()
                };

            request.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(request);
        }
    }
}