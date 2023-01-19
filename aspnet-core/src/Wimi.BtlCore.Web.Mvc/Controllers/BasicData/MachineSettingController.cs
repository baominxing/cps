using Abp.AspNetCore.Mvc.Authorization;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.AppSystem.IO;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData;
using Wimi.BtlCore.BasicData.Dto;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    [AbpMvcAuthorize(PermissionNames.Pages_BasicData)]
    public class MachineSettingController : BtlCoreControllerBase
    {
        private readonly IBtlFolders appFolders;

        private readonly IBasicDataAppService baseDataAppService;

        public MachineSettingController(
            IBasicDataAppService baseDataAppService,
            IBtlFolders appFolders)
        {
            this.baseDataAppService = baseDataAppService;
            this.appFolders = appFolders;
        }

        [HttpPost]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetMachineSetting(MachineSettingInputDto input)
        {
            var returnJson = await this.baseDataAppService.GetMachineSetting(input);
            return this.Json(returnJson);
        }

        // GET: Machines
        public ActionResult Index()
        {
            this.ViewBag.FilterText = this.Request.Query["filterText"];
            return this.View("~/Views/BasicData/MachineSetting/Index.cshtml");
        }

        public PartialViewResult PingTest()
        {
            return this.PartialView("~/Views/BasicData/MachineSetting/_PingTest.cshtml");
        }

        public PartialViewResult TelnetTest()
        {
            return this.PartialView("~/Views/BasicData/MachineSetting/_TelnetTest.cshtml");
        }

        [HttpPost]
        public JsonResult UploadMachineImage()
        {
            var json = new JsonResult(true);
            var fileNameWithoutExtension = Guid.NewGuid().ToString();
            var fileExtension = ".jpg";
            if (this.Request != null)
            {
                var imageId = this.Request.Query["imageId"];
                var file = this.Request.Form.Files["UploadedFile"];
                if (file == null || file.FileName.Length == 0)
                {
                    json.Value = this.L("SelectFile");
                    return json;
                }

                this.baseDataAppService.CheckImageSize(file.ContentType.Length);

                // Check file type & format
                using var fileImage = Image.FromStream(file.OpenReadStream());
                if (!fileImage.RawFormat.Equals(ImageFormat.Jpeg) && !fileImage.RawFormat.Equals(ImageFormat.Png))
                {
                    json.Value = this.L("FileTypeError", file.FileName.Split('.').Last());
                    return json;
                }

                // 更新数据
                // Delete old temp profile pictures
                AppFileHelper.DeleteFilesInFolderIfExists(this.appFolders.TempFileDownloadFolder, imageId);

                // Save new picture
                var tempFileName = fileNameWithoutExtension + fileExtension;
                var tempFilePath = Path.Combine(this.appFolders.TempFileDownloadFolder, tempFileName);
                try
                {
                    //file.SaveAs(tempFilePath);
                    using var stream = new FileStream(tempFilePath, FileMode.Create);
                    file.CopyTo(stream);
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }
                return this.Json(new AjaxResponse(new { fileName = tempFileName }));
                
            }

            return json;
        }
    }
}