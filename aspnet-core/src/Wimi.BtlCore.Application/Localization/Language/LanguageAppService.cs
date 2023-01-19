using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Localization;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Localization;
using Wimi.BtlCore.Localization.Dto;

namespace WIMI.BTL.Localization
{
    [AbpAuthorize(PermissionNames.Pages_Administration_Languages)]
    public class LanguageAppService : BtlCoreAppServiceBase, ILanguageAppService
    {
        private readonly IApplicationLanguageManager applicationLanguageManager;

        private readonly IApplicationLanguageTextManager applicationLanguageTextManager;

        private readonly IRepository<ApplicationLanguage> languageRepository;

        public LanguageAppService(
            IApplicationLanguageManager applicationLanguageManager,
            IApplicationLanguageTextManager applicationLanguageTextManager,
            IRepository<ApplicationLanguage> languageRepository)
        {
            this.applicationLanguageManager = applicationLanguageManager;
            this.languageRepository = languageRepository;
            this.applicationLanguageTextManager = applicationLanguageTextManager;
        }

        public async Task CreateOrUpdateLanguage(CreateOrUpdateLanguageInputDto input)
        {
            if (input.Language.Id.HasValue)
            {
                await this.UpdateLanguageAsync(input);
            }
            else
            {
                await this.CreateLanguageAsync(input);
            }
        }

        public async Task DeleteLanguage(EntityDto input)
        {
            var language = await this.languageRepository.GetAsync(input.Id);
            await this.applicationLanguageManager.RemoveAsync(this.AbpSession.TenantId, language.Name);
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Languages_Create,
            PermissionNames.Pages_Administration_Languages_Edit)]
        [HttpPost]
        public async Task<GetLanguageForEditOutputDto> GetLanguageForEdit(NullableIdDto input)
        {
            ApplicationLanguage language = null;
            if (input.Id.HasValue)
            {
                language = await this.languageRepository.GetAsync(input.Id.Value);
            }

            var output = new GetLanguageForEditOutputDto();

            // Language
            output.Language = language != null
                                  ? ObjectMapper.Map<ApplicationLanguageEditDto>(language)
                                  : new ApplicationLanguageEditDto();

            // Language names
            output.LanguageNames =
                CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .OrderBy(c => c.DisplayName)
                    .Select(
                        c =>
                        new ComboboxItemDto(c.Name, c.DisplayName + " (" + c.Name + ")")
                        {
                            IsSelected =
                                    output.Language.Name
                                    == c.Name
                        })
                    .ToList();

            // Flags
            output.Flags =
                FamFamFamFlagsHelper.FlagClassNames.OrderBy(f => f)
                    .Select(
                        f =>
                        new ComboboxItemDto(f, FamFamFamFlagsHelper.GetCountryCode(f))
                        {
                            IsSelected =
                                    output.Language.Icon == f
                        })
                    .ToList();

            return output;
        }

