using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.ThirdPartyService
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var cloudName = config["Cloudinary:CloudName"];
            var apiKey = config["Cloudinary:ApiKey"];
            var apiSecret = config["Cloudinary:ApiSecret"];

            Account account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadBase64ImageAsync(string base64, string fileName)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            using var stream = new MemoryStream(imageBytes);

            // Extract file extension (e.g., ".jpg")
            var extension = Path.GetExtension(fileName);

            // Create a unique filename using a timestamp and GUID
            var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{extension}";

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(uniqueFileName, stream),
                PublicId = $"products/{uniqueFileName}",
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Image upload failed: " + uploadResult.Error?.Message);
            }

            return uploadResult.SecureUrl.AbsoluteUri;
        }
    }
}
