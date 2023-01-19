using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wimi.BtlCore.Carton.CartonSettings.Dtos;

namespace Wimi.BtlCore.Carton.CartonSettings
{
    public interface ICartonSettingAppService : IApplicationService
    {
        IEnumerable<NameValueDto> ListLocalPrinterName();

        IEnumerable<NameValueDto> ListCartonRule();

        IEnumerable<NameValueDto> ListTraceFlow(int deviceGroupId);

        Task<int> SaveCartonSetting(CartonSettingDto input);

        Task<CartonSettingDto> GetCartonSettingByDeviceGroup(int deviceGroupId);
    }
}
