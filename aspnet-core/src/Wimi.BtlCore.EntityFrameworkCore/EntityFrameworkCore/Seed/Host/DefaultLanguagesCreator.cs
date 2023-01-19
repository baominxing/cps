using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Wimi.BtlCore.EntityFrameworkCore.Seed.Host
{
    public class DefaultLanguagesCreator
    {
        public static List<ApplicationLanguage> InitialLanguages => GetInitialLanguages();

        private readonly BtlCoreDbContext _context;

        private static List<ApplicationLanguage> GetInitialLanguages()
        {
            var tenantId = BtlCoreConsts.MultiTenancyEnabled ? null : (int?)MultiTenancyConsts.DefaultTenantId;

            return new List<ApplicationLanguage>
            {
                new ApplicationLanguage(tenantId, "en", "English", "famfamfam-flag-gb"),
                new ApplicationLanguage(tenantId, "zh-Hans", "简体中文", "famfamfam-flag-cn"),
                //new ApplicationLanguage(tenantId, "ar", "العربية", "famfamfam-flag sa"),
                //new ApplicationLanguage(tenantId, "de", "German", "famfamfam-flag de"),
                //new ApplicationLanguage(tenantId, "it", "Italiano", "famfamfam-flag it"),
                //new ApplicationLanguage(tenantId, "fr", "Français", "famfamfam-flag fr"),
                //new ApplicationLanguage(tenantId, "pt-BR", "Português", "famfamfam-flag br"),
                //new ApplicationLanguage(tenantId, "tr", "Türkçe", "famfamfam-flag tr"),
                //new ApplicationLanguage(tenantId, "ru", "Русский", "famfamfam-flag ru"),
                //new ApplicationLanguage(tenantId, "es-MX", "Español México", "famfamfam-flag mx"),
                //new ApplicationLanguage(tenantId, "nl", "Nederlands", "famfamfam-flag nl"),
                new ApplicationLanguage(tenantId, "ja", "日本語", "famfamfam-flag-jp")
            };
        }

        public DefaultLanguagesCreator(BtlCoreDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateLanguages();
        }

        private void CreateLanguages()
        {
            foreach (var language in InitialLanguages)
            {
                AddLanguageIfNotExists(language);
            }
        }

        private void AddLanguageIfNotExists(ApplicationLanguage language)
        {
            if (_context.Languages.IgnoreQueryFilters().Any(l => l.TenantId == language.TenantId && l.Name == language.Name))
            {
                return;
            }

            _context.Languages.Add(language);
            _context.SaveChanges();
        }
    }
}
