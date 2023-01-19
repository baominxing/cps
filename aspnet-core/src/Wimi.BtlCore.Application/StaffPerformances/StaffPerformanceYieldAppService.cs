using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Abp;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Specifications;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.Capacities;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.StaffPerformance;
using Wimi.BtlCore.StaffPerformances.Dto;

namespace Wimi.BtlCore.StaffPerformances
{
    public class StaffPerformanceYieldAppService : BtlCoreAppServiceBase, IStaffPerformanceYieldAppService
    {
        private readonly IRepository<Capacity> capacityRepository;

        private readonly ICommonLookupAppService commonLookUpAppService;

        private readonly IRepository<Machine> machineRepository;

        private readonly UserManager userManager;

        private readonly IStaffPerformaceRepository staffPerformaceRepository;

        private readonly IRepository<ArchiveEntry> archiveEntryRepository;


        public StaffPerformanceYieldAppService(
            IRepository<Capacity> capacityRepository,
            UserManager userManager,
            IRepository<Machine> machineRepository,
            ICommonLookupAppService commonLookupAppService,
            IStaffPerformaceRepository staffPerformaceRepository,
            IRepository<ArchiveEntry> archiveEntryRepository)
        {
            this.capacityRepository = capacityRepository;
            this.userManager = userManager;
            this.machineRepository = machineRepository;
            this.commonLookUpAppService = commonLookupAppService;
            this.staffPerformaceRepository = staffPerformaceRepository;
            this.archiveEntryRepository = archiveEntryRepository;
        }

        public async Task<IEnumerable<NameValueDto<int>>> AllMachines(AllMachinesRquestDto input)
        {
            var deviceAndMachine = await this.commonLookUpAppService.GetDeviceGroupAndMachineWithPermissions();

            var permissionMachine = deviceAndMachine.Machines
                .WhereIf(input.MachineIds.Any(), c => input.MachineIds.Contains(c.Id) || input.MachineIds.Contains(0))
                .Where(c => deviceAndMachine.GrantedGroupIds.Contains(c.DeviceGroupId))
                .Distinct(new MachineComparer());

            return permissionMachine.Select(c => new NameValueDto<int> { Name = c.Name, Value = c.Id }).ToList();
        }

        public async Task<IEnumerable<NameValueDto<long>>> AllUsers()
        {
            var users = await this.userManager.Users.OrderBy(c => c.Id).Select(c => new NameValue<long> { Name = c.Name, Value = c.Id }).ToListAsync();
            return ObjectMapper.Map<IEnumerable<NameValueDto<long>>>(users.AsEnumerable());
        }

        public async Task<ProductionChartResultDto> ProductionChart(ProductionChartRequestDto input)
        {
            var result = new ProductionChartResultDto();

            var beginDate = input.DateRange.StartDate;
            var endDate = input.DateRange.EndDate;

            if (input.MachineIds.Contains(0))
            {
                var machineids = this.machineRepository.GetAll().Select(s => (long)s.Id).ToList();

                input.MachineIds.Clear();

                input.MachineIds.AddRange(machineids);
            }

            var capacityQuery = await this.CapacityQuery(input.MachineIds.AsEnumerable(), beginDate, endDate, input.ShiftSolutionName);
            var userCapacityQuery = this.UserCapacityQuery(capacityQuery);
            var machinesUserCapacityQuery = this.MachineUserCapacityQuery(userCapacityQuery);

            var spec = new MachineUserCapacityUserOrMachineSpec(input.ByUserOrMachine, input.UserId, input.MachineId, input.ShiftSolutionId);

            result.UserMachinShiftYields = this.UserMachineShiftYieldQuery(machinesUserCapacityQuery.ToList(), spec);

            return result;
        }

