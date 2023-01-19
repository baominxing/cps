using System.IO;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Web.Controllers.App
{
    public class FileController : BtlCoreControllerBase
    {
        private readonly IBtlFolders appFolders;

        public FileController(IBtlFolders appFolders)
        {
            this.appFolders = appFolders;
        }

        [AbpMvcAuthorize]
        [DisableAuditing]
        public ActionResult DownloadTempFile(FileDto file)
        {
            this.CheckModelState();

            var filePath = Path.Combine(this.appFolders.TempFileDownloadFolder, file.FileToken);
            if (!System.IO.File.Exists(filePath))
            {
                throw new UserFriendlyException(this.L("RequestedFileDoesNotExists"));
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return this.File(fileBytes, file.FileType, file.FileName);
        }
    }
}
