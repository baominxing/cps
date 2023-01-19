using Abp.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Wimi.BtlCore.Web.Models.Localization
{
    public class LanguageTextsViewModel
    {
        public string BaseLanguageName { get; set; }

        public string FilterText { get; set; }

        public string LanguageName { get; set; }

        public List<LanguageInfo> Languages { get; set; }

        public List<SelectListItem> Sources { get; set; }

        public string TargetValueFilter { get; set; }
    }
}
