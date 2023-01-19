
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.Dto;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.BasicData
{
    [AbpAuthorize(PermissionNames.Pages_BasicData)]
    public class MachineGatherParamAppService : BtlCoreAppServiceBase, IMachineGatherParamAppService
    {
        private readonly IMachineManager machineManager;

        private readonly IMachineGatherParamManager machineGatherParamManager;

        private readonly IRepository<MachineGatherParam, long> machineGatherParamRepository;

        public MachineGatherParamAppService(IMachineManager machineManager, IRepository<MachineGatherParam, long> machineGatherParamRepository, IMachineGatherParamManager machineGatherParamManager)
        {
            this.machineManager = machineManager;
            this.machineGatherParamRepository = machineGatherParamRepository;
            this.machineGatherParamManager = machineGatherParamManager;
        }

        [HttpPost]
        public async Task<PagedResultDto<MachineGatherParamsOutputDto>> GetMachineGatherParams(GetGatherParamsInputDto input)
        {
            var machine = await this.machineManager.GetMachineByIdAsync((int)input.MachineId);

            var query = this.machineGatherParamRepository.GetAll().Where(mgp => mgp.MachineId == input.MachineId);

            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Search.Value), q =>
                 q.Code.ToUpper().Trim().Contains(input.Search.Value.ToUpper()) ||
                 q.Name.ToUpper().Trim().Contains(input.Search.Value.ToUpper()));
            var list = await query.ToListAsync();
            if (!list.Any())
            {
                return new DatatablesPagedResultOutput<MachineGatherParamsOutputDto>(0, null, 0) { Draw = input.Draw };
            }

            var returnValue = query.OrderBy(input.Sorting).Skip(input.Start).Take(input.Length).ToList();

            var returnList = ObjectMapper.Map<List<MachineGatherParamsOutputDto>>(returnValue);

            return new DatatablesPagedResultOutput<MachineGatherParamsOutputDto>(
                list.Count,
                returnList,
                list.Count)
            {
                Draw = input.Draw
            };
        }

        public async Task BatchSwitchs(BatchSwitchDto input)
        {
            var paramters = await this.machineGatherParamRepository.GetAll().Where(t => input.ParamIds.Contains(t.Id)).ToListAsync();

            foreach (var item in paramters)
            {
                switch (input.Type)
                {
                    case EnumStateParamType.IsShowForStatus:
                        item.IsShowForStatus = input.Value;
                        break;
                    case EnumStateParamType.IsShowForParam:
                        item.IsShowForParam = input.Value;
                        break;
                    case EnumStateParamType.IsShowForVisual:
                        item.IsShowForVisual = input.Value;
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}