using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Transactions;
using Wimi.BtlCore.Archives.Dtos;
using Wimi.BtlCore.Archives.Repository;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Archives
{
    public class ArchiveEntryAppService : BtlCoreAppServiceBase, IArchiveEntryAppService
    {
        private readonly string archiveByMonth = "yyyy-MM-01";//按月分表，每个班次月的数据归档到一个分表
        private readonly IRepository<ArchiveEntry, int> archiveEntryRepository;
        private readonly IArchiveRepository archiveRepository;

        public ArchiveEntryAppService(
            IRepository<ArchiveEntry, int> archiveentryRepository,
            IArchiveRepository archiveRepository
            )
        {
            this.archiveEntryRepository = archiveentryRepository;
            this.archiveRepository = archiveRepository;
        }

        #region 自动生成代码
        public async Task<DatatablesPagedResultOutput<ArchiveEntryDto>> ListArchiveEntry(ArchiveEntryInputDto input)
        {
            var query = from archiveentry in this.archiveEntryRepository.GetAll()
                        select new ArchiveEntryDto
                        {
                            Id = archiveentry.Id,
                            TargetTable = archiveentry.TargetTable,
                            ArchivedTable = archiveentry.ArchivedTable,
                            ArchiveColumn = archiveentry.ArchiveColumn,
                            ArchiveValue = archiveentry.ArchiveValue,
                            ArchiveCount = archiveentry.ArchiveCount,
                            ArchiveTotalCount = archiveentry.ArchiveTotalCount,
                            ArchivedMessage = archiveentry.ArchivedMessage,
                            LastModificationTime = archiveentry.LastModificationTime,
                            LastModifierUserId = archiveentry.LastModifierUserId,
                            CreationTime = archiveentry.CreationTime,
                            CreatorUserId = archiveentry.CreatorUserId,
                        };


            query = query
                    .WhereIf(!string.IsNullOrEmpty(input.TargetTable), q => q.TargetTable.Contains(input.TargetTable))
                    .WhereIf(!string.IsNullOrEmpty(input.ArchivedTable), q => q.ArchivedTable.Contains(input.ArchivedTable))
                    ;
            var entitiyList = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();
            var entitiyListCount = await query.CountAsync();
            return new DatatablesPagedResultOutput<ArchiveEntryDto>(
                       entitiyListCount,
                       ObjectMapper.Map<List<ArchiveEntryDto>>(entitiyList).ToList(),
                       entitiyListCount)
            {
                Draw = input.Draw
            };
        }

        public async Task Create(ArchiveEntryInputDto input)
        {
            var entity = ObjectMapper.Map<ArchiveEntry>(input);
            await this.archiveEntryRepository.InsertAsync(entity);
        }

        public async Task Update(ArchiveEntryInputDto input)
        {
            var entity = this.archiveEntryRepository.FirstOrDefault(s => s.Id == input.Id);

            entity.TargetTable = input.TargetTable;
            entity.ArchivedTable = input.ArchivedTable;
            entity.ArchiveColumn = input.ArchiveColumn;
            entity.ArchiveValue = input.ArchiveValue;
            entity.ArchiveCount = input.ArchiveCount;
            entity.ArchiveTotalCount = input.ArchiveTotalCount;
            entity.ArchivedMessage = input.ArchivedMessage;

            await this.archiveEntryRepository.UpdateAsync(entity);
        }

        public async Task Delete(ArchiveEntryInputDto input)
        {
            var entity = this.archiveEntryRepository.FirstOrDefault(s => s.Id == input.Id);

            await this.archiveEntryRepository.DeleteAsync(entity);
        }

        public async Task<ArchiveEntryDto> Get(ArchiveEntryInputDto input)
        {
            var query = from archiveentry in this.archiveEntryRepository.GetAll()
                        where archiveentry.Id == input.Id
                        select new ArchiveEntryDto
                        {
                            Id = archiveentry.Id,
                            TargetTable = archiveentry.TargetTable,
                            ArchivedTable = archiveentry.ArchivedTable,
                            ArchiveColumn = archiveentry.ArchiveColumn,
                            ArchiveValue = archiveentry.ArchiveValue,
                            ArchiveCount = archiveentry.ArchiveCount,
                            ArchiveTotalCount = archiveentry.ArchiveTotalCount,
                            ArchivedMessage = archiveentry.ArchivedMessage,
                            LastModificationTime = archiveentry.LastModificationTime,
                            LastModifierUserId = archiveentry.LastModifierUserId,
                            CreationTime = archiveentry.CreationTime,
                            CreatorUserId = archiveentry.CreatorUserId,
                        };

            return await query.FirstOrDefaultAsync();
        }
        #endregion

        #region 测试用
        [UnitOfWork(false)]
        public async Task ArchiveFromStartTimeToEndTime(ArchiveEntryTestDto input)
        {
            if (input.StartTime > input.EndTime)
            {
                throw new UserFriendlyException("开始时间不能大于结束时间");
            }

            for (var st = input.StartTime; st <= input.EndTime; st = st.AddDays(1))
            {
                await this.Archive("Alarms", st);

                await this.Archive("Capacities", st);

                await this.Archive("States", st);

                await this.ArchiveForTraceCatalogs(st);
            }
        }
        #endregion

        #region Alarms,Capacities,States数据表归档

        [UnitOfWork(false)]
        public async Task Archive(string targetTable, DateTime archiveDateTime)
        {
            #region 获取上次归档信息记录，如果没有记录则新增归档信息记录
            var archiveValue = archiveDateTime.ToString("yyyy-MM-dd");
            var startTime = archiveDateTime.ToString(archiveByMonth);
            var archivedTable = $"{targetTable}{startTime}";

            var archiveEntry = await this.archiveEntryRepository.FirstOrDefaultAsync(s => s.TargetTable == targetTable && s.ArchivedTable == archivedTable && s.ArchiveValue == archiveValue);

            if (archiveEntry == null)
            {
                archiveEntry = new ArchiveEntry
                {
                    TargetTable = targetTable,
                    ArchiveColumn = "ShiftDay",
                    ArchiveValue = archiveValue,
                    ArchiveCount = 0,
                    ArchiveTotalCount = 0,
                    ArchivedMessage = string.Empty
                };
            }

            archiveEntry.ArchivedTable = archivedTable;
            #endregion

            try
            {
                #region 查看是否需要归档的数据，没有归档数据，结束任务
                var archiveDatas = archiveRepository.GetArchiveDatas(startTime, archiveEntry);

                if (!archiveDatas.Any())
                {
                    Logger.Info($"{targetTable}-{archiveDateTime}没有归档数据，结束任务");

                    return;
                }
                #endregion

                #region 查看需要归档的数据归属分表是否存在，如果不存在，需要创建分表
                if (!archiveRepository.CheckArchiveTableIsExisted(archiveEntry))
                {
                    archiveRepository.CreateArchiveTable(archiveEntry);
                }
                #endregion

                #region 获取归档表列
                var columns = archiveRepository.GetArchiveTableColumns(archiveEntry);
                #endregion

                using (var ts = new TransactionScope())
                {
                    #region 将归档数据插入分表
                    archiveRepository.InsertDataToArchiveTable(archiveEntry, archiveDatas, columns);
                    #endregion

                    #region 将归档数据从原表删除
                    archiveRepository.DeleteDataFromTargetTable(archiveEntry, archiveDatas);
                    #endregion

                    #region 更新归档信息，如果首次归档，则新增归档信息
                    archiveEntry.ArchiveCount = archiveDatas.Count();
                    archiveEntry.ArchiveTotalCount += archiveDatas.Count();

                    await this.archiveEntryRepository.InsertOrUpdateAsync(archiveEntry);
                    #endregion

                    ts.Complete();
                }

                #region 归档成功，发送消息提示(可选)
                Logger.Error($"{archiveEntry.TargetTable}归档完成");
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error($"{archiveEntry.TargetTable}数据归档发生错误:{ex.Message}");
            }
        }

        #endregion

        #region TraceCatalogs,TraceFLowRecords数据表归档

        [UnitOfWork(false)]
        public async Task ArchiveForTraceCatalogs(DateTime archiveDateTime)
        {
            #region 获取上次归档信息记录，如果没有记录则新增归档信息记录
            var targetTable = "TraceCatalogs";
            var targetTable2 = "TraceFlowRecords";
            var archiveValue = archiveDateTime.ToString("yyyy-MM-dd");
            var startTime = archiveDateTime.ToString(archiveByMonth);
            var archivedTable = $"{targetTable}{startTime}";
            var archivedTable2 = $"{targetTable2}{startTime}";

            var archiveEntry = await this.archiveEntryRepository.FirstOrDefaultAsync(s => s.TargetTable == targetTable && s.ArchivedTable == archivedTable && s.ArchiveValue == archiveValue);
            var archiveEntry2 = await this.archiveEntryRepository.FirstOrDefaultAsync(s => s.TargetTable == targetTable2 && s.ArchivedTable == archivedTable2 && s.ArchiveValue == archiveValue);

            if (archiveEntry == null)
            {
                archiveEntry = new ArchiveEntry
                {
                    TargetTable = targetTable,
                    ArchiveColumn = "ShiftDay",
                    ArchiveValue = archiveValue,
                    ArchiveCount = 0,
                    ArchiveTotalCount = 0,
                    ArchivedMessage = string.Empty
                };

            }

            if (archiveEntry2 == null)
            {
                archiveEntry2 = new ArchiveEntry
                {
                    TargetTable = targetTable2,
                    ArchiveColumn = "ShiftDay",
                    ArchiveValue = archiveValue,
                    ArchiveCount = 0,
                    ArchiveTotalCount = 0,
                    ArchivedMessage = string.Empty
                };
            }

            archiveEntry.ArchivedTable = archivedTable;
            archiveEntry2.ArchivedTable = archivedTable2;
            #endregion

            try
            {
                #region 查看是否需要归档的数据，没有归档数据，结束任务
                var archiveDatas = archiveRepository.GetArchiveDatasTraceCatalogs(startTime, archiveEntry);

                if (!archiveDatas.Item1.Any())
                {
                    Logger.Info($"TraceCatalogs-{archiveDateTime}没有归档数据，结束任务");

                    return;
                }
                #endregion

                #region 查看需要归档的数据归属分表是否存在，如果不存在，需要创建分表
                if (!archiveRepository.CheckArchiveTableIsExisted(archiveEntry))
                {
                    archiveRepository.CreateArchiveTable(archiveEntry);
                }

                if (!archiveRepository.CheckArchiveTableIsExisted(archiveEntry2))
                {
                    archiveRepository.CreateArchiveTable(archiveEntry2);
                }
                #endregion

                #region 获取归档表列
                var columns = archiveRepository.GetArchiveTableColumns(archiveEntry);

                var columns2 = archiveRepository.GetArchiveTableColumns(archiveEntry2);
                #endregion

                using (var ts = new TransactionScope())
                {
                    #region 将归档数据插入分表
                    archiveRepository.InsertDataToArchiveTable(archiveEntry, archiveDatas.Item1.Select(s => s.ToString()).ToList(), columns);

                    archiveRepository.InsertDataToArchiveTable(archiveEntry2, archiveDatas.Item2.Select(s => s.ToString()).ToList(), columns2);
                    #endregion

                    #region 将归档数据从原表删除
                    archiveRepository.DeleteDataFromTargetTable(archiveEntry, archiveDatas.Item1.Select(s => s.ToString()).ToList());

                    archiveRepository.DeleteDataFromTargetTable(archiveEntry2, archiveDatas.Item2.Select(s => s.ToString()).ToList());
                    #endregion

                    #region 更新归档信息，如果首次归档，则新增归档信息
                    archiveEntry.ArchiveCount = archiveDatas.Item1.Count();
                    archiveEntry.ArchiveTotalCount += archiveDatas.Item2.Count();

                    await this.archiveEntryRepository.InsertOrUpdateAsync(archiveEntry);

                    archiveEntry2.ArchiveCount = archiveDatas.Item2.Count();
                    archiveEntry2.ArchiveTotalCount += archiveDatas.Item2.Count();

                    await this.archiveEntryRepository.InsertOrUpdateAsync(archiveEntry2);
                    #endregion

                    ts.Complete();
                }

                #region 归档成功，发送消息提示(可选)
                Logger.Error($"{archiveEntry.TargetTable}归档完成");
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error($"{archiveEntry.TargetTable}数据归档发生错误:{ex.Message}");
            }
        }

        #endregion
    }
}