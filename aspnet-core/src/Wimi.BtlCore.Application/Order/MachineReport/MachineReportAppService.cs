using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.ObjectMapping;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.Capacities;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Order.DefectiveParts;
using Wimi.BtlCore.Order.DefectiveReasons;
using Wimi.BtlCore.Order.MachineDefectiveRecords;
using Wimi.BtlCore.Order.MachineProcesses;
using Wimi.BtlCore.Order.MachineReport.Dtos;
using Wimi.BtlCore.Order.Products;
using Wimi.BtlCore.Order.Products.Dtos;
using Wimi.BtlCore.ShiftDayTimeRange;

namespace Wimi.BtlCore.Order.MachineReport
{
    [AbpAuthorize(PermissionNames.Pages_Order_MachineReport)]
    public class MachineReportAppService : BtlCoreAppServiceBase, IMachineReportAppService
    {
        private readonly IRepository<MachineDefectiveRecord> machineDefectiveRecordRepository;
        private readonly IRepository<Capacity> capacityRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<Products.Product> productRepository;
        private readonly IRepository<MachinesShiftDetail> machineShiftDetailRepository;
        private readonly IRepository<MachineProcess> machineProcessRepository;
        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemRepository;
        private readonly IMachineDefectiveRecordManager machineDefectiveRecordManager;
        private readonly IRepository<DefectiveReason> defectiveRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly IRepository<DefectivePart> defectivePartsRepository;
        private readonly IRepository<DefectivePartReason> defectivePartReasonsRepository;

        public MachineReportAppService(
            IRepository<MachineDefectiveRecord> machineDefectiveRecordRepository,
            IRepository<Capacity> capacityRepository,
            IRepository<Machine> machineRepository,
            IRepository<Products.Product> productRepository,
            IRepository<MachinesShiftDetail> machineShiftDetailRepository,
            IRepository<MachineProcess> machineProcessRepository,
            IRepository<ShiftSolutionItem> shiftSolutionItemRepository,
            IMachineDefectiveRecordManager machineDefectiveRecordManager,
            IRepository<DefectiveReason> defectiveRepository,
            IShiftDayTimeRangeRepository shiftDayTimeRangeRepository,
            IRepository<DefectivePart> defectivePartsRepository,
            IRepository<DefectivePartReason> defectivePartReasonsRepository
            )
        {
            this.machineDefectiveRecordRepository = machineDefectiveRecordRepository;
            this.capacityRepository = capacityRepository;
            this.machineRepository = machineRepository;
            this.productRepository = productRepository;
            this.machineShiftDetailRepository = machineShiftDetailRepository;
            this.machineProcessRepository = machineProcessRepository;
            this.shiftSolutionItemRepository = shiftSolutionItemRepository;
            this.machineDefectiveRecordManager = machineDefectiveRecordManager;
            this.defectiveRepository = defectiveRepository;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
            this.defectivePartsRepository = defectivePartsRepository;
            this.defectivePartReasonsRepository = defectivePartReasonsRepository;
        }

        [HttpPost]
        public async Task<ShiftDayTimeRange.ShiftDayTimeRange> GetShiftDayTimeRange(MachineReportDefectiveReasonRequestDto input)
        {
            var startTime = input.Date ?? DateTime.Today;
            var machineIds = this.machineRepository.GetAll().Select(m => m.Id);

            if(!machineIds.Any())
            {
                throw new UserFriendlyException("未找到任何设备!");
            }
            var shiftDayTimeRanges = await this.shiftDayTimeRangeRepository.ListShiftDayTimeRanges(machineIds, startTime, startTime);
            return shiftDayTimeRanges.FirstOrDefault();
        }

