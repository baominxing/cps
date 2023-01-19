using System.ComponentModel.DataAnnotations;

using Abp.AutoMapper;
using Abp.Localization;

namespace Wimi.BtlCore.Localization.Dto
{
    [AutoMapFrom(typeof(ApplicationLanguage))]
    public class ApplicationLanguageEditDto
    {
        [StringLength(ApplicationLanguage.MaxIconLength)]
        public virtual string Icon { get; set; }

        public virtual int? Id { get; set; }

        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public virtual string Name { get; set; }
    }
}