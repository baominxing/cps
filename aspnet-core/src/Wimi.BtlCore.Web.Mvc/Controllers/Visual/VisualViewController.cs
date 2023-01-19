namespace Wimi.BtlCore.Web.Controllers.Visual
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Abp.Authorization;
    using Abp.Domain.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.Notices;
    using Wimi.BtlCore.Visual;
    using Wimi.BtlCore.Visual.Dto;
    using Wimi.BtlCore.Web.Models.Visual;

    [AbpAllowAnonymous]
    public class VisualViewController : BtlCoreControllerBase
    {
        private readonly IBtlFolders appFolders;

        private readonly string configName = "VisualDesign.json";

        private readonly IRepository<Notice> noticesRepository;

        private readonly IVisualAppService visualAppService;

        public VisualViewController(
            IVisualAppService visualAppService, 
            IBtlFolders appFolders, 
            IRepository<Notice> noticesRepository)
        {
            this.visualAppService = visualAppService;
            this.appFolders = appFolders;
            this.noticesRepository = noticesRepository;
        }

        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            Notice notices = null;
            if (id.HasValue)
            {
                notices = await this.noticesRepository.GetAsync(id.Value);
            }

            var model = new CreateOrEditNoticesViewModel()
                            {
                                IsEditModel = id.HasValue,
                                Id = id,
                                IsActive = notices == null || notices.IsActive,
                                Content = notices?.Content,
                                RootDeviceGroupCode = notices?.RootDeviceGroupCode,
                                WorkShopList = (await this.visualAppService.GetWorkShops()).ToList(),
                                YesNoModel = this.GetYesNoSelectListItems()
                            };
            return this.PartialView("~/Views/Visual/VisualNotice/_CreateOrEditModal.cshtml", model);

        }

        public ActionResult Index()
        {
            return this.View();
        }

        public JsonResult ReadConfigFile()
        {
            var dto = new ReadFileDto()
                          {
                              FileName = this.configName, 
                              FilePath = this.appFolders.ConfigurationsFloder
                          };
            var configData = this.visualAppService.ReadFile(dto);
            return this.Json(configData);
        }

        public JsonResult SetLogo1(string imgstr, string filename)
        {
            var stateInt = 0;
            if (!string.IsNullOrEmpty(imgstr))
            {
                try
                {
                    var imgByte = Encoding.Unicode.GetBytes(imgstr);
                    var image1 = this.BytesToImage(imgByte);
                    image1.Save($"{this.appFolders.VisualImgFloder}/{filename}", ImageFormat.Jpeg);
                }
                catch
                {
                    stateInt = 1;
                }
            }

            return this.Json(stateInt);
        }

        public JsonResult SetLogo2()
        {
            var result = 0;
            try
            {
                var httpPostedFileBase = this.Request.Form.Files[0];
                if (httpPostedFileBase != null)
                {
                    var file = httpPostedFileBase.FileName;
                    if (!Directory.Exists(this.appFolders.VisualImgFloder))
                    {
                        Directory.CreateDirectory(this.appFolders.VisualImgFloder.TrimEnd('/'));
                    }
                    
                    var path = $"{this.appFolders.VisualImgFloder}/{file}";
                    FileStream stream = new FileStream(path, FileMode.Create);
                    httpPostedFileBase.CopyTo(stream);
                }
            }
            catch
            {
                result = 1;
            }

            return this.Json(result);
        }

        public ActionResult VisualNotice()
        {
            return this.View("~/Views/Visual/VisualNotice/VisualNotice.cshtml");
        }

        public JsonResult WriteConfigFile(EntityDto<string> configInput)
        {
            var dto = new WriteFileDto()
                          {
                              FileName = this.configName, 
                              FilePath = this.appFolders.ConfigurationsFloder, 
                              Data = configInput.Id
                          };
            var result = this.visualAppService.WriteConfigFile(dto);
            return this.Json(result);
        }

        private Image BytesToImage(byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            Image image = new Bitmap(ms, true);
            return image;
        }
    }
}