using System;
using System.Threading.Tasks;
using Abp;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.Friendships;
using Wimi.BtlCore.AppSystem.Net.MimeTypes;
using Wimi.BtlCore.Storage;
using Wimi.BtlCore;
using Wimi.BtlCore.Authorization.Users.Profile;
using Abp.UI;
using Abp.Web.Models;
using System.IO;
using Abp.IO.Extensions;
using Wimi.BtlCore.Authorization.Users.Profile.Dto;
using Wimi.BtlCore.Dto;
using System.Linq;
using Wimi.BtlCore.Web.Helpers;
using System.Drawing.Imaging;
using System.Drawing;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Timing.Dto;
using Abp.Configuration;
using Wimi.BtlCore.Web.Models.App.Profile;
using Wimi.BtlCore.Timing;

namespace Wimi.BtlCore.Web.Controllers
{
    [DisableAuditing]
    public class ProfileController : BtlCoreControllerBase
    {
        private readonly IBtlFolders appFolders;
        private readonly IBinaryObjectManager binaryObjectManager;
        private readonly IProfileAppService profileAppService;
        private readonly ITimingAppService timingAppService;
        private readonly UserManager userManager;
        private readonly IFriendshipManager friendshipManager;
        private readonly ITempFileCacheManager tempFileCacheManager;

        private const int MaxProfilePictureSize = 5242880; //5MB
        public ProfileController(
            IBtlFolders appFolders,
                IBinaryObjectManager binaryObjectManager,
                IProfileAppService profileAppService,
                ITimingAppService timingAppService,
                UserManager userManager,
                IFriendshipManager friendshipManager,
                ITempFileCacheManager tempFileCacheManager
            )
        {
            this.appFolders = appFolders;
            this.binaryObjectManager = binaryObjectManager;
            this.profileAppService = profileAppService;
            this.timingAppService = timingAppService;
            this.userManager = userManager;
            this.friendshipManager = friendshipManager;
            this.tempFileCacheManager = tempFileCacheManager;
        }

        public PartialViewResult ChangePasswordModal()
        {
            return this.PartialView("Views/App/Profile/_ChangePasswordModal.cshtml");
        }

        public PartialViewResult ChangePictureModal()
        {
            return this.PartialView("Views/App/Profile/_ChangePictureModal.cshtml");
        }

        [UnitOfWork]
        public virtual async Task<JsonResult> ChangeProfilePicture(FileDto input)
        {
            try
            {
                var profilePictureFile = Request.Form.Files.First();

                //Check input
                if (profilePictureFile == null)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
                }

                if (profilePictureFile.Length > MaxProfilePictureSize)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit"));
                }

                byte[] fileBytes;
                using (var stream = profilePictureFile.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                if (!ImageFormatHelper.GetRawImageFormat(fileBytes).IsIn(ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif))
                {
                    throw new Exception(L("IncorrectImageFormat"));
                }

                // Get user
                var user = await this.userManager.GetUserByIdAsync(this.AbpSession.GetUserId());

                // Delete old picture
                if (user.ProfilePictureId.HasValue)
                {
                    await this.binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);
                }

                // Save new picture
                var storedFile = new BinaryObject(this.AbpSession.TenantId, fileBytes);
                await this.binaryObjectManager.SaveAsync(storedFile);

                // Update new picture on the user
                user.ProfilePictureId = storedFile.Id;

                // Return success
                return this.Json(new AjaxResponse());
            }
            catch (UserFriendlyException ex)
            {
                // Return error message
                return this.Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [DisableAuditing]
        [HttpGet, HttpPost]
        public async Task<FileResult> GetProfilePicture()
        {
            var user = await this.userManager.GetUserByIdAsync(AbpSession.GetUserId());
            if (user.ProfilePictureId == null)
            {
                return GetDefaultProfilePicture();
            }

            return await GetProfilePictureById(user.ProfilePictureId.Value);
        }

        [DisableAuditing]
        public async Task<FileResult> GetProfilePictureById(string id = "")
        {
            if (id.IsNullOrEmpty())
            {
                return GetDefaultProfilePicture();
            }

            return await GetProfilePictureById(Guid.Parse(id));
        }

        public PartialViewResult LinkAccountModal()
        {
            return this.PartialView("Views/App/Profile/_LinkAccountModal.cshtml");
        }

        public PartialViewResult LinkedAccountsModal()
        {
            return this.PartialView("Views/App/Profile/_LinkedAccountsModal.cshtml");
        }

        public async Task<PartialViewResult> MySettingsModal()
        {
            var output = await this.profileAppService.GetCurrentUserProfileForEdit();
            var timezoneItems =
                await
                this.timingAppService.GetEditionComboboxItems(
                    new GetTimezoneComboboxItemsInputDto
                    {
                        DefaultTimezoneScope = SettingScopes.User,
                        SelectedTimezoneId = output.Timezone
                    });

            var viewModel = new MySettingsViewModel(output) { TimezoneItems = timezoneItems };

            return this.PartialView("Views/App/Profile/_MySettingsModal.cshtml", viewModel);
        }

        [UnitOfWork]
        public virtual async Task<FileResult> GetFriendProfilePictureById(long userId, int? tenantId, string id = "")
        {
            if (id.IsNullOrEmpty() ||
                await friendshipManager.GetFriendshipOrNullAsync(AbpSession.ToUserIdentifier(), new UserIdentifier(tenantId, userId)) == null)
            {
                return GetDefaultProfilePicture();
            }

            using (CurrentUnitOfWork.SetTenantId(tenantId))
            {
                return await GetProfilePictureById(Guid.Parse(id));
            }
        }

        public JsonResult UploadProfilePicture()
        {
            try
            {
                var profilePictureFile = Request.Form.Files.First();

                //Check input
                if (profilePictureFile == null)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
                }

                if (profilePictureFile.Length > MaxProfilePictureSize)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit"));
                }

                byte[] fileBytes;
                using (var stream = profilePictureFile.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                if (!ImageFormatHelper.GetRawImageFormat(fileBytes).IsIn(ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif))
                {
                    throw new Exception(L("IncorrectImageFormat"));
                }


                var fileToken = Guid.NewGuid().ToString("N");
                var fileInfo = new FileInfo(profilePictureFile.FileName);
                var tempFileName = fileToken + fileInfo.Extension;
                var tempFilePath = Path.Combine(appFolders.TempFileDownloadFolder, tempFileName);
                System.IO.File.WriteAllBytes(tempFilePath, fileBytes);
                this.tempFileCacheManager.SetFile(fileToken, fileBytes);

                using (var bmpImage = new Bitmap(tempFilePath))
                {
                    return this.Json(new UploadProfilePictureOutput
                    {
                        FileType = fileInfo.Extension,
                        FileToken = fileToken,
                        FileName = tempFileName,
                        Width = bmpImage.Width,
                        Height = bmpImage.Height
                    });
                }
            }
            catch (UserFriendlyException ex)
            {
                return this.Json(new UploadProfilePictureOutput(new ErrorInfo(ex.Message)));
            }
        }

        private FileResult GetDefaultProfilePicture()
        {
            return File(@"~/Content/Images/default-profile-picture.png", MimeTypeNames.ImagePng);
        }

        private async Task<FileResult> GetProfilePictureById(Guid profilePictureId)
        {
            var file = await binaryObjectManager.GetOrNullAsync(profilePictureId);

            if (file == null)
            {
                return GetDefaultProfilePicture();
            }

            return File(file.Bytes, MimeTypeNames.ImageJpeg);
        }
    }
}