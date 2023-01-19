using Abp.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Wimi.BtlCore.Localization.MultiLanguage.Dtos;

namespace Wimi.BtlCore.Localization.MultiLanguage
{
    public interface IMultiLanguageAppService : IApplicationService
    {
        Task DealLanguageFile(MultiLanguageDto languageData,IFormFile file);
    }
}