        private async Task<IEnumerable<dynamic>> CapacityQuery(IEnumerable<long> machineIds, DateTime? first, DateTime? last, string shiftSolutionName)
        {
            ISpecification<Capacity> capacitySpec = new CapacityMachineSpec(machineIds);
            capacitySpec = capacitySpec.And(new CapacityDateRangeSpec(first, last));
            var unionTables = this.GetUnionTables(first.Value, last.Value);

            var capacityQuery = await this.staffPerformaceRepository.CapacityQuery(machineIds, first.Value, last.Value, unionTables);//this.capacityRepository.GetAll().Where(c => c.UserId.HasValue).Where(capacitySpec.ToExpression());


            if (!string.IsNullOrEmpty(shiftSolutionName))
            {
                capacityQuery = capacityQuery.Where(s => s.ShiftDetail_SolutionName == shiftSolutionName);
            }

            var result = capacityQuery.ToList();

            return capacityQuery;
        }
        private List<string> GetUnionTables(DateTime startTime, DateTime endTime)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
               .Where(s => s.TargetTable == "Capacities").ToList()
               .Where(s => startTime <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= endTime)
               .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }
        private IEnumerable<MachineUserCapacity> MachineUserCapacityQuery(IEnumerable<UserCapacity> userCapacityQuery)
        {
            var machineQuery = this.machineRepository.GetAll().ToList();

            var machinesUserCapacityQuery = userCapacityQuery.Join(
                machineQuery,
                uc => new { MachineId = (int)uc.Capacity.MachineId },
                m => new { MachineId = m.Id },
                (uc, m) =>
                new MachineUserCapacity
                {
                    Machine = m,
                    User = uc.User,
                    FullName = uc.FullName,
                    MachineName = m.Name,
                    Yield = uc.Capacity.Yield,
                    StartDate = uc.Capacity.StartTime,
                    Capacity = uc.Capacity
                });

            var result = machinesUserCapacityQuery.ToList();

            return machinesUserCapacityQuery;
        }

        private IEnumerable<UserCapacity> UserCapacityQuery(IEnumerable<dynamic> capacityQuery)
        {
            var userQuery = this.userManager.Users.ToList();

            var userCapacityQuery = capacityQuery.Join(
                userQuery,
                c => c.UserId,
                u => (int?)u.Id,
                (c, u) => new UserCapacity { User = u, FullName = u.Name, Capacity = c });

            var result = userCapacityQuery.ToList();

            return userCapacityQuery;
        }

        private IEnumerable<UserMachinShiftYieldDto> UserMachineShiftYieldQuery(List<MachineUserCapacity> machinesUserCapacityQuery, MachineUserCapacityUserOrMachineSpec spec)
        {
            var query = machinesUserCapacityQuery.Where(spec.ToExpression().Compile()).Select(
                        c =>
                        new UserMachineYieldDto()
                        {
                            UserName = c.FullName,
                            MachineName = c.MachineName,
                            Yield = c.Yield,
                            MachineStartDate = c.StartDate,
                            StaffShiftName = c.Capacity.ShiftDetail_StaffShiftName,
                            MachineShiftName = c.Capacity.ShiftDetail_MachineShiftName,
                            ShiftDate = c.Capacity.ShiftDetail_ShiftDay
                        });

            var groupQuery = query.GroupBy(
                    c =>
                    new
                    {
                        Date = c.ShiftDate,
                        c.StaffShiftName,
                        c.MachineShiftName,
                        c.MachineName,
                        c.UserName
                    })
                    .Select(
                        c =>
                        new UserMachinShiftYieldDto
                        {
                            ShiftDate = c.Key.Date,
                            MachineShiftName = c.Key.MachineShiftName,
                            StaffShiftName = c.Key.StaffShiftName,
                            MachineName = c.Key.MachineName,
                            UserName = c.Key.UserName,
                            SumYield = c.Sum(d => d.Yield)
                        });

            return groupQuery;
        }

        private class MachineComparer : IEqualityComparer<FlatMachineDto>
        {
            public bool Equals(FlatMachineDto x, FlatMachineDto y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(FlatMachineDto obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        private class MachineUserCapacity
        {
            public dynamic Capacity { get; set; }

            public string FullName { get; set; }

            public Machine Machine { get; set; }

            public string MachineName { get; set; }

            public DateTime? StartDate { get; set; }

            public User User { get; set; }

            public decimal Yield { get; set; }
        }

        private class MachineUserCapacityUserOrMachineSpec : Specification<MachineUserCapacity>
        {
            private const string ByMachine = "machine";

            private const string ByUser = "user";

            private readonly int machineId;

            private readonly string type;

            private readonly long? userId;

            public MachineUserCapacityUserOrMachineSpec(string type, long? userId, int machineId, int shiftSolutionId)
            {
                this.type = type;
                this.userId = userId;
                this.machineId = machineId;
            }

            public override Expression<Func<MachineUserCapacity, bool>> ToExpression()
            {
                switch (this.type)
                {
                    case ByUser:
                        return c => c.User.Id == this.userId.Value;
                    case ByMachine:
                        return c => c.Machine.Id == this.machineId;
                }

                return c => c.User.Id == this.userId.Value;
            }
        }

        private class UserCapacity
        {
            public dynamic Capacity { get; set; }

            public string FullName { get; set; }

            public User User { get; set; }
        }
    }
}