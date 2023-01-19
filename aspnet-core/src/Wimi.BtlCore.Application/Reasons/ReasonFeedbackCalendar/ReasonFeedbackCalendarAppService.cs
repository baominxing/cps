using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Hangfire;
using Hangfire.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Feedback;
using Wimi.BtlCore.Reasons.ReasonFeedbackCalendar.Dtos;

namespace WIMI.BTL.ReasonFeedbackCalendar
{
    public class ReasonFeedbackCalendarAppService: BtlCoreAppServiceBase, IReasonFeedbackCalendarAppService
    {
        private readonly RecurringJobService recurringJobService;
        private readonly IRepository<FeedbackCalendar> feedbackcalendarRepository;
        private readonly IRepository<FeedbackCalendarDetail> detailRepository;
        private readonly IRepository<StateInfo> stateInfoRepository;
        private readonly IStateInfoManager stateInfoManager;

        public ReasonFeedbackCalendarAppService(RecurringJobService recurringJobService, IRepository<FeedbackCalendar> feedbackcalendarRepository, IStateInfoManager stateInfoManager, IRepository<StateInfo> stateInfoRepository, IRepository<FeedbackCalendarDetail> detailRepository)
        {
            this.recurringJobService = recurringJobService;
            this.feedbackcalendarRepository = feedbackcalendarRepository;
            this.stateInfoManager = stateInfoManager;
            this.stateInfoRepository = stateInfoRepository;
            this.detailRepository = detailRepository;
        }

        [Obsolete]
        public async Task<DatatablesPagedResultOutput<FeedbackCalendarDto>> ListFeedbackCalendar(FeedbackCalendarInputDto input)
        {
            var query = from feedbackcalendar in this.feedbackcalendarRepository.GetAll()
                        join s in this.stateInfoRepository.GetAll() on feedbackcalendar.StateCode  equals  s.Code
                        select new FeedbackCalendarDto
                        {
                            Id = feedbackcalendar.Id,
                            Code = feedbackcalendar.Code,
                            Name = feedbackcalendar.Name,
                            Cron = feedbackcalendar.Cron,
                            StateCode = feedbackcalendar.StateCode,
                            StateName = s.DisplayName,
                            Duration = feedbackcalendar.Duration,
                            IsDeleted = feedbackcalendar.IsDeleted,
                            DeleterUserId = feedbackcalendar.DeleterUserId,
                            DeletionTime = feedbackcalendar.DeletionTime,
                            LastModificationTime = feedbackcalendar.LastModificationTime,
                            LastModifierUserId = feedbackcalendar.LastModifierUserId,
                            CreationTime = feedbackcalendar.CreationTime,
                            CreatorUserId = feedbackcalendar.CreatorUserId,
                        };


            query = query.WhereIf(!string.IsNullOrEmpty(input.Name), q => q.Name.Contains(input.Name))
                         .WhereIf(input.StateCode!="0", q=>q.StateCode.Equals(input.StateCode));

            var entitiyList = await query.AsNoTracking().PageBy(input).OrderBy(input.Sorting).ToListAsync();
            var entitiyListCount = await query.CountAsync();


            var jobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            var result = from e in entitiyList
                join j in jobs on e.Code equals j.Id into k
                from g in k.DefaultIfEmpty()
                select new FeedbackCalendarDto
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = e.Name,
                    StateCode = e.StateCode,
                    Duration = e.Duration,
                    StateName = e.StateName,
                    Cron = e.Cron,
                    Error = g?.Error,
                    LastExecution = g?.LastExecution,
                    NextExecution = g?.NextExecution,
                    LastJobState = g?.LastJobState
                };
           
            return new DatatablesPagedResultOutput<FeedbackCalendarDto>(
                       entitiyListCount,
                       result.MapTo<List<FeedbackCalendarDto>>(),
                       entitiyListCount)
            {
                Draw = input.Draw
            };
        }

        public async Task Create(FeedbackCalendarInputDto input)
        {
            var entity = new FeedbackCalendar();
            ObjectMapper.Map(input, entity);
           // var entity = input.MapTo<FeedbackCalendar>();

            var existCode = await this.feedbackcalendarRepository.GetAll().AnyAsync(t => t.Code.Equals(entity.Code));
            if (existCode)
            {
                throw new UserFriendlyException($"已存在编码为[{entity.Code}]的记录!");
            }

            var existName = await this.feedbackcalendarRepository.GetAll().AnyAsync(t => t.Name.Trim().Equals(entity.Name.Trim()));
            if (existName)
            {
                throw new UserFriendlyException($"已存在名称为[{entity.Name}]的记录!");
            }

            var id=  await this.feedbackcalendarRepository.InsertAndGetIdAsync(entity);

            RecurringJob.AddOrUpdate($"{entity.Code}",  () => recurringJobService.Feedback(new EntityDto(id)), entity.Cron, TimeZoneInfo.Local);
        }

        public async Task Update(FeedbackCalendarInputDto input)
        {
            var entity = this.feedbackcalendarRepository.FirstOrDefault(s => s.Id == input.Id);

            var existName = await this.feedbackcalendarRepository.GetAll().AnyAsync(t => t.Name.Trim().Equals(input.Name.Trim()) && t.Id != entity.Id);
            if (existName)
            {
                throw new UserFriendlyException($"已存在名称为[{input.Name}]的记录!");
            }

            entity.Code = input.Code;
            entity.Name = input.Name;
            entity.Cron = input.Cron;
            entity.StateCode = input.StateCode;
            entity.Duration = input.Duration;

            await this.feedbackcalendarRepository.UpdateAsync(entity);
        }

        public async Task Delete(EntityDto input)
        {
            var entity = this.feedbackcalendarRepository.FirstOrDefault(s => s.Id == input.Id);
            if (entity!=null)
            {
                await this.feedbackcalendarRepository.DeleteAsync(entity);
            }
            await this.detailRepository.DeleteAsync(t => t.FeedbackCalendarId == input.Id);

            RecurringJob.RemoveIfExists($"{entity.Code}");
        }

        public async Task<FeedbackCalendarDto> Get(FeedbackCalendarInputDto input)
        {
            var query = from feedbackcalendar in this.feedbackcalendarRepository.GetAll()
                        where feedbackcalendar.Id == input.Id
                        select new FeedbackCalendarDto
                        {
                            Id = feedbackcalendar.Id,
                            Code = feedbackcalendar.Code,
                            Name = feedbackcalendar.Name,
                            Cron = feedbackcalendar.Cron,
                            StateCode = feedbackcalendar.StateCode,
                            Duration = feedbackcalendar.Duration,
                            IsDeleted = feedbackcalendar.IsDeleted,
                            DeleterUserId = feedbackcalendar.DeleterUserId,
                            DeletionTime = feedbackcalendar.DeletionTime,
                            LastModificationTime = feedbackcalendar.LastModificationTime,
                            LastModifierUserId = feedbackcalendar.LastModifierUserId,
                            CreationTime = feedbackcalendar.CreationTime,
                            CreatorUserId = feedbackcalendar.CreatorUserId
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<NameValueDto>> ListFeedbackState()
        {
            return await this.stateInfoManager.ListFeedbackStates();
        }
    }
}