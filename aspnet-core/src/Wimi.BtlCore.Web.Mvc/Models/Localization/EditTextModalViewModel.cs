using Abp.Localization;

namespace Wimi.BtlCore.Web.Models.Localization
{
    public class EditTextModalViewModel
    {
        public LanguageInfo BaseLanguage { get; set; }

        public string BaseText { get; set; }

        public string Key { get; set; }

        public string SourceName { get; set; }

        public LanguageInfo TargetLanguage { get; set; }

        public string TargetText { get; set; }
    }
}
