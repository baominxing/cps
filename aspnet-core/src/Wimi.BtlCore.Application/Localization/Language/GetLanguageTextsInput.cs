using System.ComponentModel.DataAnnotations;

using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Localization;
using Abp.Runtime.Validation;

using Wimi.BtlCore.Authorization.Users.Dto;

namespace Wimi.BtlCore.Localization
{
    public class GetLanguageTextsInputDto : IDatatablesPagedResultRequest, ISortedResultRequest, IShouldNormalize
    {
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public string BaseLanguageName { get; set; }

        public string FilterText { get; set; }

        [Range(0, int.MaxValue)]
        public int Length { get; set; } // 0: Unlimited.

        public int MaxResultCount { get; set; }

        public int SkipCount { get; set; }

        public string Sorting { get; set; }

        [Required]
        [MaxLength(ApplicationLanguageText.MaxSourceNameLength)]
        public string SourceName { get; set; }

        [Range(0, int.MaxValue)]
        public int Start { get; set; }

        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength, MinimumLength = 2)]
        public string TargetLanguageName { get; set; }

        public string TargetValueFilter { get; set; }

        public void Normalize()
        {
            if (this.TargetValueFilter.IsNullOrEmpty())
            {
                this.TargetValueFilter = "ALL";
            }
        }
    }
}