using Abp.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Common.Dtos;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Localization.MultiLanguage;
using Wimi.BtlCore.Localization.MultiLanguage.Dtos;
using Wimi.BtlCore.Web.Models.Localization;

namespace Wimi.BtlCore.Web.Controllers.Localization
{
    public class MultiLanguageController : BtlCoreControllerBase
    {
        private IMultiLanguageAppService multiLanguageService;
        private IWebHostEnvironment webHostEnvironment;
        public MultiLanguageController(
            IMultiLanguageAppService multiLanguageService,
            IWebHostEnvironment webHostEnvironment)
        {
            this.multiLanguageService = multiLanguageService;
            this.webHostEnvironment = webHostEnvironment;
        }
        // GET: Language
        public ActionResult Index()
        {
            return this.View("~/Views/MultiLanguage/Index.cshtml");
        }

        [HttpPost]
        public async Task<string> DealLanguageFile(LanguageViewModel fromData)
        {
            string result = "OK";
            try
            {
                var file = Request.Form.Files.First();

                //Check input
                if (file == null)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
                }

                var laguageDto = ObjectMapper.Map<MultiLanguageInputDto>(fromData);

                if (fromData.TitleCheck.ToLower() == "on")
                {
                    laguageDto.IsCheck = true;
                }
                else
                {
                    laguageDto.IsCheck = false;
                }

                var dto = new MultiLanguageDto
                {
                    FilePath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot\\Language"),
                    LanguageInfo = laguageDto,
                };


                await multiLanguageService.DealLanguageFile(dto, file);

            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }
    }
}
