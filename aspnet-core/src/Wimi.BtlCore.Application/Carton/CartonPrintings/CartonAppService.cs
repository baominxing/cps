using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Carton.CartonPrintings.Dtos;
using Wimi.BtlCore.Cartons;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.DefectiveParts;
using Wimi.BtlCore.Order.PartDefects;
using Wimi.BtlCore.Order.PartDefects.Manager;
using Wimi.BtlCore.Trace;
using Wimi.BtlCore.Trace.Dtos;
using Wimi.BtlCore.Trace.Manager;

namespace Wimi.BtlCore.Carton.CartonPrintings
{
    public class CartonAppService : BtlCoreAppServiceBase, ICartonAppService
    {
        private readonly IRepository<Cartons.Carton> cartonRepository;
        private readonly IRepository<CartonRecord> cartonRecordRepository;
        private readonly IRepository<CartonSetting> cartonSettingRepository;
        private readonly IRepository<TraceCatalog, long> traceCatalogRepository;
        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemsRepository;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly DeviceGroupManager deviceGroupManager;
        private readonly CartonManager cartonManager;
        private readonly TraceabilityManager traceabilityManager;
        private readonly IRepository<DefectivePartReason> defectivePartReasonRepository;
        private readonly IPartDefectManager partDefectManager;
        private readonly IRepository<PartDefect> partDefectRepository;
        private readonly IPrinterManager printerManager;

        public CartonAppService(
            IRepository<Cartons.Carton> cartonRepository,
            IRepository<CartonRecord> cartonRecordRepository,
            IRepository<TraceCatalog, long> traceCatalogRepository,
            DeviceGroupManager deviceGroupManager,
            IRepository<CartonSetting> cartonSettingRepository,
            CartonManager cartonManager,
            IRepository<ShiftSolutionItem> shiftSolutionItemsRepository,
            IRepository<DeviceGroup> deviceGroupRepository,
            TraceabilityManager traceabilityManager,
            IRepository<DefectivePartReason> defectivePartReasonRepository,
            IPartDefectManager partDefectManager,
            IRepository<PartDefect> partDefectRepository,
            IPrinterManager printerManager)
        {
            this.cartonRepository = cartonRepository;
            this.cartonRecordRepository = cartonRecordRepository;
            this.traceCatalogRepository = traceCatalogRepository;
            this.deviceGroupManager = deviceGroupManager;
            this.cartonSettingRepository = cartonSettingRepository;
            this.cartonManager = cartonManager;
            this.shiftSolutionItemsRepository = shiftSolutionItemsRepository;
            this.deviceGroupRepository = deviceGroupRepository;
            this.traceabilityManager = traceabilityManager;
            this.defectivePartReasonRepository = defectivePartReasonRepository;
            this.partDefectManager = partDefectManager;
            this.partDefectRepository = partDefectRepository;
            this.printerManager = printerManager;
        }
        
        [HttpPost]
        public async Task<GetCartonSettingOutputDto> GetCartonSettingByParNo(EntityDto<string> input)
        {
            var traceCatalog = await this.traceCatalogRepository.FirstOrDefaultAsync(t => t.PartNo == input.Id);
            if (traceCatalog == null)
            {
                throw new UserFriendlyException(L("CantFindPart"));
            }

            var cartonSetting = await this.cartonManager.GetCartonSettingByDeviceGroupId(traceCatalog.DeviceGroupId);

            var result = ObjectMapper.Map<GetCartonSettingOutputDto>(cartonSetting);
            return result;
        }

        [HttpPost]
        public async Task<GetCartonAndSettingByCartonNoOutputDto> GetCartonAndSettingByCartonNo(EntityDto<string> input)
        {
            var result = new GetCartonAndSettingByCartonNoOutputDto();
            var carton = await this.cartonRepository.FirstOrDefaultAsync(c => c.CartonNo == input.Id);
            if (carton == null)
            {
                throw new UserFriendlyException(L("CantFindCarton"));
            }

            var cartonSetting = await this.cartonManager.GetCartonSettingByDeviceGroupId(carton.DeviceGroupId);

            result.CartonInfo = ObjectMapper.Map<PackingOutputDto>(carton);
            result.CartonSetting = ObjectMapper.Map<GetCartonSettingOutputDto>(cartonSetting);
            return result;
        }

