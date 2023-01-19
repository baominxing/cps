using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Cutter.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.RealtimeIndicators.Parameters.Dto;
using Wimi.BtlCore.Cutter;
using Microsoft.AspNetCore.Mvc;
using Abp.AutoMapper;
using Wimi.BtlCore.Cutters.Export;

namespace Wimi.BtlCore.Cutter
{
    public class CutterAppService : BtlCoreAppServiceBase, ICutterAppService
    {
        private readonly IRepository<CutterLoadAndUnloadRecord> cutterLoadAndUnloadRecordRepository;

        private readonly IRepository<CutterModel> cutterModelRepository;

        private readonly IRepository<CutterParameter> cutterParameterRepository;

        private readonly IRepository<CutterStates> cutterStatesRepository;

        private readonly IRepository<CutterType> cutterTypeRepository;

        private readonly IRepository<Machine> machineRepository;

        private readonly IUnitOfWorkManager unitOfWorkManager;

        private readonly IRepository<User, long> userRepository;

        private readonly ICutterExporter cutterExporter;

        public CutterAppService(
            IRepository<CutterParameter> cutterParameterRepository,
            IRepository<CutterLoadAndUnloadRecord> cutterLoadAndUnloadRecordRepository,
            IRepository<User, long> userRepository,
            IRepository<CutterStates> cutterStatesRepository,
            IRepository<CutterModel> cutterModelRepository,
            IRepository<CutterType> cutterTypeRepository,
            IRepository<Machine> machineRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ICutterExporter cutterExporter)
        {
            this.cutterParameterRepository = cutterParameterRepository;
            this.cutterTypeRepository = cutterTypeRepository;
            this.cutterModelRepository = cutterModelRepository;
            this.userRepository = userRepository;
            this.cutterLoadAndUnloadRecordRepository = cutterLoadAndUnloadRecordRepository;
            this.cutterStatesRepository = cutterStatesRepository;
            this.cutterModelRepository = cutterModelRepository;
            this.cutterTypeRepository = cutterTypeRepository;
            this.machineRepository = machineRepository;
            this.unitOfWorkManager = unitOfWorkManager;
            this.cutterExporter = cutterExporter;
        }

        /// <summary>
        /// Appends a child code to a parent code.
        ///     Example: if parentCode = "00001", childCode = "00042" then returns "00001.00042".
        /// </summary>
        /// <param name="parentCode">
        /// Parent code. Can be null or empty if parent is a root.
        /// </param>
        /// <param name="childCode">
        /// Child code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string AppendCode(string parentCode, string childCode)
        {
            if (childCode.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(childCode), this.L("ChildCodeCanNotBeNullOrEmpty"));

            if (parentCode.IsNullOrEmpty()) return childCode;

            return parentCode + "." + childCode;
        }

        /// <summary>
        /// Calculates next code for given code.
        ///     Example: if code = "00019.00055.00001" returns "00019.00055.00002".
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CalculateNextCode(string code)
        {
            if (code.IsNullOrEmpty()) throw new ArgumentNullException(nameof(code), this.L("CodeCanNotBeNullOrEmpty"));

            var parentCode = GetParentCode(code);
            var lastUnitCode = GetLastUnitCode(code);

            return AppendCode(parentCode, CreateCode(Convert.ToInt32(lastUnitCode) + 1));
        }

        /// <summary>
        /// Creates code for given numbers.
        ///     Example: if numbers are 4,2 then returns "00004.00002";
        /// </summary>
        /// <param name="numbers">
        /// Numbers
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string CreateCode(params int[] numbers)
        {
            if (numbers.IsNullOrEmpty()) return null;

            return numbers.Select(number => number.ToString(new string('0', AppConsts.CodeUnitLength)))
                .JoinAsString(".");
        }

        /// <summary>
        /// Gets the last unit code.
        ///     Example: if code = "00019.00055.00001" returns "00001".
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [HttpPost]
        public string GetLastUnitCode(string code)
        {
            if (code.IsNullOrEmpty()) throw new ArgumentNullException(nameof(code), this.L("CodeCanNotBeNullOrEmpty"));

            var splittedCode = code.Split('.');
            return splittedCode[splittedCode.Length - 1];
        }

        /// <summary>
        /// Gets parent code.
        ///     Example: if code = "00019.00055.00001" returns "00019.00055".
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [HttpPost]
        public string GetParentCode(string code)
        {
            if (code.IsNullOrEmpty()) throw new ArgumentNullException(nameof(code), this.L("CodeCanNotBeNullOrEmpty"));

            var splittedCode = code.Split('.');
            if (splittedCode.Length == 1) return null;

            return splittedCode.Take(splittedCode.Length - 1).JoinAsString(".");
        }

