using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using MongoDB.Driver;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore.Feedback;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.Machines.Mongo;
using Wimi.BtlCore.Feedback.Manager;
using Wimi.BtlCore.Reasons.ReasonFeedbacks.Dtos;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Dto;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WIMI.BTL.ReasonFeedback.Dto;

namespace Wimi.BtlCore.Reasons.ReasonFeedbacks
{
    public class ReasonFeedbackAppService : BtlCoreAppServiceBase, IReasonFeedbackAppService
    {
        private readonly IRepository<ReasonFeedbackRecord> reasonFeedbackRecordRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<StateInfo> stateInfoRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly IMachineManager machineManager;
        private readonly IRepository<User, long> userRepository;
        private readonly IReasonFeedbackManager reasonFeedbackManager;
        private readonly MongoMachineManager mongoMachineManager;

        public ReasonFeedbackAppService(
            IRepository<ReasonFeedbackRecord> reasonFeedbackRecordReposiory,
            IRepository<Machine> machineRepository,
            IRepository<StateInfo> stateInfoRepository,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<DeviceGroup> deviceGroupRepository,
            IRepository<User, long> userRepository,
            IMachineManager machineManager,
            IReasonFeedbackManager reasonFeedbackManager,
            MongoMachineManager mongoMachineManager)
        {
            this.machineRepository = machineRepository;
            this.reasonFeedbackRecordRepository = reasonFeedbackRecordReposiory;
            this.stateInfoRepository = stateInfoRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.deviceGroupRepository = deviceGroupRepository;
            this.userRepository = userRepository;
            this.machineManager = machineManager;
            this.reasonFeedbackManager = reasonFeedbackManager;
            this.mongoMachineManager = mongoMachineManager;
        }