        public async Task<PackingOutputDto> Packing(PackingInputDto input)
        {
            var cartonSetting = await this.cartonSettingRepository.GetAsync(input.CartonSettingId);

            var traceCatalog = await this.traceCatalogRepository.FirstOrDefaultAsync(t => t.PartNo == input.PartNo);
            if (traceCatalog == null)
            {
                throw new UserFriendlyException(L("CantFindPart"));
            }

            //校验工作
            await this.cartonManager.CheckCartonSetting(traceCatalog, cartonSetting);

            //箱码
            var cartonNo = input.CartonNo;
            if (input.IsSwitchNo && cartonSetting.AutoCartonNo)
            {
                cartonNo = await this.cartonManager.GenerateCartonNoById(cartonSetting.CartonRuleId, traceCatalog);
            }
            if (cartonNo.IsNullOrEmpty() || cartonNo.IsNullOrWhiteSpace())
            {
                throw new UserFriendlyException(L("CartonCantBeNull"));
            }

            //生成箱码记录
            await this.cartonManager.AddPackingRecord(traceCatalog, cartonSetting, cartonNo, AbpSession.UserId);
            await this.CurrentUnitOfWork.SaveChangesAsync();

            //返回装箱信息
            var carton = await this.cartonRepository.FirstOrDefaultAsync(x => x.CartonNo == cartonNo);
            return ObjectMapper.Map<PackingOutputDto>(carton);
        }

        public async Task<PagedResultDto<ListCartonRecordsOutputDto>> ListCartonRecords(ListCartonRecordsInputDto input)
        {
            var query = from cr in this.cartonRecordRepository.GetAll()
                        join si in this.shiftSolutionItemsRepository.GetAll() on cr.ShiftSolutionItemId equals si.Id
                        where cr.CartonNo == input.CartonNo
                        select new ListCartonRecordsOutputDto()
                        {
                            Id = cr.Id,
                            PartNo = cr.PartNo,
                            ShiftDay = cr.ShiftDay,
                            ShiftName = si.Name,
                            CartonTime = cr.CreationTime
                        };

            var result = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            var resultCount = await query.CountAsync();
            return new DatatablesPagedResultOutput<ListCartonRecordsOutputDto>(
                       resultCount,
                       result,
                       resultCount)
            {
                Draw = input.Draw
            };
        }

        [HttpPost]
        public async Task<GetCartonRecordOutputDto> GetCartonRecord(EntityDto input)
        {
            var cartonRecord = await this.cartonRecordRepository.GetAsync(input.Id);

            var partDetail = await (from catalog in traceCatalogRepository.GetAll()
                                    join d in deviceGroupRepository.GetAll() on catalog.DeviceGroupId equals d.Id
                                    where catalog.PartNo == cartonRecord.PartNo
                                    select new GetCartonRecordOutputDto
                                    {
                                        PartNo = catalog.PartNo,
                                        OnlineTime = catalog.OnlineTime,
                                        OfflineTime = catalog.OfflineTime,
                                        Qualified = catalog.Qualified,
                                        IsReworkPart = catalog.IsReworkPart,
                                        DeviceGroupId = catalog.DeviceGroupId,
                                        DeviceGroupName = d.DisplayName
                                    }).FirstAsync();

            partDetail.BuildTags();

            return partDetail;
        }

        public async Task UpdateMaxCount(UpdateMaxCountInputDto input)
        {
            await this.cartonManager.UpdateCartonMaxCount(input.MaxCount, input.CartonNo, input.CartonSettingId);
        }

        public async Task Print(PrintInputDto input)
        {
            input.CartonNo = "1";
            await this.cartonManager.Print(input.CartonNo, input.PrinterName);
        }

        public async Task FinalInspec(FinalInspecInputDto input)
        {
            //下线处理
            var machineId = await this.traceabilityManager.OfflinePart(new OfflinePartInputDto { PartNo = input.PartNo, Qualified = input.Qualified });

            //NG录入NG工件列表
            if (!input.Qualified)
            {
                foreach (var partReasonId in input.DefectivePartReasonIds)
                {
                    var partReason = await this.defectivePartReasonRepository.GetAsync(partReasonId);
                    var entity = new PartDefect()
                    {
                        DefectivePartId = partReason.PartId,
                        DefectiveReasonId = partReason.ReasonId,
                        PartNo = input.PartNo,
                        DefectiveMachineId = machineId
                    };
                    this.partDefectManager.CheckPartDefectiveInfo(entity);
                    await this.partDefectRepository.InsertAsync(entity);
                }
            }
        }


        [HttpPost]
        public List<string> GetInstalledPrinters()
        {
            return printerManager.GetInstalledPrinters();
        }
    }
}
