using System.Collections.Generic;

using Abp.Localization;

namespace Wimi.BtlCore.Web.Models.Account
{
    public class LanguagesViewModel
    {
        public IReadOnlyList<LanguageInfo> AllLanguages { get; set; }

        public LanguageInfo CurrentLanguage { get; set; }
    }
}