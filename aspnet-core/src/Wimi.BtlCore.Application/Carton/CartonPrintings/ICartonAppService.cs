using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wimi.BtlCore.Carton.CartonPrintings.Dtos;

namespace Wimi.BtlCore.Carton.CartonPrintings
{
    public interface ICartonAppService : IApplicationService
    {
        Task<GetCartonSettingOutputDto> GetCartonSettingByParNo(EntityDto<string> input);

        Task<PackingOutputDto> Packing(PackingInputDto input);

        Task<PagedResultDto<ListCartonRecordsOutputDto>> ListCartonRecords(ListCartonRecordsInputDto input);

        Task<GetCartonRecordOutputDto> GetCartonRecord(EntityDto input);

        Task<GetCartonAndSettingByCartonNoOutputDto> GetCartonAndSettingByCartonNo(EntityDto<string> input);

        Task UpdateMaxCount(UpdateMaxCountInputDto input);

        Task FinalInspec(FinalInspecInputDto input);

        List<string> GetInstalledPrinters();

        Task Print(PrintInputDto input);
    }
}