        /// <inheritdoc />
        /// <summary>
        /// 通过GroupId获取选中组下面的Machine的原因反馈的详细信息
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns />
        public async Task<IEnumerable<ReasonFeedbackDto>> ListReasonFeedbackInfo(EntityDto input)
        {
            var machineDeviceGroup = this.machineDeviceGroupRepository.GetAll().Where(m => m.DeviceGroupId == input.Id);
            var query =
                await (from mgr in machineDeviceGroup
                       join rfr in this.reasonFeedbackRecordRepository.GetAll().Where(r => r.EndTime == null) on
                           mgr.MachineId equals rfr.MachineId into tbFeedback
                       from rfrData in tbFeedback.DefaultIfEmpty()
                       join mr in this.machineRepository.GetAll() on mgr.MachineId equals mr.Id into tbMachine
                       from mrData in tbMachine.DefaultIfEmpty()
                       join sr in this.stateInfoRepository.GetAll() on rfrData.StateCode equals sr.Code into tbStateInfo
                       from srData in tbStateInfo.DefaultIfEmpty()
                       orderby mrData.SortSeq, mrData.Code
                       select new { mgr, rfrData, mrData, srData }).ToListAsync();
            var users = await this.userRepository.GetAll().ToListAsync();
            var mongodata = this.OriginalMongoMachineList(machineDeviceGroup.Select(m => m.MachineId).ToList());

            var result = new List<ReasonFeedbackDto>();
            foreach (var child in query)
            {
                var reasonFeedback = new ReasonFeedbackDto();
                if (child.mgr != null)
                {
                    reasonFeedback.MachineId = child.mgr.MachineId;
                    var objData = mongodata.FirstOrDefault(m => m.MachineId == child.mgr.MachineId);
                    if (objData != null && objData.State != null)
                    {
                        var stateDocument = objData.State;
                        if (stateDocument.Code != null)
                        {
                            var stateCode = stateDocument.Code;
                            var stateInfo = this.stateInfoRepository.FirstOrDefault(s => s.Code == stateCode);
                            if (stateInfo != null)
                            {
                                reasonFeedback.MachineStateDisplayName = stateInfo.DisplayName;
                                reasonFeedback.Hexcode = stateInfo.Hexcode;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(reasonFeedback.MachineStateDisplayName))
                    {
                        reasonFeedback.MachineStateDisplayName = this.L("NotConnected");
                    }

                    var machine = this.machineRepository.FirstOrDefault(mr => mr.Id == child.mgr.MachineId);
                    if (machine != null)
                        reasonFeedback.ImagePath = this.machineManager.GetMachineImagePath(new EntityDto<Guid?>(machine.ImageId));
                }

                reasonFeedback.StateDisplayName = child.srData?.DisplayName;
                if (child.rfrData != null)
                {
                    reasonFeedback.CreatorUserId = child.rfrData.CreatorUserId;
                    reasonFeedback.Duration = child.rfrData.Duration;
                    reasonFeedback.StartTime = child.rfrData.StartTime;
                    reasonFeedback.EndTime = child.rfrData.EndTime;
                    reasonFeedback.StateCode = child.rfrData.StateCode;
                    var createUser = users.FirstOrDefault(u => u.Id == child.rfrData.CreatorUserId);
                    var finishUser = users.FirstOrDefault(u => u.Id == child.rfrData.EndUserId);
                    if (createUser != null)
                        reasonFeedback.FeedbackPersonName = createUser.Name;
                    if (finishUser != null)
                        reasonFeedback.EndUserName = finishUser.Name;
                    reasonFeedback.Feedbacking = true;
                    reasonFeedback.Hexcode = child.srData.Hexcode;
                    reasonFeedback.MachineStateDisplayName = child.srData.DisplayName;
                }
                else
                {
                    reasonFeedback.Feedbacking = false;
                }

                reasonFeedback.MachineName = child.mrData?.Name;
                reasonFeedback.GroupName = null;
                result.Add(reasonFeedback);
            }

            foreach (var reasonFeedbackDto in result)
            {
                var groupQuery = await (from mgr in this.machineDeviceGroupRepository.GetAll()
                                        join dgr in this.deviceGroupRepository.GetAll() on mgr.DeviceGroupId equals dgr.Id
                                        where mgr.MachineId == reasonFeedbackDto.MachineId
                                        select new { dgr.DisplayName }).ToListAsync();
                var list = new List<string>();
                foreach (var groupName in groupQuery)
                {
                    if (!list.Contains(groupName.DisplayName))
                        list.Add(groupName.DisplayName);
                }

                reasonFeedbackDto.GroupName = list;
            }

            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// 通过MachineId获取Machine的原因反馈历史
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ReasonFeedbackDto>> ListReasonFeedbackHistoryInfo(GetFeedbackHistoryDto input)
        {
            if (input?.Id == null) return new DatatablesPagedResultOutput<ReasonFeedbackDto>();

            var query = from rfr in this.reasonFeedbackRecordRepository.GetAll()
                        join sr in this.stateInfoRepository.GetAll() on rfr.StateCode equals sr.Code
                        where rfr.MachineId == input.Id && rfr.EndTime != null
                        select new { rfr, sr, rfr.StartTime };
            var totalCount = await query.CountAsync();
            var returnValue = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();

            var result = returnValue.Select(
                q => new ReasonFeedbackDto()
                {
                    StartTime = q.rfr.StartTime,
                    EndTime = q.rfr.EndTime,
                    Duration = q.rfr.Duration,
                    StateDisplayName = this.stateInfoRepository
                        .FirstOrDefault(s => s.Code == q.rfr.StateCode)?.DisplayName,
                    FeedbackPersonName = this.GetUserName(q.rfr.CreatorUserId),
                    EndUserName = this.GetUserName(q.rfr.EndUserId)
                }).ToList();

            return new DatatablesPagedResultOutput<ReasonFeedbackDto>(totalCount, result)
            {
                Draw = input.Draw
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// 获取原因反馈的类型
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<GetFeedbackTypeDto>> ListFeedbackType()
        {
            var query =
                await (from sr in this.stateInfoRepository.GetAll()
                           .Where(s => !s.IsPlaned && s.Type == EnumMachineStateType.Reason)
                       select new GetFeedbackTypeDto()
                       {
                           StateId = sr.Id,
                           StateCode = sr.Code,
                           StateDisplayName = sr.DisplayName
                       }).ToListAsync();
            return query;
        }

        /// <inheritdoc />
        /// <summary>
        /// 创建原因反馈
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateReasonFeedback(ReasonFeedbackDto input)
        {
            this.reasonFeedbackManager.CreateReasonFeedbackCheck(input.MachineId);
            var reasonFeedbackRecord = ObjectMapper.Map<ReasonFeedbackRecord>(input);
            await this.reasonFeedbackRecordRepository.InsertAsync(reasonFeedbackRecord);
        }

        /// <inheritdoc />
        /// <summary>
        /// 通过MachineId获取到原因反馈（一台设备同一时间只能进行一种原因反馈）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ReasonFeedbackDto> GetReasonFeedbackRecord(EntityDto input)
        {
            var result =
                await this.reasonFeedbackRecordRepository.FirstOrDefaultAsync(
                    r => r.MachineId == input.Id && r.EndTime == null);
            return ObjectMapper.Map<ReasonFeedbackDto>(result);
        }

        /// <inheritdoc />
        /// <summary>
        /// 结束原因反馈
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task FinishReasonFeedbackRecord(ReasonFeedbackDto input)
        {
            var reasonFeedback = this.reasonFeedbackManager.FinishReasonFeedbackCheck(input.MachineId);
            reasonFeedback.EndUserId = Convert.ToInt32(this.AbpSession.UserId);
            reasonFeedback.EndTime = input.EndTime;
            reasonFeedback.Duration = input.Duration;
            await this.reasonFeedbackRecordRepository.UpdateAsync(reasonFeedback);
        }

        /// <inheritdoc />
        /// <summary>
        /// 检查原因反馈的开始时间是否重合。 Over:表示超出7天的限制,Error:表示开始时间与已有的反馈重合，OK：正确
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CheckTimeResultDto> CheckStartTime(ReasonFeedbackDto input)
        {
            var record = new ReasonFeedbackRecord();
            ObjectMapper.Map(input, record);
               // Mapper.Map<ReasonFeedbackRecord>(input);
            return await Task.Run(() => this.reasonFeedbackManager.CheckStartTime(record));
        }


        /// <summary>
        /// 检查结束时间是否重合
        /// true:表示重合，false:表示没有重合
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public Task<CheckTimeResultDto> CheckEndTime(ReasonFeedbackDto input)
        {
            var record = new ReasonFeedbackRecord();
            ObjectMapper.Map(input, record);
            // var record = Mapper.Map<ReasonFeedbackRecord>(input);
            return Task.Run(() => this.reasonFeedbackManager.CheckEndTime(record));
        }


        private string GetUserName(long? userId)
        {
            var user = this.userRepository.FirstOrDefault(u => u.Id == userId);
            return user == null ? null : user.Name;
        }

        /// <summary>
        /// 获取Mogo数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private List<MongoMachine> OriginalMongoMachineList(IEnumerable<int> input)
        {
            var result = mongoMachineManager.OriginalMongoMachineList(input).ToList();
            return result;
        }

    }
}