        /// <summary>
        /// Gets relative code to the parent.
        ///     Example: if code = "00019.00055.00001" and parentCode = "00019" then returns "00055.00001".
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="parentCode">
        /// The parent code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [HttpPost]
        public string GetRelativeCode(string code, string parentCode)
        {
            if (code.IsNullOrEmpty()) throw new ArgumentNullException(nameof(code), this.L("CodeCanNotBeNullOrEmpty"));

            if (parentCode.IsNullOrEmpty()) return code;

            if (code.Length == parentCode.Length) return null;

            return code.Substring(parentCode.Length + 1);
        }

        /// <summary>
        ///     批量卸刀
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task BulkUnLoadCutters(LoadOrUnLoadCuttersDto input)
        {
            try
            {
                foreach (var item in input.SelectedIds)
                {
                    var cutter = await this.cutterStatesRepository.GetAsync(item);
                    if (cutter.CutterUsedStatus != EnumCutterUsedStates.Loading) continue;
                    var operationDto = new LoadOrUnLoadCuttersOperationDto()
                    {
                        CutterTVlaue = cutter.CutterTValue ?? 0,
                        MachineId = cutter.MachineId ?? 0,
                        OperationType = (int)EnumOperationType.Unload,
                        OperatorUserId = this.AbpSession.UserId
                    };
                    this.UnloadCutter(operationDto, cutter);
                }
            }
            catch
            {
                throw new UserFriendlyException(this.L("OperationFailed"));
            }
        }

        /// <summary>
        /// 新增或更新刀具型号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateCutterModel(CutterModelDto input)
        {
            var nameIsExisted = await this.cutterModelRepository.GetAll().WhereIf(input.Id != 0, s => s.Id != input.Id).AnyAsync(s => s.Name == input.Name);

            if (nameIsExisted)
            {
                throw new UserFriendlyException(this.L("ModelNameMustBeUnique"));
            }

            var entity = new CutterModel();

            if (input.Id == 0)
            {
                // input.MapTo(entity);
                entity = ObjectMapper.Map<CutterModel>(input);
                await this.cutterModelRepository.InsertAsync(entity);
            }
            else
            {
                var cutterModel = await this.cutterModelRepository.GetAsync(input.Id);

                ObjectMapper.Map(input, cutterModel);
                await this.cutterModelRepository.UpdateAsync(cutterModel);
            }

            //await this.cutterModelRepository.InsertOrUpdateAsync(entity);
        }

        /// <summary>
        /// 创建或更新刀具参数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateCutterParameter(CutterParameterDto input)
        {
            var nameIsExisted = await this.cutterParameterRepository.GetAll().WhereIf(input.Id != 0, s => s.Id != input.Id).AnyAsync(s => s.Name == input.Name);

            if (nameIsExisted)
            {
                throw new UserFriendlyException(this.L("ParameterNameMustBeUnique"));
            }

            var entity = new CutterParameter();

            if (input.Id == 0)
            {
                // input.MapTo(entity);
                entity = ObjectMapper.Map<CutterParameter>(input);
                await this.cutterParameterRepository.InsertAsync(entity);
            }
            else
            {
                var cutterParameter = await this.cutterParameterRepository.GetAsync(input.Id);
                cutterParameter.Name = input.Name;
                cutterParameter.CreatorUserId = input.CreatorUserId;
                await this.cutterParameterRepository.UpdateAsync(cutterParameter);
            }
        }

        /// <summary>
        ///     新增 和 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateCutterStates(CreateUpdateCutterStatesDto input)
        {
            if (input.Id.HasValue)
            {
                var cutterState = await this.cutterStatesRepository.GetAsync(input.Id.Value);
                if (cutterState != null)
                {
                    //cutterState = ObjectMapper.Map<CutterStates>(input);

                    cutterState.CutterUsedStatus = input.CutterUsedStatus.Value;
                    cutterState.CountingMethod = input.CountingMethod;
                    cutterState.OriginalLife = input.OriginalLife;
                    cutterState.RestLife = input.RestLife;
                    cutterState.WarningLife = input.WarningLife;
                    cutterState.Parameter1 = input.Parameter1;
                    cutterState.Parameter10 = input.Parameter10;
                    cutterState.Parameter2 = input.Parameter2;
                    cutterState.Parameter3 = input.Parameter3;
                    cutterState.Parameter4 = input.Parameter4;
                    cutterState.Parameter5 = input.Parameter5;
                    cutterState.Parameter6 = input.Parameter6;
                    cutterState.Parameter7 = input.Parameter7;
                    cutterState.Parameter8 = input.Parameter8;
                    cutterState.Parameter9 = input.Parameter9;

                    await this.cutterStatesRepository.UpdateAsync(cutterState);
                }
            }
            else
            {
                var entity = ObjectMapper.Map<CutterStates>(input);

                entity.CutterUsedStatus = EnumCutterUsedStates.New;
                entity.CutterLifeStatus = EnumCutterLifeStates.Normal;

                await this.cutterStatesRepository.InsertOrUpdateAsync(entity);
            }
        }

