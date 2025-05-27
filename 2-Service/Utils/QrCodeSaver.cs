using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.Utils
{
    public static class QrCodeSaver
    {
        /// <summary>
        /// Saves a Base64 QR code image to the specified file path.
        /// </summary>
        /// <param name="base64Image">Base64 string of the image without data prefix.</param>
        /// <param name="fileName">File name (e.g., "qr_code.png").</param>
        /// <returns>Relative URL to the saved image.</returns>
        public static async Task<string> SaveQrImageAsync(string base64Image, string fileName)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
                throw new ArgumentException("QR image data is null or empty.");

            byte[] imageBytes = Convert.FromBase64String(base64Image);

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            // Ensure directory exists
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, fileName);

            await File.WriteAllBytesAsync(filePath, imageBytes);

            // Return relative URL
            return $"/images/{fileName}";
        }
    }

}
