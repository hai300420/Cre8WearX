using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

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

        // Handle url
        //public async Task<string> UploadBase64ImageAsync(string base64, string fileName)
        //{
        //    byte[] imageBytes = Convert.FromBase64String(base64);
        //    using var stream = new MemoryStream(imageBytes);

        //    // Extract file extension (e.g., ".jpg")
        //    var extension = Path.GetExtension(fileName);

        //    // Create a unique filename using a timestamp and GUID
        //    var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{extension}";

        //    var uploadParams = new ImageUploadParams()
        //    {
        //        File = new FileDescription(uniqueFileName, stream),
        //        PublicId = $"products/{uniqueFileName}",
        //        Overwrite = true
        //    };

        //    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        //    if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        throw new Exception("Image upload failed: " + uploadResult.Error?.Message);
        //    }

        //    return uploadResult.SecureUrl.AbsoluteUri;
        //}

        // Handle base64
        public async Task<CloudinaryImageResult> UploadBase64ImageAsync(string base64, string fileName)
        {
            // Handle base64 with or without data URI scheme prefix
            var base64Data = base64;

            // If the string starts with "data:", strip the header
            if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                var base64Parts = base64.Split(',');
                if (base64Parts.Length != 2)
                {
                    throw new ArgumentException("Invalid base64 image format.");
                }
                base64Data = base64Parts[1];
            }

            // Convert base64 to bytes
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            using var stream = new MemoryStream(imageBytes);



            // Extract file extension (or default to ".png")
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                extension = ".png";
            }

            // Generate unique file name
            var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{extension}";
            var publicId = $"products/{uniqueFileName}";

            // Upload to Cloudinary
            var uploadParams = new ImageUploadParams
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

            return new CloudinaryImageResult
            {
                Url = uploadResult.SecureUrl?.AbsoluteUri ?? "",
                PublicId = uploadResult.PublicId
            };
        }

        public async Task<CloudinaryImageResult> UploadImageFromUrlAsync(string imageUrl, string fileName)
        {
            using var httpClient = new HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
            using var stream = new MemoryStream(imageBytes);

            // Create a unique filename
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{extension}";

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(uniqueFileName, stream),
                PublicId = $"products/{uniqueFileName}",
                Overwrite = true
            };
            var publicId = $"products/{uniqueFileName}";

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);


            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Image upload failed: " + uploadResult.Error?.Message);
            }

            // return uploadResult.SecureUrl.AbsoluteUri;
            return new CloudinaryImageResult
            {
                Url = uploadResult.SecureUrl?.AbsoluteUri ?? "",
                PublicId = uploadResult.PublicId
            };
        }

        public async Task<CloudinaryImageResult> UpdateImageAsync(string base64, string fileName, string? existingPublicId = null)
        {
            if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                var base64Parts = base64.Split(',');
                if (base64Parts.Length != 2)
                    throw new ArgumentException("Invalid base64 image format.");
                base64 = base64Parts[1];
            }

            byte[] imageBytes = Convert.FromBase64String(base64);
            using var stream = new MemoryStream(imageBytes);

            var publicId = existingPublicId ?? $"products/{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                PublicId = publicId,
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != HttpStatusCode.OK)
                throw new Exception("Image upload failed: " + uploadResult.Error?.Message);

            // return uploadResult.SecureUrl.AbsoluteUri;
            return new CloudinaryImageResult
            {
                Url = uploadResult.SecureUrl?.AbsoluteUri ?? "",
                PublicId = uploadResult.PublicId
            };
        }
        public class CloudinaryImageResult
        {
            public string Url { get; set; } = string.Empty;
            public string PublicId { get; set; } = string.Empty;
        }

    }
}