        public async Task<IEnumerable<MachineReportDefectiveReasonDto>> ListHourlyYieldAnalysis(MachineReportDefectiveReasonRequestDto input)
        {
            var startTime = input.Date ?? DateTime.Today;
            var endTime = input.Date?.AddDays(1).Date ?? DateTime.Now.AddDays(1).Date;
            var machineIds = this.machineRepository.GetAll().Select(m => m.Id);

            var shiftDayTimeRanges = (await this.shiftDayTimeRangeRepository.ListShiftDayTimeRanges(machineIds, startTime, endTime)).ToList();
            var shift = shiftDayTimeRanges.FirstOrDefault(sd => sd.ShiftDay == startTime);
            if (shift == null)
            {
                return new List<MachineReportDefectiveReasonDto>();
            }

            var reportList = (from c in this.capacityRepository.GetAll()
                              join m in this.machineRepository.GetAll() on c.MachineId equals m.Id
                              join msd in this.machineShiftDetailRepository.GetAll() on c.MachinesShiftDetailId equals msd.Id
                              join s in this.shiftSolutionItemRepository.GetAll() on msd.ShiftSolutionItemId equals s.Id
                              where c.ShiftDetail.ShiftDay == shift.ShiftDay
                              select new MachineReportDefectiveReasonDto
                              {
                                  MachineId = m.Id,
                                  StarTime = c.StartTime,
                                  MachineName = m.Name,
                                  ShiftDay = msd.ShiftDay,
                                  ShiftSolutionItemId = msd.ShiftSolutionItemId,
                                  ShiftName = s.Name,
                                  Yield = c.Yield,
                                  ProductId = c.ProductId
                              })
                            .WhereIf(input.MacineId.HasValue && input.MacineId!=0, q => q.MachineId == input.MacineId)
                            .AsEnumerable();

            var query = from q in reportList
                        join sd in shiftDayTimeRanges on new { q.MachineId, q.ShiftDay } equals new { sd.MachineId, sd.ShiftDay }
                        where sd.ShiftDay == input.Date && q.StarTime >= sd.StartTime && q.StarTime <= sd.EndTime
                        select q;

            var defectiveRecords = this.machineDefectiveRecordRepository.GetAll()
                .Where(md => md.ShiftDay == shift.ShiftDay)
                .GroupBy(x => new { x.MachineId, x.ShiftSolutionItemId, x.ProductId })
                .Select(n => new { n.Key.MachineId, n.Key.ShiftSolutionItemId, n.Key.ProductId, UnqualifiedCount = n.ToList().Sum(s => s.Count) });

            var groupByMachine = (from s in query
                                  join r in defectiveRecords on new { s.MachineId, s.ShiftSolutionItemId, s.ProductId } equals new { r.MachineId, r.ShiftSolutionItemId, r.ProductId } into g
                                  from k in g.DefaultIfEmpty()
                                  select new { Dto = s, DefectiveRecord = k })
                                  .GroupBy(q => new { q.Dto.MachineId, q.Dto.ProductId }, (key, g) => new { key.MachineId, key.ProductId, List = g.ToList() });

            var result = new List<MachineReportDefectiveReasonDto>();
            var products = await this.GetProductByMachineProcess();

            var temp = groupByMachine.ToList();
            foreach (var gm in temp)
            {
                var groupByShift = gm.List.GroupBy(k => k.Dto.ShiftSolutionItemId, (key, k) => new { SolutionItemId = key, ShiftList = k.ToList() }).ToList();
                var product = products.FirstOrDefault(p => p.Name == gm.ProductId.ToString());

                var list = from item in groupByShift
                           let report = item.ShiftList.First()
                           select new MachineReportDefectiveReasonDto()
                           {
                               MachineId = gm.MachineId,
                               ShiftSolutionItemId = item.SolutionItemId,
                               ShiftName = item.ShiftList.First(s => s.Dto.ShiftSolutionItemId == item.SolutionItemId).Dto.ShiftName,
                               MachineName = report.Dto.MachineName,
                               ProductId = product?.Value.Id ?? 0,
                               ProductName = product?.Value.Name,
                               UnqualifiedCount = report.DefectiveRecord?.UnqualifiedCount ?? 0,
                               Yield = item.ShiftList.Sum(s => s.Dto.Yield),
                               CreationTime = input.Date
                           };

                result.AddRange(list);
            }

            return result.OrderBy(r => r.MachineId).ThenBy(r => r.ShiftSolutionItemId);
        }

