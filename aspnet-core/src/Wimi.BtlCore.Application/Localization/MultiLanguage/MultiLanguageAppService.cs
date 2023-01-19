using Abp.Configuration;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Common.Dtos;
using Wimi.BtlCore.Localization.MultiLanguage.Dtos;

namespace Wimi.BtlCore.Localization.MultiLanguage
{
    public class MultiLanguageAppService : BtlCoreAppServiceBase, IMultiLanguageAppService
    {
        private readonly ISettingManager settingManager;
        private readonly ICommonRepository commonRepository;

        public MultiLanguageAppService(
            ISettingManager settingManager,
            ICommonRepository commonRepository)
        {
            this.settingManager = settingManager;
            this.commonRepository = commonRepository;
        }
 
        public static string BaseModule = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<localizationDictionary culture =\"@Culture\">\r\n<texts>\r\n@Content\r\n</texts>\r\n</localizationDictionary>";

        public async Task DealLanguageFile(MultiLanguageDto languageData, IFormFile file)
        {
            //添加语言记录
            WriteFileToLanguageFolder(languageData.FilePath, languageData.LanguageInfo, file);

            //写入语言包
            await commonRepository.WriteLanguageToDatabase(languageData.LanguageInfo);
        }

        private bool WriteFileToLanguageFolder(string filePath, MultiLanguageInputDto languageData, IFormFile file)
        {
            try
            {
                int sheetIndex = languageData.SheetIndex;
                int nameIndex = languageData.CodeIndex;
                int valueIndex = languageData.ValueIndex;
                var baseContent = BaseModule.Replace("@Culture", languageData.Culture);
                StringBuilder contentBuilder = new StringBuilder();
                using (ExcelPackage package = new ExcelPackage(file.OpenReadStream()))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets[sheetIndex - 1];
                    var rowStart = sheet.Dimension.Start.Row;
                    if (languageData.IsCheck)
                    {
                        rowStart += 1;
                    }
                    var rowEnd = sheet.Dimension.End.Row;
                    for (int i = rowStart; i < rowEnd; i++)
                    {
                        var translateValue = sheet.GetValue(i, valueIndex)?.ToString();
                        if (!string.IsNullOrEmpty(translateValue))
                        {
                            translateValue = translateValue.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;");
                        }
                        contentBuilder.AppendFormat("<text name=\"{0}\">{1}</text>\r\n", sheet.GetValue(i, nameIndex), translateValue);
                    }
                }
                baseContent = baseContent.Replace("@Content", contentBuilder.ToString());
                var exportPath = Path.Combine(filePath, "BtlCore-" + languageData.Culture + ".xml");
                File.WriteAllText(exportPath, baseContent);
                return true;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("写入语言包文件失败,具体信息:", ex.ToString());

            }
        }
    }
}
