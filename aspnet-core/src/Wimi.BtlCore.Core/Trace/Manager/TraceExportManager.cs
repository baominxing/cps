using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Trace.Dto;

namespace Wimi.BtlCore.Trace.Manager
{
    public class TraceExportManager : ITraceExportManager
    {
        private readonly IRepository<TraceCatalog, long> traceCatalogRepository;
        private readonly IRepository<TraceFlowRecord, long> traceFlowRecordsRepository;
        private readonly IRepository<MachinesShiftDetail> machineShiftDetailsRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<User, long> userRepository;
        private readonly ITraceExportRepository traceExportRepository;

        public TraceExportManager(IRepository<TraceCatalog, long> traceCatalogRepository,
            IRepository<TraceFlowRecord, long> traceFlowRecordsRepository,
            IRepository<MachinesShiftDetail> machineShiftDetailsRepository,
            IRepository<Machine> machineRepository,
            IRepository<User, long> userRepository,
            ITraceExportRepository traceExportRepository)
        {
            this.traceCatalogRepository = traceCatalogRepository;
            this.traceFlowRecordsRepository = traceFlowRecordsRepository;
            this.machineShiftDetailsRepository = machineShiftDetailsRepository;
            this.machineRepository = machineRepository;
            this.userRepository = userRepository;
            this.traceExportRepository = traceExportRepository;
        }

        public async Task<List<TraceExportDto>> ListTraceCatalogForExport(TraceCatalogsInputDto input)
        {
            var query = from tc in traceCatalogRepository.GetAll()
                        join tfr in this.traceFlowRecordsRepository.GetAll() on tc.PartNo equals tfr.PartNo
                        join msd in this.machineShiftDetailsRepository.GetAll() on tc.MachineShiftDetailId equals msd.Id

                        select new
                        {
                            tc.Id,
                            tc.PartNo,
                            tc.DeviceGroupId,
                            tc.OfflineTime,
                            tc.OnlineTime,
                            tc.MachineShiftDetailId,
                            tfr.MachineId,
                            tfr.Station,
                            msd.ShiftSolutionItemId,
                            tc.Qualified,
                            tc.IsReworkPart
                        };

            query = query.WhereIf(input.DeviceGroupId.HasValue, q => q.DeviceGroupId == input.DeviceGroupId)
                        .WhereIf(!input.PartNo.IsNullOrWhiteSpace(), q => q.PartNo.Contains(input.PartNo))
                        .WhereIf(input.MachineId != null && input.MachineId.Count != 0, q => input.MachineId.Contains(q.MachineId))
                        .WhereIf(input.ShiftSolutionItemId != 0, q => q.ShiftSolutionItemId == input.ShiftSolutionItemId)
                        .WhereIf(input.StartTime.HasValue && input.NgPartCatlogId == 0 && input.PartNo.IsNullOrWhiteSpace(), q => q.OnlineTime >= input.StartTime)
                        .WhereIf(input.EndTime.HasValue && input.NgPartCatlogId == 0 && input.PartNo.IsNullOrWhiteSpace(), q => q.OnlineTime <= input.EndTime)
                        .WhereIf(!input.StationCode.IsNullOrWhiteSpace(), q => q.Station.Contains(input.StationCode));

            var queryResult = from q in query
                              select new TraceExportDto
                              {
                                  Id = q.Id,
                                  PartNo = q.PartNo,
                                  DeviceGroupId = q.DeviceGroupId,
                                  OfflineTime = q.OfflineTime,
                                  OnlineTime = q.OnlineTime,
                                  Qualified = q.Qualified,
                                  ShiftSolutionItemId = q.ShiftSolutionItemId,
                                  IsReworkPart = q.IsReworkPart
                              };
            queryResult = queryResult.Distinct();

            if (input.NgPartCatlogId != 0)
            {
                queryResult = queryResult.Where(q => q.Id.Equals(input.NgPartCatlogId));
            }
            var result = await queryResult.OrderByDescending(t => t.OnlineTime).ToListAsync();
            return result;
        }

        public List<TraceExportDto> ListTraceRecordByPartNoForExport(string partNo)
        {

            var partData = (from tc in traceCatalogRepository.GetAll()
                            join tfr in traceFlowRecordsRepository.GetAll() on tc.PartNo equals tfr.PartNo
                            join m in machineRepository.GetAll() on tfr.MachineId equals m.Id into TC_M from m in TC_M.DefaultIfEmpty()
                            join u in this.userRepository.GetAll() on tfr.UserId equals u.Id into u2 from u in u2.DefaultIfEmpty()
                            where tc.PartNo == partNo
                            select new TraceExportDto()
                            {
                                FlowDisplayName = tfr.FlowDisplayName,
                                LeftTime = tfr.LeftTime,
                                EntryTime = tfr.EntryTime,
                                MachineId = tfr.MachineId,
                                MachineName = m == null ? "" : m.Name,
                                State = tfr.State,
                                Tag = tfr.Tag,
                                UserName = u == null ? "" :  u.Name,
                                Station = tfr.Station
                            }).ToList();

            return partData;
        }

        public async Task<IEnumerable<TraceExportItem>> ListTraceExportItem(TraceCatalogsInputDto input)
        {
            return await this.traceExportRepository.ListTraceExportItem(input);
        }
    }
}