        public async Task<IEnumerable<MachineShiftDefectiveAnalysisDto>> ListMachineDefective(MachineShiftDefectiveAnalysisRequestDto input)
        {
            var startTime = input.Date;
            var endTime = input.Date.AddDays(1).Date;
            var machineIds = this.machineRepository.GetAll().Select(m => m.Id);

            var shiftDayTimeRanges = (await this.shiftDayTimeRangeRepository.ListShiftDayTimeRanges(machineIds, startTime, endTime)).ToList();
            var shiftDay = shiftDayTimeRanges.FirstOrDefault(sd => sd.ShiftDay == startTime);
            var query = await this.machineDefectiveRecordRepository.GetAll().Include(d => d.DefectiveReason).Where(
                            md => md.MachineId == input.MachineId && md.ShiftSolutionItemId == input.ShiftSolutionItemId && md.ProductId == input.ProductId
                                  && md.ShiftDay == shiftDay.ShiftDay).ToListAsync();

            var result = query.GroupBy(q => q.DefectiveReasonsId, (key, g) => new { ReasonsId = key, List = g.ToList() }).Select(
                    r => new MachineShiftDefectiveAnalysisDto()
                    {
                        DefectiveReasonsId = r.ReasonsId,
                        MachineId = r.List.First().MachineId,
                        DefectiveReason = r.List.First().DefectiveReason,
                        Count = r.List.Sum(c => c.Count),
                    });

            result = result.Where(p => p.Count > 0);
            return result;
        }

        public async Task<IEnumerable<MachinePartsDefectiveRecordDto>> ListMachineDefectiveRecords(MachineDefectiveRecordDto input)
        {
            var machineIds = this.machineRepository.GetAll().Select(m => m.Id);
            var shiftDayTimeRanges = (await this.shiftDayTimeRangeRepository.ListShiftDayTimeRanges(machineIds, input.CreationTime, input.CreationTime)).ToList();
            var shiftDay = shiftDayTimeRanges.FirstOrDefault(sd => sd.ShiftDay == input.CreationTime);

            var reasons = this.machineDefectiveRecordRepository.GetAll().Where(
                w => w.MachineId == input.MachineId && w.ShiftSolutionItemId == input.ShiftSolutionItemId && w.ProductId == input.ProductId && w.ShiftDay >= shiftDay.ShiftDay);

            var result = await (from d in this.defectiveRepository.GetAll()
                                join wdr in reasons on d.Id equals wdr.DefectiveReasonsId into g
                                from k in g.DefaultIfEmpty()
                                select new MachineDefectiveRecordDto()
                                {
                                    DefectiveReasonsId = d.Id,
                                    ReasonName = d.Name,
                                    Count = k.Count
                                }).ToListAsync();

            var query = (from r in result
                         join dp in defectivePartReasonsRepository.GetAll() on r.DefectiveReasonsId equals dp.ReasonId
                         join p in defectivePartsRepository.GetAll() on dp.PartId equals p.Id
                         group new { p.Name, r } by new { p.Id, p.Name } into t
                         select new MachinePartsDefectiveRecordDto()
                         {
                             DefectivePartName = t.Key.Name,
                             DefectiveRecordList = t.Where(p => p.Name == t.Key.Name).Select(p => p.r).ToList()
                         }).ToList();

            return query;
            //return ObjectMapper.Map<IEnumerable<MachineDefectiveRecordDto>>(result);
        }

        public async Task FeedbackDefectiveReason(MachineDefectiveRecordInputDto input)
        {
            await this.machineDefectiveRecordManager.CreateOrUpdateDefectiveRecords(input.MachineId, input.ShiftSolutionItemId, input.ProductId, input.Reasons, input.Date);
        }

        private async Task<IEnumerable<NameValueDto<ProductDto>>> GetProductByMachineProcess()
        {
            var products = await this.machineProcessRepository.GetAll().Join(
                this.productRepository.GetAll(),
                mp => mp.ProductId,
                p => p.Id,
                (mp, p) => new { MachineProcess = mp, Product = p }).ToListAsync();

            var result = products.Select(
                p => new NameValueDto<ProductDto>()
                {
                    Name = p.MachineProcess.ProductId.ToString(),
                    Value = ObjectMapper.Map<ProductDto>(p.Product)
                });

            return result;
        }
    }
}
