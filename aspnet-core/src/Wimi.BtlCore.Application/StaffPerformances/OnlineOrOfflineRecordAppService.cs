using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.StaffPerformance;
using Wimi.BtlCore.StaffPerformances.Dto;
using Abp.Linq.Extensions;

namespace Wimi.BtlCore.StaffPerformances
{
    public class OnlineOrOfflineRecordAppService : BtlCoreAppServiceBase, IOnlineOrOfflineRecordAppService
    {
        private readonly IRepository<Machine> machineRepository;

        private readonly IRepository<OnlineAndOfflineLog, long> onlineAndOfflineLogRepository;

        private readonly IRepository<User, long> userRepository;

        private readonly IRepository<MachineDeviceGroup> machineGroupRepository;

        private readonly IRepository<DeviceGroup> deviceGroupRepository;


        public OnlineOrOfflineRecordAppService(
            IRepository<OnlineAndOfflineLog, long> onlineAndOfflineLogRepository,
            IRepository<Machine> machineRepository,
            IRepository<User, long> userRepository,
            IRepository<MachineDeviceGroup> machineGroupRepository,
            IRepository<DeviceGroup> deviceGroupRepository
        )
        {
            this.onlineAndOfflineLogRepository = onlineAndOfflineLogRepository;
            this.machineRepository = machineRepository;
            this.userRepository = userRepository;
            this.machineGroupRepository = machineGroupRepository;
            this.deviceGroupRepository = deviceGroupRepository;
        }

        /// <summary>
        /// 查询人员上下线记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<OnlineAndOfflineRecordDto>> QueryRecords(QueryRecordsDto input)
        {
            if (input.MachineIdList.Count <= 0)
            {
                return new DatatablesPagedResultOutput<OnlineAndOfflineRecordDto>();
            }

            var query = from oaolr in this.onlineAndOfflineLogRepository.GetAll()
                        join mr in this.machineRepository.GetAll() on oaolr.MachineId equals mr.Id
                        join ur in this.userRepository.GetAll() on oaolr.UserId equals ur.Id
                        select
                            new OnlineAndOfflineRecordDto
                                {
                                    UserName = ur.Name, 
                                    MachineName = mr.Name, 
                                    OnlineTime = oaolr.OnlineDateTime, 
                                    OfflineTime = oaolr.OfflineDateTime, 
                                    MachineId = mr.Id
                                };

            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate;

            if (input.StartTime.HasValue && input.EndTime.HasValue)
            {
                startDate = input.StartTime.Value.Date;
                var endNext = input.EndTime.Value.AddDays(1);
                endDate = endNext.Date;
            }

            query =
                query.WhereIf(!input.UserName.IsNullOrWhiteSpace(), q => q.UserName.Contains(input.UserName))
                    .WhereIf(
                        input.StartTime.HasValue && input.EndTime.HasValue, 
                        c => c.OnlineTime >= startDate && c.OnlineTime < endDate)
                    .WhereIf(input.MachineIdList.Count > 0, q => input.MachineIdList.Contains(q.MachineId));

            var result = query.OrderBy(input.Sorting).AsNoTracking().PageBy(input);
            var count = await query.CountAsync();
            return new DatatablesPagedResultOutput<OnlineAndOfflineRecordDto>(
                count, 
                ObjectMapper.Map<List<OnlineAndOfflineRecordDto>>(result), 
                count)
                {
                    Draw = input.Draw 
                };
        }

        public async Task<ListResultDto<NameValueDto<long>>> ListUsers(EntityDto input)
        {
            var query = this.UserManager.Users
                .Select(q => new NameValueDto<long>()
                {
                    Name = q.Name,
                    Value = q.Id

                });
            var result = await query.ToListAsync();

            return new ListResultDto<NameValueDto<long>>(result);
        }

        public async Task<ListResultDto<NameValueDto<int>>> ListDeviceGroups()
        {
            var query = from g in this.machineGroupRepository.GetAll()
                        join d in this.deviceGroupRepository.GetAll() on g.DeviceGroupId equals d.Id
                        select (new NameValueDto<int>()
                        {
                            Value = d.Id,
                            Name = d.DisplayName

                        });
            var result = await query.Distinct().ToListAsync();
            return new ListResultDto<NameValueDto<int>>(result);
        }
    }
}