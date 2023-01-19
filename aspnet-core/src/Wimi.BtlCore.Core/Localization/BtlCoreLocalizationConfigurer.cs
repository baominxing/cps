using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Localization.Sources;
using Abp.Reflection.Extensions;
using System.Reflection;

namespace Wimi.BtlCore.Localization
{
    public static class BtlCoreLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {

            localizationConfiguration.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo("AbpZero",
                new XmlEmbeddedFileLocalizationDictionaryProvider(
                    Assembly.GetExecutingAssembly(),
                    "Wimi.BtlCore.Localization.AbpZero"
                    ))
                );

            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(BtlCoreConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(BtlCoreLocalizationConfigurer).GetAssembly(),
                        "Wimi.BtlCore.Localization.SourceFiles"
                    )
                )
            );

            localizationConfiguration.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo("AbpWeb",
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                    Assembly.GetExecutingAssembly(),
                    "Wimi.BtlCore.Localization.AbpWeb"
                   )
                )
            );
        }
    }
}