        [HttpPost]
        public async Task<GetLanguagesOutputDto> GetLanguages()
        {
            var languages =
                (await this.applicationLanguageManager.GetLanguagesAsync(this.AbpSession.TenantId)).OrderBy(
                    l => l.DisplayName);
            var defaultLanguage =
                await this.applicationLanguageManager.GetDefaultLanguageOrNullAsync(this.AbpSession.TenantId);

            return new GetLanguagesOutputDto(
                ObjectMapper.Map<List<ApplicationLanguageListDto>>(languages),
                defaultLanguage == null ? null : defaultLanguage.Name);
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Languages_ChangeTexts)]
        [HttpPost]
        public async Task<PagedResultDto<LanguageTextListDto>> GetLanguageTexts(GetLanguageTextsInputDto input)
        {
            /* Note: This method is used by SPA without paging, MPA with paging.
             * So, it can both usable with paging or not */

            // Normalize base language name
            if (input.BaseLanguageName.IsNullOrEmpty())
            {
                var defaultLanguage =
                    await this.applicationLanguageManager.GetDefaultLanguageOrNullAsync(this.AbpSession.TenantId);
                if (defaultLanguage == null)
                {
                    defaultLanguage =
                        (await this.applicationLanguageManager.GetLanguagesAsync(this.AbpSession.TenantId))
                            .FirstOrDefault();
                    if (defaultLanguage == null)
                    {
                        throw new ApplicationException(this.L("NoLanguageFoundInTheApplication"));
                    }
                }

                input.BaseLanguageName = defaultLanguage.Name;
            }

            var source = this.LocalizationManager.GetSource(input.SourceName);
            var baseCulture = CultureInfo.GetCultureInfo(input.BaseLanguageName);
            var targetCulture = CultureInfo.GetCultureInfo(input.TargetLanguageName);

            var languageTexts =
                source.GetAllStrings()
                    .Select(
                        localizedString =>
                        new LanguageTextListDto
                        {
                            Key = localizedString.Name,
                            BaseValue =
                                    this.applicationLanguageTextManager.GetStringOrNull(
                                        this.AbpSession.TenantId,
                                        source.Name,
                                        baseCulture,
                                        localizedString.Name),
                            TargetValue =
                                    this.applicationLanguageTextManager.GetStringOrNull(
                                        this.AbpSession.TenantId,
                                        source.Name,
                                        targetCulture,
                                        localizedString.Name,
                                        false)
                        })
                    .AsQueryable();

            // Filters
            if (input.TargetValueFilter == "EMPTY")
            {
                languageTexts = languageTexts.Where(s => s.TargetValue.IsNullOrEmpty());
            }

            if (!input.FilterText.IsNullOrEmpty())
            {
                languageTexts =
                    languageTexts.Where(
                        l =>
                        (l.Key != null
                         && l.Key.IndexOf(input.FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        || (l.BaseValue != null
                            && l.BaseValue.IndexOf(input.FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        || (l.TargetValue != null
                            && l.TargetValue.IndexOf(input.FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0));
            }

            var totalCount = languageTexts.Count();

            // Ordering
            if (!input.Sorting.IsNullOrEmpty())
            {
                languageTexts = languageTexts.OrderBy(input.Sorting);
            }

            // Paging
            if (input.Start > 0)
            {
                languageTexts = languageTexts.Skip(input.Start);
            }

            if (input.Length > 0)
            {
                languageTexts = languageTexts.Take(input.Length);
            }

            return new DatatablesPagedResultOutput<LanguageTextListDto>(totalCount, languageTexts.ToList());
        }

        public async Task SetDefaultLanguage(SetDefaultLanguageInputDto input)
        {
            await
                this.applicationLanguageManager.SetDefaultLanguageAsync(
                    this.AbpSession.TenantId,
                    this.GetCultureInfoByChecking(input.Name).Name);
        }

        public async Task UpdateLanguageText(UpdateLanguageTextInputDto input)
        {
            var culture = this.GetCultureInfoByChecking(input.LanguageName);
            var source = this.LocalizationManager.GetSource(input.SourceName);
            await
                this.applicationLanguageTextManager.UpdateStringAsync(
                    this.AbpSession.TenantId,
                    source.Name,
                    culture,
                    input.Key,
                    input.Value);
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Languages_Create)]
        protected virtual async Task CreateLanguageAsync(CreateOrUpdateLanguageInputDto input)
        {
            var culture = this.GetCultureInfoByChecking(input.Language.Name);

            await this.CheckLanguageIfAlreadyExists(culture.Name);

            await
                this.applicationLanguageManager.AddAsync(
                    new ApplicationLanguage(
                        this.AbpSession.TenantId,
                        culture.Name,
                        culture.DisplayName,
                        input.Language.Icon));
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Languages_Edit)]
        protected virtual async Task UpdateLanguageAsync(CreateOrUpdateLanguageInputDto input)
        {
            Debug.Assert(input.Language.Id != null, "input.Language.Id != null");

            var culture = this.GetCultureInfoByChecking(input.Language.Name);

            await this.CheckLanguageIfAlreadyExists(culture.Name, input.Language.Id.Value);

            var language = await this.languageRepository.GetAsync(input.Language.Id.Value);

            language.Name = culture.Name;
            language.DisplayName = culture.DisplayName;
            language.Icon = input.Language.Icon;

            await this.applicationLanguageManager.UpdateAsync(this.AbpSession.TenantId, language);
        }

        private async Task CheckLanguageIfAlreadyExists(string languageName, int? expectedId = null)
        {
            var existingLanguage =
                (await this.applicationLanguageManager.GetLanguagesAsync(this.AbpSession.TenantId)).FirstOrDefault(
                    l => l.Name == languageName);

            if (existingLanguage == null)
            {
                return;
            }

            if (expectedId != null && existingLanguage.Id == expectedId.Value)
            {
                return;
            }

            throw new UserFriendlyException(this.L("ThisLanguageAlreadyExists"));
        }

        private CultureInfo GetCultureInfoByChecking(string name)
        {
            try
            {
                return CultureInfo.GetCultureInfo(name);
            }
            catch (CultureNotFoundException ex)
            {
                this.Logger.Warn(ex.ToString(), ex);
                throw new UserFriendlyException(this.L("InvlalidLanguageCode"));
            }
        }
    }
}