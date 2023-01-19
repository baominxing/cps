using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Abp.Authorization;
using Abp.IO;
using Abp.Web.Security.AntiForgery;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.AppSystem.IO;

namespace Wimi.BtlCore.Web.Controllers.Visual
{
    [AbpAllowAnonymous]
    public class VisualSettingController : BtlCoreControllerBase
    {
        private readonly IBtlFolders appFolders;

        public VisualSettingController(IBtlFolders appFolders)
        {
            this.appFolders = appFolders;
        }

        [HttpPost]
        [DisableAbpAntiForgeryTokenValidation]
        public JsonResult UploadPicture()
        {
            var data = new { Data = this.L("ChooseFiles") };
            var json = new JsonResult(data);
            if (this.Request == null || this.Request.Form.Files.Count == 0) return json;

            var file = this.Request.Form.Files[0];
            if (file == null || file.FileName.Length == 0)
            {
                return json;
            }

            // Check file type & format
            var fileImage = Image.FromStream(file.OpenReadStream());
            if (!fileImage.RawFormat.Equals(ImageFormat.Jpeg) && !fileImage.RawFormat.Equals(ImageFormat.Png) && !fileImage.RawFormat.Equals(ImageFormat.Gif))
            {
                data = new { Data = this.L("FileTypeError", file.FileName.Split('.').Last()) };
                json = new JsonResult(data);
                return json;
            }

            var newFileName = $"{DateTime.Now:yyyyMMddHHmmss}.{file.FileName.Split('.').Last()}";
            AppFileHelper.DeleteFilesInFolderIfExists(this.appFolders.VisualImgFloder, newFileName);
            var tempFilePath = Path.Combine(this.appFolders.VisualImgFloder, newFileName);
            var directory = new DirectoryInfo(this.appFolders.VisualImgFloder);
            var tempImgFiles = directory.GetFiles(newFileName).ToList();
            FileStream stream;
            try
            {
                if (!System.IO.File.Exists(tempFilePath))
                {
                    stream = new FileStream(tempFilePath, FileMode.Create);
                    file.CopyTo(stream);
                    stream.Close();
                }
                else
                {
                    foreach (var tempImgFile in tempImgFiles)
                    {
                        FileHelper.DeleteIfExists(tempImgFile.FullName);
                    }
                    stream = new FileStream(tempFilePath, FileMode.Create);
                    file.CopyTo(stream);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                data = new { Data = ex.Message };
                json = new JsonResult(data);
                return json;
            }

            var boolData = new { Data = true };
            json = new JsonResult(boolData);
            return json;
        }
    }
}