        /// <summary>
        ///     新增或更新刀具类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CutterTypeDto> CreateOrUpdateCutterType(CutterTypeDto input)
        {
            var nameIsExisted = await this.cutterTypeRepository.GetAll().WhereIf(input.Id != 0, s => s.Id != input.Id).AnyAsync(s => s.Name == input.Name);

            if (nameIsExisted)
            {
                throw new UserFriendlyException(this.L("TypeNameMustBeUnique"));
            }

            var cutterIsAssociated = await this.cutterStatesRepository.GetAll().AnyAsync(s => s.CutterTypeId == input.PId);

            if (cutterIsAssociated)
            {
                throw new UserFriendlyException(this.L("CannotAddChildCutterType"));
            }

            var entity = new CutterType();
            if (input.Id == 0)
            {
               // input.MapTo(entity);
                entity = ObjectMapper.Map<CutterType>(input);
                entity.Code = await this.GenerateCode(input);
            }
            else
            {
                entity = await this.cutterTypeRepository.GetAsync(input.Id);

                // 如果刀具类型有所改变,需要重新生成Code
                if (entity.PId != input.PId)
                {
                    entity.Code = await this.GenerateCode(input);
                    entity.PId = input.PId;
                }

                entity.Name = input.Name;
                entity.CutterNoPrefix = input.CutterNoPrefix;
                entity.IsCutterNoPrefixCanEdit = input.IsCutterNoPrefixCanEdit;
            }

            await this.cutterTypeRepository.InsertOrUpdateAsync(entity);

            return ObjectMapper.Map<CutterTypeDto>(entity);
        }

        public async Task CutterLoadOrUnLoad(LoadOrUnLoadCuttersDto input)
        {
            var cutter = await this.cutterStatesRepository.GetAsync(input.Id);
            if (input.OperationType == (int)EnumOperationType.Load)
            {
                this.CutterLoading(input, cutter);
            }
            else
            {
                this.CutterUnLoading(input, cutter);
            }
        }

