using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.UI;
using Abp.Web.Models;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Controllers.App.Dto;
using Wimi.BtlCore.Web.Helpers;

namespace Wimi.BtlCore.Web.Controllers
{
    public class UploadIocController : BtlCoreControllerBase
    {
        private readonly IBtlFolders appFolders;

        public UploadIocController(IBtlFolders appFolders)
        {
            this.appFolders = appFolders;
        }

        public JsonResult SaveCropImageToIco([FromBody]SaveCropImageToIcoDto input)
        {
            var imageFilePath = Path.Combine(this.appFolders.TempFileUploadFolder, input.ImageName);
            var guid = Guid.NewGuid();

            if(input.CropWidth==default || input.CropHeight ==default)
            {
                throw new UserFriendlyException("请重新选定裁剪区域!");
            }

            var imageByte = System.IO.File.ReadAllBytes(imageFilePath);
            using (var stream = new MemoryStream(imageByte))
            {
                using var factory = new ImageFactory();
                factory.Load(stream)
                       .Format(new PngFormat())
                       .Crop(new Rectangle(input.CropX, input.CropY, input.CropWidth, input.CropHeight));

                
                using var icon = HelperIcon.PngIconFromImage(factory.Image, 32);
                using var fs = new FileStream(
                        Path.Combine(this.appFolders.IcosFolder, $"{guid}.ico"),
                        FileMode.OpenOrCreate);
                icon.Save(fs);
            }
          
            return this.Json(new AjaxResponse() { Result = $"{guid}.ico" });
        }

        public async Task<JsonResult> Upload()
        {
            var newPathFileName = "";
            var newFileName = "";
            try
            {
                // Check input
                if (this.Request.Form.Files.Count <= 0 || this.Request.Form.Files[0] == null)
                {
                    throw new Exception(this.L("ProfilePicture_Change_Error"));
                }

                var file = this.Request.Form.Files[0];

                if (file.Length > 30720 * 1024)
                {
                    // 30MB.
                    throw new Exception(this.L("ProfilePicture_Warn_SizeLimit"));
                }

                if (!file.ContentType.Contains("image"))
                {
                    throw new Exception(this.L("ProfilePicture_Warn_FileType"));
                }
                var ext = Path.GetExtension(file.FileName);
                newFileName = DateTime.Now.ToFileTime().ToString() + ext;

                newPathFileName = Path.Combine(this.appFolders.TempFileUploadFolder, newFileName);

                using var stream = new FileStream(newPathFileName, FileMode.Create);
                file.CopyTo(stream);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(this.Json(new AjaxResponse() { Result = ex.Message, Success = false }));
            }

            return await Task.FromResult(this.Json(new AjaxResponse() { Result = newFileName, Success = true }));

        }

        public static class HelperIcon
        {
            private static readonly byte[] PngiconHeader = 
                {
                    0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0
                };

            public static Icon PngIconFromImage(Image img, int size = 16)
            {
                using (var bmp = new Bitmap(img, new Size(size, size)))
                {
                    byte[] png;
                    using (var fs = new MemoryStream())
                    {
                        bmp.Save(fs, ImageFormat.Png);
                        fs.Position = 0;
                        png = fs.ToArray();
                    }

                    using (var fs = new MemoryStream())
                    {
                        if (size >= 256)
                        {
                            size = 0;
                        }

                        PngiconHeader[6] = (byte)size;
                        PngiconHeader[7] = (byte)size;
                        PngiconHeader[14] = (byte)(png.Length & 255);
                        PngiconHeader[15] = (byte)(png.Length / 256);
                        PngiconHeader[18] = (byte)PngiconHeader.Length;
                        fs.Write(PngiconHeader, 0, PngiconHeader.Length);
                        fs.Write(png, 0, png.Length);
                        fs.Position = 0;
                        return new Icon(fs);
                    }
                }
            }
        }
    }
}