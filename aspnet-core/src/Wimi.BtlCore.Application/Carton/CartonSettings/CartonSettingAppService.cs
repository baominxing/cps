using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Carton.CartonSettings.Dtos;
using Wimi.BtlCore.Cartons;
using Wimi.BtlCore.Trace;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Wimi.BtlCore.Carton.CartonSettings
{
    public class CartonSettingAppService : BtlCoreAppServiceBase, ICartonSettingAppService
    {
        private readonly IRepository<CartonRule> cartonRulerepository;
        private readonly IRepository<TraceFlowSetting> traceFlowSettingRepository;
        private readonly IRepository<CartonSetting> cartonSettingRepository;
        private readonly IPrinterManager printerManager;

        public CartonSettingAppService(IRepository<CartonRule> cartonRulerepository,
            IRepository<TraceFlowSetting> traceFlowSettingRepository,
            IRepository<CartonSetting> cartonSettingRepository,
             IPrinterManager printerManager)
        {
            this.cartonRulerepository = cartonRulerepository;
            this.traceFlowSettingRepository = traceFlowSettingRepository;
            this.cartonSettingRepository = cartonSettingRepository;
            this.printerManager = printerManager;
        }

        [HttpPost]
        public async Task<CartonSettingDto> GetCartonSettingByDeviceGroup(int deviceGroupId)
        {
            var returnValue = new CartonSettingDto();

            var setting = await (from cs in cartonSettingRepository.GetAll()
                                 where cs.DeviceGroupId == deviceGroupId
                                 select cs).FirstOrDefaultAsync();

            if (setting != null)
            {
                returnValue = ObjectMapper.Map<CartonSettingDto>(setting);
            }

            return returnValue;
        }

        public IEnumerable<NameValueDto> ListCartonRule()
        {
            var query = (from cr in cartonRulerepository.GetAll()
                         where cr.IsActive
                         select new NameValueDto()
                         {
                             Value = cr.Id.ToString(),
                             Name = cr.Name

                         }).ToList();

            return query;
        }

        public IEnumerable<NameValueDto> ListLocalPrinterName()
        {
            var returnValue = new List<NameValueDto>();
            //var allPrinter = PrinterSettings.InstalledPrinters.Cast<string>().ToList();

            var allPrinter = printerManager.GetInstalledPrinters();
            var i = 1;
            foreach (var print in allPrinter)
            {
                returnValue.Add(new NameValueDto
                {
                    Value = i.ToString(),
                    Name = print,
                });
                i++;
            }
            return returnValue;
        }

        public IEnumerable<NameValueDto> ListTraceFlow(int deviceGroupId)
        {
            var query = (from tfs in traceFlowSettingRepository.GetAll()
                         where tfs.DeviceGroupId == deviceGroupId
                         select new NameValueDto()
                         {
                             Value = tfs.Id.ToString(),
                             Name = tfs.DisplayName

                         }).ToList();

            return query;

            throw new System.NotImplementedException();
        }

        public async Task<int> SaveCartonSetting(CartonSettingDto input)
        {
            if (input.MaxPackingCount <= 0)
            {
                throw new UserFriendlyException(L("KeyOkMaxPackingCount"));
            }

            if (input.IsPrint && string.IsNullOrEmpty(input.PrinterName))
            {
                throw new UserFriendlyException(L("PleaseChosePrinter"));
            }

            if (input.AutoCartonNo && input.CartonRuleId == 0)
            {
                throw new UserFriendlyException(L("PleaseChoseCartonRule"));
            }

            if (input.HasToFlow && string.IsNullOrEmpty(input.FlowIds))
            {
                throw new UserFriendlyException(L("PleaseChoseFlow"));
            }

       
            if (input.Id == 0)
            {
                var entity = ObjectMapper.Map<CartonSetting>(input);
                return await cartonSettingRepository.InsertAndGetIdAsync(entity);
            }
            else
            {
                var entity = await this.cartonSettingRepository.GetAsync(input.Id);
                ObjectMapper.Map(input, entity);

                return await cartonSettingRepository.InsertOrUpdateAndGetIdAsync(entity);
            }
        }
    }
}