        /// <summary>
        ///     删除刀具型号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteCutterModel(CutterModelDto input)
        {
            var cutterModel = await this.cutterModelRepository.GetAsync(input.Id);

            var cutterIsAssociated = await this.cutterStatesRepository.GetAll().AnyAsync(s => s.CutterModelId == cutterModel.Id);

            if (cutterIsAssociated)
            {
                throw new UserFriendlyException(this.L("CannotDeleteCutterType"));
            }

            await this.cutterModelRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        ///     删除刀具参数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteCutterParameter(CutterParameterDto input)
        {
            await this.cutterParameterRepository.DeleteAsync(input.Id);
        }

        public async Task DeleteCutterStates(EntityDto input)
        {
            await this.cutterStatesRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        ///     删除刀具类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteCutterType(CutterTypeDto input)
        {
            var cutterIsAssociated = await this.cutterStatesRepository.GetAll().AnyAsync(s => s.CutterTypeId == input.Id);

            if (cutterIsAssociated)
            {
                throw new UserFriendlyException(this.L("CannotDeleteCutterType"));
            }

            var entity = await this.cutterTypeRepository.GetAsync(input.Id);

            // 删除该类型和其所有子类型
            var cutterTypeIdList = (from e in this.cutterTypeRepository.GetAll()
                                    where e.Code.StartsWith(entity.Code)
                                    select e.Id).ToList();

            await this.cutterTypeRepository.DeleteAsync(s => s.Code.Contains(entity.Code));

            // 删除所关联的刀具型号
            await this.cutterModelRepository.DeleteAsync(s => cutterTypeIdList.Any(x => x == s.CutterTypeId));
        }

        public async Task<IEnumerable<NameValueDto<int>>> FindCutterModal(QueryCutterStateDto input)
        {
            return await this.cutterModelRepository.GetAll()
                       .WhereIf(input.CutterModelIds.Count > 0, c => input.CutterModelIds.Contains(c.CutterTypeId))
                       .Select(t => new NameValueDto<int> { Name = t.Name, Value = t.Id }).ToListAsync();
        }

        public async Task<IEnumerable<CutterTypeDto>> FindCutterType()
        {
            var result = await this.cutterTypeRepository.GetAll().ToListAsync();
            return ObjectMapper.Map<IEnumerable<CutterTypeDto>>(result);
        }

        public async Task<CutterTypeDto> FindCutterTypeById(EntityDto input)
        {
            var query = await this.cutterTypeRepository.GetAsync(input.Id);
            return ObjectMapper.Map<CutterTypeDto>(query);
        }

        [HttpPost]
        public async Task<GetCutterModelDefaultDto> GetCutterModelDefaultValue(EntityDto input)
        {
            var cutterModal = await this.cutterModelRepository.GetAsync(input.Id);
            var returnValue = new GetCutterModelDefaultDto();

            if (cutterModal != null)
            {
                returnValue = ObjectMapper.Map<GetCutterModelDefaultDto>(cutterModal);
                returnValue.CutterNo = await this.GetCutterNo(new EntityDto(returnValue.CutterTypeId), input.Id);
            }

            return returnValue;
        }

        /// <summary>
        ///     获取编辑时弹出框数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<CutterModelDto> GetCutterModelForEdit(CutterModelDto input)
        {
            var result = await this.cutterModelRepository.GetAsync(input.Id);

            return ObjectMapper.Map<CutterModelDto>(result);
        }

        /// <summary>
        ///     获取刀具型号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<CutterModelDto>> GetCutterModelList(CutterModelDto input)
        {
            List<CutterModelDto> result;
            using (this.unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                result = (from e in await this.cutterModelRepository.GetAllListAsync()
                          join u in this.userRepository.GetAll() on e.CreatorUserId equals u.Id
                          join u2 in this.userRepository.GetAll() on e.LastModifierUserId equals u2.Id into eu2
                          from eu in eu2.DefaultIfEmpty()
                          where e.CutterTypeId == input.CutterTypeId && !e.IsDeleted
                          select new CutterModelDto
                          {
                              Id = e.Id,
                              CutterTypeId = e.CutterTypeId,
                              Name = e.Name,
                              CountingMethodDisplayText = e.CountingMethod.ToString(),
                              CutterNoPrefix = e.CutterNoPrefix,
                              OriginalLife = e.OriginalLife,
                              WarningLife = e.WarningLife,
                              Parameter1 = e.Parameter1,
                              Parameter10 = e.Parameter10,
                              Parameter2 = e.Parameter2,
                              Parameter3 = e.Parameter4,
                              Parameter4 = e.Parameter4,
                              Parameter5 = e.Parameter5,
                              Parameter6 = e.Parameter6,
                              Parameter7 = e.Parameter7,
                              Parameter8 = e.Parameter8,
                              Parameter9 = e.Parameter9,
                              CreatorUserId = e.CreatorUserId,
                              CreationTime = e.CreationTime,
                              CreatorName = u.Name,
                              LastModifierUserId = e?.LastModifierUserId,
                              LastModificationTime = e?.LastModificationTime,
                              LastModifierName = eu?.Name,
                              ToolLifeCountingMethod=e.CountingMethod == EnumCountingMethod.Time? "Time":"Number"
                          }).ToList();
            }

            return result;
        }

        /// <summary>
        ///     获取所有刀具类型
        /// </summary>
        /// <returns>刀具类型名称和ID的列表</returns>
        [HttpPost]
        public async Task<ListResultDto<CutterModelDto>> GetCutterModels()
        {
            var query = from cm in this.cutterModelRepository.GetAll() select cm;
            var items = await query.ToListAsync();
            return new ListResultDto<CutterModelDto>(items.Select(item => ObjectMapper.Map<CutterModelDto>(item)).ToList());
        }

        /// <summary>
        ///     获取编辑时弹出框数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<CutterParameterDto> GetCutterParameterForEdit(CutterParameterDto input)
        {
            var result = await this.cutterParameterRepository.GetAsync(input.Id);

            return ObjectMapper.Map<CutterParameterDto>(result);
        }

        /// <summary>
        ///     获取刀具参数配置项列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<CutterParameterDto>> GetCutterParameterList()
        {
            List<CutterParameterDto> result;

            using (this.unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                result = (from e in await this.cutterParameterRepository.GetAllListAsync()
                          join u in this.userRepository.GetAll() on e.CreatorUserId equals u.Id
                          join u2 in this.userRepository.GetAll() on e.LastModifierUserId equals u2.Id into eu2
                          from eu in eu2.DefaultIfEmpty()
                          where !e.IsDeleted
                          select new CutterParameterDto
                          {
                              Id = e.Id,
                              Code = e.Code,
                              Name = e.Name,
                              CreatorUserId = e.CreatorUserId,
                              CreationTime = e.CreationTime,
                              CreatorName = u.Name,
                              LastModifierUserId = e?.LastModifierUserId,
                              LastModificationTime = e?.LastModificationTime,
                              LastModifierName = eu?.Name
                          }).ToList();
            }

            return result;
        }

        [HttpPost]

        public async Task<GetCutterStatesColumnsDto> GetCutterStatesColumns()
        {
            return new GetCutterStatesColumnsDto
            {
                DataTablesColumns =typeof(CutterStatesDto).GetProperties()
                                   .Select(
                                       p => new DataTablesColumns
                                       {
                                           Title = p.Name.ToCamelCase().ToString(),
                                           Data = p.Name.ToCamelCase().ToString()
                                       })
                                   .ToList(),
                ParameterMap =
                               (await this.cutterParameterRepository.GetAllListAsync())
                               .Select(p => new NameValueDto(p.Code, p.Name))
                               .ToList()
            };
        }

        /// <summary>
        ///     获取刀具状态表格查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DatatablesPagedResultOutput<CutterStatesDto>> GetCutterStatesList(QueryCutterStateDto input)
        {
            var query = from s in this.cutterStatesRepository.GetAll()
                        join mo in this.cutterModelRepository.GetAll() on s.CutterModelId equals mo.Id
                        join t in this.cutterTypeRepository.GetAll() on s.CutterTypeId equals t.Id
                        join m in this.machineRepository.GetAll() on s.MachineId.Value equals m.Id into g
                        from k in g.DefaultIfEmpty()
                        select new CutterStatesDto
                        {
                            Id = s.Id,
                            CutterNo = s.CutterNo,
                            CutterTypeName = t.Name,
                            CutterTypeId = s.CutterTypeId,
                            CutterModelId = s.CutterModelId,
                            CutterModelName = mo.Name,
                            CutterTValue = s.CutterTValue,
                            MachineId = s.MachineId,
                            MachineName = k.Name,
                            MachineNo = k.Code,
                            CutterUsedStatus = s.CutterUsedStatus,
                            UsedStatusName = s.CutterUsedStatus.ToString(),
                            CutterLifeStatus = s.CutterLifeStatus,
                            LifeStatusName = s.CutterLifeStatus.ToString(),
                            CountingMethod = s.CountingMethod,
                            CountingMethodName = s.CountingMethod.ToString(),
                            OriginalLife = s.OriginalLife,
                            RestLife = s.RestLife,
                            UsedLife = s.UsedLife,
                            WarningLife = s.WarningLife,
                            Parameter1 = s.Parameter1,
                            Parameter2 = s.Parameter2,
                            Parameter3 = s.Parameter3,
                            Parameter4 = s.Parameter4,
                            Parameter5 = s.Parameter5,
                            Parameter6 = s.Parameter6,
                            Parameter7 = s.Parameter7,
                            Parameter8 = s.Parameter8,
                            Parameter9 = s.Parameter9,
                            Parameter10 = s.Parameter10
                        };

            query = query
                .WhereIf(
                    !input.CutterNo.IsNullOrWhiteSpace(),
                    q => q.CutterNo.ToLower().Contains(input.CutterNo.Trim().ToLower()))
                .WhereIf(
                    !input.MachineNo.IsNullOrWhiteSpace(),
                    q => q.MachineNo.ToLower().Contains(input.MachineNo.Trim().ToLower()))
                .WhereIf(input.CutterLifeStateses.Count > 0, q => input.CutterLifeStateses.Distinct().Contains(q.CutterLifeStatus))
                .WhereIf(
                    input.CutterUsedStateses.Count > 0,
                    q => input.CutterUsedStateses.Distinct().Contains(q.CutterUsedStatus.Value))
                .WhereIf(input.CutterTValue.HasValue, q => q.CutterTValue == input.CutterTValue)
                .WhereIf(input.CutterModelIds.Count > 0, q => input.CutterModelIds.Contains(q.CutterModelId))
                .WhereIf(input.CutterTypeIds.Count > 0, q => input.CutterTypeIds.Contains(q.CutterTypeId));

            var result = !input.IsForExport ? query.OrderBy(input.Sorting).AsNoTracking().PageBy(input) : query.OrderBy(input.Sorting).AsNoTracking();
            var cutterStateList = new List<CutterStatesDto>();
            ObjectMapper.Map(result, cutterStateList);

            var count = await query.CountAsync();
            return new DatatablesPagedResultOutput<CutterStatesDto>(
                       count,
                       cutterStateList,
                       count)
            {
                Draw = input.Draw
            };
        }

        /// <summary>
        ///     获取编辑时弹出框数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<CutterTypeDto> GetCutterTypeForEdit(CutterTypeDto input)
        {
            var result = await this.cutterTypeRepository.GetAsync(input.Id);

            return ObjectMapper.Map<CutterTypeDto>(result);
        }

        /// <summary>
        ///     获得所有刀具类型
        /// </summary>
        /// <returns></returns>
       [HttpPost]
        public async Task<IEnumerable<CutterTypeDto>> GetCutterTypeList()
        {
 
            var cutterTypes = await cutterTypeRepository.GetAll().ToListAsync();

            return new List<CutterTypeDto>(
                cutterTypes.Select(
                        cutterType =>
                        {
                            var dto = new CutterTypeDto();
                            ObjectMapper.Map(cutterType, dto);
                            dto.MemberCount = GetMemberCount(cutterType.Id);
                            return dto;
                        })
                    .ToList());


            //var result = from e in this.cutterTypeRepository.GetAll()
            //             join u in this.cutterModelRepository.GetAll() on e.Id equals u.CutterTypeId into eu
            //             select new { e, memberCount = eu.Count() };

            //var items = await result.ToListAsync();

            //return new List<CutterTypeDto>(
            //    items.Select(
            //            item =>
            //                {
            //                    var dto = ObjectMapper.Map<CutterTypeDto>(item.e);
            //                    dto.MemberCount = item.memberCount;
            //                    return dto;
            //                })
            //        .ToList());
        }

        private int GetMemberCount(int typeId)
        {
            return cutterModelRepository.GetAll().Where(d => d.CutterTypeId == typeId).Count();
        }

        /// <summary>
        ///     获取刀具参数
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ListResultDto<CutterParameterDto>> GetDynamicColumns()
        {
            var query = from cp in this.cutterParameterRepository.GetAll() select cp;
            var items = await query.ToListAsync();
            return new ListResultDto<CutterParameterDto>(
                items.Select(item => ObjectMapper.Map<CutterParameterDto>(item)).ToList());
        }

        /// <summary>
        ///     根据选择查询装卸刀记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<CutterLoadAndUnloadRecordDto>> QueryCutterLoadAndUnloadRecords(
            QueryCutterRecordDto input)
        {
            if (input.MachineIdList == null || input.MachineIdList.Count <= 0)
                return new DatatablesPagedResultOutput<CutterLoadAndUnloadRecordDto>();
            var query = from claur in this.cutterLoadAndUnloadRecordRepository.GetAll()
                        join cmr in this.cutterModelRepository.GetAll() on claur.CutterModelId equals cmr.Id
                        join mr in this.machineRepository.GetAll() on claur.MachineId equals mr.Id
                        join ctr in this.cutterTypeRepository.GetAll() on claur.CutterTypeId equals ctr.Id
                        join ur in this.userRepository.GetAll() on claur.CreatorUserId equals ur.Id
                        join opur in this.userRepository.GetAll() on claur.OperatorUserId equals opur.Id
                        select new CutterLoadAndUnloadRecordDto
                        {
                            CutterNo = claur.CutterNo,
                            CutterTypeName = ctr.Name,
                            CutterModelName = cmr.Name,
                            CutterModelId = cmr.Id,
                            CutterTValue = claur.CutterTValue,
                            MachineName = mr.Name,
                            MachineId = mr.Id,
                            OperationType = claur.OperationType,
                            OperationTypeName =
                                           claur.OperationType == 0 ? "卸刀" : "装刀",
                            Parameter1 = claur.Parameter1,
                            Parameter2 = claur.Parameter2,
                            Parameter3 = claur.Parameter3,
                            Parameter4 = claur.Parameter4,
                            Parameter5 = claur.Parameter5,
                            Parameter6 = claur.Parameter6,
                            Parameter7 = claur.Parameter7,
                            Parameter8 = claur.Parameter8,
                            Parameter9 = claur.Parameter9,
                            Parameter10 = claur.Parameter10,
                            CountingMethod = claur.CountingMethod,
                            CountingMethodName =
                                           claur.CountingMethod == 0 ? "按次数" : "按时间",
                            OriginalLife = claur.OriginalLife,
                            UsedLife = claur.UsedLife,
                            RestLife = claur.RestLife,
                            CreationTime = claur.CreationTime,
                            CreatorUserId = ur.CreatorUserId,
                            CreatorUserName = ur.Name,
                            Operator = opur.Name,
                            OperatorTime = claur.OperatorTime
                        };

            query = query
                .WhereIf(
                    !input.CutterNo.IsNullOrWhiteSpace(),
                    q => q.CutterNo.ToLower().Contains(input.CutterNo.Trim().ToLower()))
                .WhereIf(input.StartTime.HasValue, q => q.CreationTime >= input.StartTime.Value)
                .WhereIf(input.EndTime.HasValue, q => q.CreationTime <= input.EndTime.Value)
                .WhereIf(input.CutterModelId.HasValue && input.CutterModelId.Value>0 , q => q.CutterModelId == input.CutterModelId.Value);

            query = query.WhereIf(input.MachineIdList.Any(t => t!=0),
             q => input.MachineIdList.Contains(q.MachineId));

            var result = !input.IsForExport ? query.OrderBy(input.Sorting).AsNoTracking().PageBy(input) : query.OrderBy(input.Sorting).AsNoTracking();

            var count = await query.CountAsync();
            return new DatatablesPagedResultOutput<CutterLoadAndUnloadRecordDto>(
                       count,
                       ObjectMapper.Map<List<CutterLoadAndUnloadRecordDto>>(result),
                       count)
            {
                Draw = input.Draw
            };
        }

        Task ICutterAppService.CreateOrUpdateCutterParameter(CutterParameterDto input)
        {
            return this.CreateOrUpdateCutterParameter(input);
        }

        Task ICutterAppService.DeleteCutterParameter(CutterParameterDto input)
        {
            return this.DeleteCutterParameter(input);
        }

        Task<CutterParameterDto> ICutterAppService.GetCutterParameterForEdit(CutterParameterDto input)
        {
            return this.GetCutterParameterForEdit(input);
        }

        /// <summary>
        ///     生成CutterType的Code字段
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<string> GenerateCode(CutterTypeDto input)
        {
            string nextCode;

            // 获取PId等于input.PId的所有Code
            var subCodes = (from e in this.cutterTypeRepository.GetAll()
                            where e.PId == input.PId
                            orderby e.Code
                            select e.Code).ToList();

            if (input.PId == null)
            {
                // 根类型
                nextCode = CreateCode(subCodes.Count + 1);
            }
            else
            {
                if (subCodes.Count == 0)
                {
                    // 该父类型下第一个子类型
                    var parentCode = (await this.cutterTypeRepository.GetAsync((int)input.PId)).Code;
                    nextCode = AppendCode(parentCode, CreateCode(1));
                }
                else
                {
                    nextCode = CalculateNextCode(subCodes.LastOrDefault());
                }
            }

            return nextCode;
        }

        private CutterLoadAndUnloadRecord GetCutterLoadAndUnloadRecord(
            LoadOrUnLoadCuttersOperationDto input,
            CutterStates cutter)
        {
            return new CutterLoadAndUnloadRecord
            {
                CutterNo = cutter.CutterNo,
                CutterTypeId = cutter.CutterTypeId,
                CutterModelId = cutter.CutterModelId,
                CutterTValue = input.CutterTVlaue,
                MachineId = input.MachineId,
                OperationType = input.OperationType,
                CountingMethod = (int)cutter.CountingMethod,
                OriginalLife = cutter.OriginalLife,
                UsedLife = cutter.UsedLife,
                RestLife = cutter.RestLife,
                OperatorUserId = input.OperatorUserId,
                OperatorTime = DateTime.Now,
                Parameter1 = cutter.Parameter1,
                Parameter2 = cutter.Parameter2,
                Parameter3 = cutter.Parameter3,
                Parameter4 = cutter.Parameter4,
                Parameter5 = cutter.Parameter5,
                Parameter6 = cutter.Parameter6,
                Parameter7 = cutter.Parameter7,
                Parameter8 = cutter.Parameter8,
                Parameter9 = cutter.Parameter9,
                Parameter10 = cutter.Parameter10
            };
        }

        /// <summary>
        /// 系统自动生成刀具编号
        /// </summary>
        /// <param name="input">
        /// </param>
        /// <param name="cutterModalId">
        /// The cutter Modal Id.
        /// </param>
        /// <returns>
        /// </returns>
        private async Task<string> GetCutterNo(EntityDto input, int cutterModalId)
        {
            var cutterNo = string.Empty;
            var statesMaxId = 0;
            using (this.unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var statesModal = from cm in this.cutterModelRepository.GetAll()
                                  join ct in this.cutterTypeRepository.GetAll() on cm.CutterTypeId equals ct.Id
                                  join cs in this.cutterStatesRepository.GetAll() on cm.Id equals cs.CutterModelId
                                  where cs.CutterTypeId == input.Id
                                  select cs;

                if (statesModal.Any()) statesMaxId = statesModal.Max(m => m.Id);

                var cutterModal = await this.cutterModelRepository.GetAsync(cutterModalId);

                if (statesMaxId > 0)
                {
                    var statesEntity = await this.cutterStatesRepository.GetAsync(statesMaxId);
                    var maxSerialNoCutterModal = await this.cutterModelRepository.GetAsync(statesEntity.CutterModelId);
                    var serialNo = statesEntity.CutterNo.Substring(maxSerialNoCutterModal.CutterNoPrefix.Length);

                    int numSerialNo;
                    if (int.TryParse(serialNo, out numSerialNo)) cutterNo = cutterModal.GetCutterNo(numSerialNo);
                }
                else
                {
                    cutterNo = cutterModal.GetCutterNo();
                }
            }

            return cutterNo;
        }

        private void CutterLoading(LoadOrUnLoadCuttersDto input, CutterStates cutter)
        {
            var cutterEntity = this.cutterStatesRepository.FirstOrDefault(c=>c.MachineId == input.MachineId && c.CutterTValue == input.CutterTVlaue);
            if (cutterEntity != null)
            {
                throw new UserFriendlyException(L("CutterTVauleCantLoadingMore"));
            }

            if (cutter.CutterUsedStatus == EnumCutterUsedStates.Loading)
            {
                // 自动卸刀
                var loadDto = new LoadOrUnLoadCuttersOperationDto()
                {
                    CutterTVlaue = cutter.CutterTValue ?? 0,
                    OperationType = (int)EnumOperationType.Unload,
                    OperatorUserId = input.OperatorUserId,
                    MachineId = cutter.MachineId ?? 0
                };
                this.UnloadCutter(loadDto, cutter);
            }

            // 装刀
            var unloadDto = ObjectMapper.Map<LoadOrUnLoadCuttersOperationDto>(input);
            this.LoadingCutter(unloadDto, cutter);
        }

        private void CutterUnLoading(LoadOrUnLoadCuttersDto input, CutterStates cutter)
        {
            if (cutter.CutterUsedStatus != EnumCutterUsedStates.Loading)
                throw new UserFriendlyException(this.L("TheToolCannotBeUnloaded"));

            var dto = ObjectMapper.Map<LoadOrUnLoadCuttersOperationDto>(input);
            this.UnloadCutter(dto, cutter);
        }

        // 装刀
        private async void LoadingCutter(LoadOrUnLoadCuttersOperationDto input, CutterStates cutter)
        {
            cutter.CutterUsedStatus = EnumCutterUsedStates.Loading;
            cutter.MachineId = input.MachineId;
            cutter.CutterTValue = input.CutterTVlaue;

            var record = this.GetCutterLoadAndUnloadRecord(input, cutter);
            await this.cutterLoadAndUnloadRecordRepository.InsertAsync(record);
        }

        // 卸刀
        private async void UnloadCutter(LoadOrUnLoadCuttersOperationDto input, CutterStates cutter)
        {
            cutter.CutterUsedStatus = EnumCutterUsedStates.NotLoad;
            cutter.MachineId = null;
            cutter.CutterTValue = null;

            var record = this.GetCutterLoadAndUnloadRecord(input, cutter);
            await this.cutterLoadAndUnloadRecordRepository.InsertAsync(record);
        }

        public async Task<IEnumerable<ListLoadingMachineCuttersDto>> ListLoadingMachineCutters()
        {
            var query = this.cutterStatesRepository.GetAll().Where(c => c.MachineId != null && c.CutterTValue != null)
                .Join(this.machineRepository.GetAll(), c => c.MachineId, m => m.Id, (c, m) => new { c, m })
                .Select(q => new
                {
                    MachineId = (int)q.c.MachineId,
                    MachineName = q.m.Name,
                    MachineCode = q.m.Code,
                    TValue = q.c.CutterTValue,
                    Rate = q.c.Rate
                });

            var groupQuery = from q in query
                         group q by new { q.MachineId, q.MachineCode, q.MachineName } into g
                         select new ListLoadingMachineCuttersDto
                         {
                             MachineId = g.Key.MachineId,
                             MachineName = g.Key.MachineName,
                             MachineCode = g.Key.MachineCode,
                             Details = g.Select(x => new LoadingMachineCutterDetails() { Rate = x.Rate, TValue = (int)x.TValue }).ToList()
                         };

            var result = await groupQuery.ToListAsync();

            if (result.Count()<=0)
            {
                throw new UserFriendlyException(L("NotExistLoadingCutter"));
            }

            return result;
        }

        public async Task<IEnumerable<ListLoadingMachineCuttersDto>> ListLoadingMachines()
        {
            var query = this.cutterStatesRepository.GetAll().Where(c => c.MachineId != null && c.CutterTValue != null)
                .Join(this.machineRepository.GetAll(), c => c.MachineId, m => m.Id, (c, m) => new { c, m })
                .Select(q => new ListLoadingMachineCuttersDto
                {
                    MachineId = (int)q.c.MachineId,
                    MachineName = q.m.Name,
                    MachineCode = q.m.Code,
                });

            var result = await query.Distinct().ToListAsync();

            if (result.Count() <= 0)
            {
                throw new UserFriendlyException(L("NotExistLoadingCutter"));
            }

            return result;
        }

        public async Task<IEnumerable<LoadingMachineCutterDetails>> ListMachineCutterDetails(EntityDto<string> input)
        {
            var machine = await this.machineRepository.FirstOrDefaultAsync(m => m.Name == input.Id);

            var query = this.cutterStatesRepository.GetAll().Where(c => c.MachineId == machine.Id && c.CutterTValue != null)               
                .Select(q => new LoadingMachineCutterDetails
                {
                    Rate = q.Rate,
                    TValue = (int)q.CutterTValue
                });

            var result = await query.Distinct().ToListAsync();

            return result;
        }

        public async Task SaveMachineCutterRates(SaveMachineCutterRatesInputDto input)
        {
            foreach (var item in input.MachineCutterRates)
            {
                var machine = await this.machineRepository.FirstOrDefaultAsync(m => m.Name == item.MachineName);
                if (machine != null)
                {
                    foreach (var cutterRate in item.CutterRates)
                    {
                        var cutterState = await this.cutterStatesRepository.FirstOrDefaultAsync(c=>c.CutterTValue == cutterRate.TValue && c.MachineId == machine.Id);
                        if (cutterState!=null)
                        {
                            cutterState.Rate = cutterRate.Rate;
                            await this.cutterStatesRepository.UpdateAsync(cutterState);
                        }                      
                    }
                }
            }
        }

        public async Task ResetCutterLife(EntityDto input)
        {
            var cutterState = await this.cutterStatesRepository.FirstOrDefaultAsync(c=>c.Id == input.Id);

            if (cutterState != null)
            {
                cutterState.UsedLife = 0;
                cutterState.RestLife = cutterState.OriginalLife;
                cutterState.CutterLifeStatus = EnumCutterLifeStates.Normal;
                await this.cutterStatesRepository.UpdateAsync(cutterState);
            }
        }

        public async Task<FileDto> ExportCutterLoadAndUnloadRecords(QueryCutterRecordDto input)
        {
            input.IsForExport = true;

            var result = await this.QueryCutterLoadAndUnloadRecords(input);

            return cutterExporter.ExportToFile(result.Items.ToList(), input.StartTime.Value, input.EndTime.Value, input);
        }

        public async Task<FileDto> ExportCutterStatesList(QueryCutterStateDto input)
        {
            input.IsForExport = true;

            var result = await this.GetCutterStatesList(input);

            return cutterExporter.ExportToFile(result.Items.ToList(), input);
        }
    }
}