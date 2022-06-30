using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PosStoneNfce.API.Portal.App.Common.Extensions
{
    public static class ImageExtension
    {
        public static string FitImage(string imageBase64, int width, int height)
        {
            var image = Base64ToImage(imageBase64);
            image = ResizeImage(image, new Size(width, height));
            
            var imageBytes = ImageToArrayBytes(image, ImageFormat.Png);            
            return Convert.ToBase64String(imageBytes);
        }

        private static Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);

            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        private static Image ResizeImage(Image imgToResize, Size size)
        {
            return (Image)new Bitmap(imgToResize, size);
        }

        private static byte[] ImageToArrayBytes(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                return imageBytes;                
            }
        }
    }
}
