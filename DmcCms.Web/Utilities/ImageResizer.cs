using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web
{
    public class ImageResizer
    {
        public static ImageInfo MapAndResizeImageIfNeeded(string directoryName, string imageName, int width, int height)
        {
            string cacheKey = GetCacheKey(directoryName, imageName, width, height);
            ImageInfo cached = HttpRuntime.Cache.Get(cacheKey) as ImageInfo;

            if (cached != null)
            {
                return cached;
            }

            string imageFullPath = Path.Combine(directoryName, imageName)
                .ToLower(); // ugly hack because rewriting is messed up.

            if (!File.Exists(imageFullPath))
            {
                return null;
            }

            // Image info should not contain full path.

            ImageInfo result = null;
            Image image = null;
            Image resized = null;
            try
            {
                string destinationFullPath = Path.Combine(directoryName, GetDestinationFileName(imageName, width, height))
                    .ToLower();

                image = Image.FromFile(imageFullPath);
                
                if (File.Exists(destinationFullPath)) // we do this because it's highly likely that we already processed this image and it physicaly exists, just the cache expired.
                {
                    result = new ImageInfo
                    {
                        FileType = GetFileType(image.RawFormat),
                        FullPath = destinationFullPath
                    };
                    AddResultToCache(cacheKey, result);
                    return result;
                }
                
                resized = FixedSize(image, width, height);

                var encoder = ImageCodecInfo.GetImageEncoders()
                    .First(c => c.FormatID == image.RawFormat.Guid);

                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                resized.Save(destinationFullPath, encoder, encoderParameters);

                result = new ImageInfo
                {
                    FileType = GetFileType(image.RawFormat),
                    FullPath = destinationFullPath,
                    Height = resized.Height,
                    Width = resized.Width
                };
            }
            catch(Exception) // nothing for now. todo. log
            {
                return null;
            }
            finally
            {
                if (image != null)
                {
                    image.Dispose();
                }

                if (resized != null)
                {
                    resized.Dispose();
                }
            }

            if (result != null)
            {
                AddResultToCache(cacheKey, result);
            }

            return result;
        }

        private static void AddResultToCache(string cacheKey, ImageInfo result)
        {
            HttpRuntime.Cache.Add(cacheKey
                                , result
                                , null
                                , System.Web.Caching.Cache.NoAbsoluteExpiration
                                , System.Web.Caching.Cache.NoSlidingExpiration
                                , System.Web.Caching.CacheItemPriority.Low
                                , null);
        }

        private static string GetFileType(ImageFormat rawFormat)
        {
            if (ImageFormat.Jpeg.Equals(rawFormat))
            {
                return "image/jpeg";
            }
            else if (ImageFormat.Png.Equals(rawFormat))
            {
                return "image/png";
            }
            else if (ImageFormat.Gif.Equals(rawFormat))
            {
                return "image/gif";
            }

            return null;
        }

        private static string GetCacheKey(string directoryName, string imageName, int width, int height)
        {
            return string.Concat("ImageResizer_ImageInfo_", directoryName, imageName, width, height);
        }

        private static string GetDestinationFileName(string imageName, int width, int height)
        {
            string extension = Path.GetExtension(imageName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
            return string.Format("{0}{1}X{2}.{3}"
                , fileNameWithoutExtension
                , width
                , height
                , extension);
        }

        private static Image FixedSize(Image imgPhoto, int width, int height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)width / (float)sourceWidth);
            nPercentH = ((float)height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(width, height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
    }
}