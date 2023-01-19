using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.Carton.CartonTraceability.Dtos;
using Wimi.BtlCore.Carton.CartonTraceability.Export;
using Wimi.BtlCore.Cartons;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Carton.CartonTraceability
{
    [AbpAuthorize(PermissionNames.Pages_Carton_CartonTraceability)]
    public class CartonTraceabilityAppService : BtlCoreAppServiceBase, ICartonTraceabilityAppService
    {
        private readonly IRepository<Cartons.Carton> cartonRepository;
        private readonly IRepository<CartonRecord> cartonRecordRepository;
        private readonly IRepository<CartonSetting> cartonSettingRepository;
        private readonly ICartonTraceabilityExporter exporter;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        public CartonTraceabilityAppService(IRepository<CartonRecord> cartonRecordRepository,
            IRepository<Cartons.Carton> cartonRepository, 
            IRepository<CartonSetting> cartonSettingRepository,
            ICartonTraceabilityExporter exporter,
            IRepository<DeviceGroup> deviceGroupRepository)
        {
            this.cartonRecordRepository = cartonRecordRepository;
            this.cartonRepository = cartonRepository;
            this.cartonSettingRepository = cartonSettingRepository;
            this.exporter = exporter;
            this.deviceGroupRepository = deviceGroupRepository;
        }

        [HttpPost]
        public async Task<CartonDto> GetCartonDto(EntityDto<int> input)
        {
            var result = await this.cartonRepository.GetAsync(input.Id);

            return ObjectMapper.Map<CartonDto>(result);
        }

        public async Task<IEnumerable<CartonRecordDto>> ListPartsInCarton(EntityDto<int> input)
        {
            var query = await this.cartonRecordRepository.GetAll().Where(c => c.CartonId == input.Id).Select(c =>
                new CartonRecordDto
                {
                    Id = c.Id,
                    PartNo = c.PartNo,
                    ShiftDay = c.ShiftDay,
                    ShiftSolutionItemName = c.ShiftSolutionItems.Name,
                    OptionTime = c.CreationTime
                }).ToListAsync();


            return query;
        }

        public async Task<PagedResultDto<CartonTraceabilityDto>> ListTraceabilityRecords(CartonTraceabilityRequestDto input)
        {
            var query = this.cartonRepository.GetAll();
            var endTime = input.EndTime ?? DateTime.Now;

            query = query.WhereIf(!input.CartonNo.IsNullOrWhiteSpace(),
                    t => t.CartonNo.ToLower().Contains(input.CartonNo.ToLower()))
                .WhereIf(!input.PartNo.IsNullOrWhiteSpace(),
                    t => t.CartonRecords.Any(c => c.PartNo.ToLower().Equals(input.PartNo.ToLower().Trim())))
                .WhereIf(input.DeviceGroupId != 0, t => t.DeviceGroupId == input.DeviceGroupId)
                .WhereIf(
                    input.CartonNo.IsNullOrWhiteSpace() && input.PartNo.IsNullOrWhiteSpace() &&
                    input.StartTime.HasValue, t => t.CreationTime >= input.StartTime.Value)
                .WhereIf(
                    input.CartonNo.IsNullOrWhiteSpace() && input.PartNo.IsNullOrWhiteSpace() && input.EndTime.HasValue,
                    t => t.CreationTime <= endTime);

            var result = await query.OrderBy(input.Sorting).PageBy(input).Select(t => new CartonTraceabilityDto
            {
                Id = t.Id,
                CartonNo = t.CartonNo,
                DeviceGroupName = t.DeviceGroup.DisplayName,
                DeviceGroupId = t.DeviceGroupId,
                MaxPackingCount = t.MaxPackingCount,
                PrintLabelCount = t.PrintLabelCount,
                RealPackingCount = t.RealPackingCount,
                CreationTime = t.CreationTime
            }).ToListAsync();
            var count = await query.CountAsync();

            return new DatatablesPagedResultOutput<CartonTraceabilityDto>(count, result);
        }

        public async Task<IEnumerable<CartonRecordDto>> ListCartonRecords(CartonRecordRequestDto input)
        {
            using (this.CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var query = await this.cartonRecordRepository.GetAll()
                    .Where(c => c.CartonNo.ToLower().Equals(input.CartonNo.ToLower())).ToListAsync();

                var result = query.Select(q => new CartonRecordDto()
                {
                    PartNo = q.PartNo,
                    Type = q.IsDeleted,
                    OptionTime = q.IsDeleted ? (q.DeletionTime!=null? q.DeletionTime.Value:null) : q.CreationTime
                });

                return result;
            }
        }

        public async Task<bool> CheckUndoPermission(UndoPackingPermissionDto input)
        {
            var setting = await this.cartonSettingRepository.FirstOrDefaultAsync(t => t.DeviceGroupId == input.DeviceGroupId);
            if (setting == null)
            {
                this.Logger.Error($"设备组 Id =[{input.DeviceGroupId}]无装箱设定信息 ！");
                return false;
            }

            var carton = await this.cartonRepository.FirstOrDefaultAsync(c =>
                c.CartonNo.ToLower().Trim().Equals(input.CartonNo.ToLower().Trim()));

            if (carton == null)
            {
                this.Logger.Error($"箱号[{input.CartonNo}]未找到记录！请核实");
                return false;
            }

            // 允许拆箱 = 再未打印之前可以拆出来重装。 但打印标签后不可以重装 
            if (setting.IsUnpackingRedo && carton.PrintLabelCount == 0)
                return true;

            // 允许打印后拆箱 ， 此时打印次数会大于 0
            if (setting.IsUnpackingAfterPrint && carton.PrintLabelCount > 0)
                return true;


            return false;
        }

        [HttpPost]
        public async Task Delete(EntityDto<int> input)
        {
            var record = await this.cartonRecordRepository.GetAsync(input.Id);
            var carton = await this.cartonRepository.GetAsync(record.CartonId);

            await this.cartonRecordRepository.DeleteAsync(input.Id);
            carton.RealPackingCount = carton.RealPackingCount - 1;


            if (carton.RealPackingCount <= 0)
            {
                await this.cartonRepository.DeleteAsync(record.CartonId);
            }
        }

        public async Task<FileDto> ExportTraceabilityRecords(CartonTraceabilityRequestDto input)
        {
            var query = this.cartonRepository.GetAll();
            var endTime = input.EndTime ?? DateTime.Now;

            query = query.WhereIf(!input.CartonNo.IsNullOrWhiteSpace(), t => t.CartonNo.ToLower().Contains(input.CartonNo.ToLower()))
                .WhereIf(!input.PartNo.IsNullOrWhiteSpace(), t => t.CartonRecords.Any(c => c.PartNo.ToLower().Equals(input.PartNo.ToLower().Trim())))
                .WhereIf(input.DeviceGroupId != 0, t => t.DeviceGroupId == input.DeviceGroupId)
                .WhereIf(input.CartonNo.IsNullOrWhiteSpace() && input.PartNo.IsNullOrWhiteSpace() && input.StartTime.HasValue, t => t.CreationTime >= input.StartTime.Value)
                .WhereIf(input.CartonNo.IsNullOrWhiteSpace() && input.PartNo.IsNullOrWhiteSpace() && input.EndTime.HasValue, t => t.CreationTime <= endTime);

            var cartons = await query.ToListAsync();

            var exportData = new List<CartonExportDto>();

            cartons.ForEach(carton =>
            {
                var cartonRecords = this.cartonRecordRepository.GetAll().Where(c => c.CartonId == carton.Id).Select(c => new CartonDetailDto
                {
                    PartNo = c.PartNo,
                    ShiftDay = c.ShiftDay,
                    ShiftSolutionItemName = c.ShiftSolutionItems.Name,
                    OperationTime = c.CreationTime
                }).ToList();
                carton.DeviceGroup = deviceGroupRepository.FirstOrDefault(x=>x.Id==carton.DeviceGroupId);
                var exportCarton = new CartonExportDto
                {
                    CartonNo = carton.CartonNo,
                    DeviceGroupName = carton.DeviceGroup.DisplayName,
                    DeviceGroupId = carton.DeviceGroup.Id,
                    MaxPackingCount = carton.MaxPackingCount,
                    RealPackingCount = carton.RealPackingCount,
                    PrintLabelCount = carton.PrintLabelCount,
                    CreationTime = carton.CreationTime,
                    Details = cartonRecords
                };

                exportData.Add(exportCarton);
            });

            return exporter.ExportCartonToFile(exportData);
        }
    }
}
