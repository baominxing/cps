using System;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Localization;
using Abp.AspNetCore.Mvc.Authorization;
using Wimi.BtlCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Wimi.BtlCore.Localization;
using Wimi.BtlCore.Web.Models.Localization;

namespace Wimi.BtlCore.Web.Controllers.Localization
{
    [AbpMvcAuthorize(PermissionNames.Pages_Administration_Languages)]
    public class LanguagesController : BtlCoreControllerBase
    {
        private readonly IApplicationLanguageTextManager applicationLanguageTextManager;

        private readonly ILanguageAppService languageAppService;

        private readonly ILanguageManager languageManager;

        public LanguagesController(
            ILanguageAppService languageAppService,
            ILanguageManager languageManager,
            IApplicationLanguageTextManager applicationLanguageTextManager)
        {
            this.languageAppService = languageAppService;
            this.languageManager = languageManager;
            this.applicationLanguageTextManager = applicationLanguageTextManager;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_Languages_Create,
            PermissionNames.Pages_Administration_Languages_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            var output = await this.languageAppService.GetLanguageForEdit(new NullableIdDto { Id = id });
            var viewModel = new CreateOrEditLanguageModalViewModel(output);

            return this.PartialView("_CreateOrEditModal", viewModel);
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_Languages_ChangeTexts)]
        public PartialViewResult EditTextModal(
            string sourceName,
            string baseLanguageName,
            string languageName,
            string key)
        {
            var languages = this.languageManager.GetLanguages();

            var baselanguage = languages.FirstOrDefault(l => l.Name == baseLanguageName);
            if (baselanguage == null)
            {
                throw new ApplicationException("Could not find language: " + baseLanguageName);
            }

            var targetLanguage = languages.FirstOrDefault(l => l.Name == languageName);
            if (targetLanguage == null)
            {
                throw new ApplicationException("Could not find language: " + languageName);
            }

            var baseText = this.applicationLanguageTextManager.GetStringOrNull(
                this.AbpSession.TenantId,
                sourceName,
                CultureInfo.GetCultureInfo(baseLanguageName),
                key);

            var targetText = this.applicationLanguageTextManager.GetStringOrNull(
                this.AbpSession.TenantId,
                sourceName,
                CultureInfo.GetCultureInfo(languageName),
                key,
                false);

            var viewModel = new EditTextModalViewModel
            {
                SourceName = sourceName,
                BaseLanguage = baselanguage,
                TargetLanguage = targetLanguage,
                BaseText = baseText,
                TargetText = targetText,
                Key = key
            };

            return this.PartialView("_EditTextModal", viewModel);
        }

        public async Task<JsonResult> GetLanguageTexts(GetLanguageTextsInputDto input)
        {
            return this.Json(await this.languageAppService.GetLanguageTexts(input));
        }

        public ActionResult Index()
        {
            var viewModel = new LanguagesIndexViewModel { IsTenantView = this.AbpSession.TenantId.HasValue };

            return this.View(viewModel);
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_Languages_ChangeTexts)]
        public ActionResult Texts(
            string languageName,
            string sourceName = "",
            string baseLanguageName = "",
            string targetValueFilter = "ALL",
            string filterText = "")
        {
            // Normalize arguments
            if (sourceName.IsNullOrEmpty())
            {
                sourceName = "AbpZeroTemplate";
            }

            if (baseLanguageName.IsNullOrEmpty())
            {
                baseLanguageName = this.languageManager.CurrentLanguage.Name;
            }

            // Create view model
            var viewModel = new LanguageTextsViewModel();

            viewModel.LanguageName = languageName;

            viewModel.Languages = this.languageManager.GetLanguages().ToList();

            viewModel.Sources =
                this.LocalizationManager.GetAllSources()
                    .Where(s => s.GetType() == typeof(MultiTenantLocalizationSource))
                    .Select(s => new SelectListItem { Value = s.Name, Text = s.Name, Selected = s.Name == sourceName })
                    .ToList();

            viewModel.BaseLanguageName = baseLanguageName;

            viewModel.TargetValueFilter = targetValueFilter;
            viewModel.FilterText = filterText;

            return this.View(viewModel);
        }
    }
}
