using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.CraftMaintain.Dtos;

namespace Wimi.BtlCore.CraftMaintain
{
    [AbpAuthorize(PermissionNames.Pages_CraftMaintain_FlexibleCraftPath)]
    public class FlexibleCraftMaintainAppService : BtlCoreAppServiceBase, IFlexibleCraftMaintainAppService
    {
        private readonly FlexibleCraftMaintainManager craftMaintainManager;
        public FlexibleCraftMaintainAppService(FlexibleCraftMaintainManager craftMaintainManager)
        {
            this.craftMaintainManager = craftMaintainManager;
        }

        [HttpPost]
        public async Task<List<GetCraftsDto>> GetCrafts(GetCraftsInput input)
        {
            var result = new List<GetCraftsDto>();
            var crafts = await craftMaintainManager.GetCrafts(input.ProductId);

            foreach (var craft in crafts)
            {
                var craftEntity = await craftMaintainManager.GetCraft(craft.Id);
                var dto =  ObjectMapper.Map<GetCraftsDto>(craftEntity);

                foreach (var item in craftEntity.CraftProcesses)
                {
                    dto.CraftProcesses.Add(
                        new FlexibleCraftProcesseDto()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Sequence = item.Sequence,
                            TongName = item.Tong.Name,
                            Programes = item.Tong.ToProgramString()
                        });
                }

                result.Add(dto);
            }

            return result;
        }

        [HttpPost]
        public async Task<List<GetAllCraftsDto>> GetAllCrafts(GetCraftsInput input)
        {
            var crafts = await craftMaintainManager.GetCrafts(input.ProductId);
            return ObjectMapper.Map<List<GetAllCraftsDto>>(crafts);
        }

        [HttpPost]
        public async Task<GetCraftCuttersDto> GetCraftCutters(GetCraftCuttersInput input)
        {
            var result = new GetCraftCuttersDto();
            var craft = await craftMaintainManager.GetCraft(input.CraftId);

            var craftProcedureCutterMaps = await craftMaintainManager.GetCraftProcedureCutterMapsByCraftId(input.CraftId);
            BuildGetCraftPathForCutterDto(craft.CraftProcesses.ToList(), craftProcedureCutterMaps, result.CraftPathCutters, input.CraftId);

            return result;
        }

        public async Task CreatOrEditCraft(CreatCraftInput input)
        {
            var craft = ObjectMapper.Map<FlexibleCraft>(input);
            if (await craftMaintainManager.CheckCraft(craft))
            {
                throw new UserFriendlyException("工艺名称不能重复");
            }

            var craftProcedureCutters = ObjectMapper.Map<List<FlexibleCraftProcedureCutterMap>>(input.CraftProcedureCutters);
            input.CraftProcesseIds.ForEach(f =>
            {
                craft.CraftProcesses.Add(new FlexibleCraftProcesse()
                {
                    Id = f
                });
            });

            if (input.Id.HasValue)
            {
                await craftMaintainManager.EditCraft(craft, craftProcedureCutters);
            }
            else
            {
                await craftMaintainManager.CreatCraft(craft, craftProcedureCutters);
            }
        }

        public async Task<StepOneDto> StepOne(EntityDto input)
        {
            var craft = await craftMaintainManager.GetCraft(input.Id);
            var result = ObjectMapper.Map<StepOneDto>(craft);

            foreach (var item in craft.CraftProcesses)
            {
                result.Processes.Add(
                    new FlexibleCraftProcesseDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Sequence = item.Sequence,
                        TongName = item.Tong.Name,
                        Programes = item.Tong.ToProgramString()
                    });
            }

            return result;
        }

        public async Task<List<CraftPathCutterDto>> StepTwo(StepTwoInput input)
        {
            var result = new List<CraftPathCutterDto>();
            if (input.CraftId.HasValue)
            {
                var craftProcedureCutterMaps = await craftMaintainManager.GetCraftProcedureCutterMapsByCraftId(input.CraftId.Value);
                var craftProcesses = await craftMaintainManager.GetFlexibleCraftProcessesByIds(input.CraftProcesseIds);

                BuildGetCraftPathForCutterDto(craftProcesses, craftProcedureCutterMaps, result, input.CraftId.Value);
            }
            else
            {
                var craftProcesses = await craftMaintainManager.GetFlexibleCraftProcessesByIds(input.CraftProcesseIds);

                BuildGetCraftPathForCutterDto(craftProcesses, null, result);
            }

            return result;
        }

        /// <summary>
        /// 根据程序号构建程序号路径
        /// </summary>
        /// <param name="craftProcesses">用户选择的工序</param>
        /// <param name="craftProcedureCutterMaps">程序号和刀具关系</param>
        /// <param name="cutterContainer"></param>
        private void BuildGetCraftPathForCutterDto(List<FlexibleCraftProcesse> craftProcesses,
            List<FlexibleCraftProcedureCutterMap> craftProcedureCutterMaps,
            List<CraftPathCutterDto> cutterContainer, int? CraftId = null)
        {
            foreach (var craftProcesse in craftProcesses)
            {
                var programes = new List<string>();
                programes.Add(craftProcesse.Tong.ProgramA);
                programes.Add(craftProcesse.Tong.ProgramB);
                programes.Add(craftProcesse.Tong.ProgramC);
                programes.Add(craftProcesse.Tong.ProgramD);
                programes.Add(craftProcesse.Tong.ProgramE);
                programes.Add(craftProcesse.Tong.ProgramF);

                foreach (var item in programes)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var dto = new CraftPathCutterDto()
                        {
                            CraftProcesseId = craftProcesse.Id,
                            CraftProcesseName = craftProcesse.Name,
                            ProcedureNumber = item
                        };
                        List<FlexibleCraftProcedureCutterMap> maps = null;

                        if (CraftId.HasValue)
                        {
                            maps = craftProcedureCutterMaps?.Where(x => x.CraftId == CraftId.Value
                                && x.CraftProcesseId.Equals(craftProcesse.Id)
                                && x.ProcedureNumber.Equals(item)
                                ).ToList();
                        }
                        else
                        {
                            maps = craftProcedureCutterMaps?
                                     .Where(x => x.CraftProcesseId.Equals(craftProcesse.Id)
                                    && x.ProcedureNumber.Equals(item)
                                ).ToList();
                        }

                        foreach (var m in maps ?? new List<FlexibleCraftProcedureCutterMap>())
                        {
                            dto.CutterDetails.Add(ObjectMapper.Map<GetCraftPathForCutterDetailDto>(m.Cutter));
                        }

                        cutterContainer.Add(dto);
                    }
                }
            }
        }

        public async Task<FlexibleCraftProcesseDto> CreateOrEditCraftProcesse(CreateCraftProcesseInput input)
        {
            var entity =ObjectMapper.Map<FlexibleCraftProcesse>(input);
            if (await craftMaintainManager.CheckCraftProcesse(entity))
            {
                throw new UserFriendlyException("工序名称不能重复");
            }

            var result = await craftMaintainManager.CreateOrEditCraftProcesse(entity);

            return new FlexibleCraftProcesseDto()
            {
                Id = result.Id,
                Name = result.Name,
                Sequence = result.Sequence,
                TongName = result.Tong.Name,
                Programes = result.Tong.ToProgramString()
            };
        }

        [HttpPost]
        public async Task<PagedResultDto<FlexibleCraftProcesseDto>> GetCraftProcesses(GetCraftProcessesInput input)
        {
            var processes = await craftMaintainManager.GetCraftProcesses(input.CraftId, input.Search.Value);

            return new PagedResultDto<FlexibleCraftProcesseDto>(processes.Count,
                processes.Select(s => new FlexibleCraftProcesseDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Sequence = s.Sequence,
                    TongName = s.Tong.Name,
                    Programes = s.Tong.ToProgramString()
                }).ToList());
        }

        public async Task DeleteCraft(EntityDto input)
        {
            await craftMaintainManager.DeleteCraft(input.Id);
        }

        /// <summary>
        /// 获取工艺路线图数据
        /// </summary>
        /// <param name="input">工艺Id</param>
        /// <returns></returns>
       [HttpPost]
        public async Task<CraftPathMapData> GetCraftPathMapData(EntityDto input)
        {
            var result = new CraftPathMapData();
            var craftEntity = await craftMaintainManager.GetCraft(input.Id);
            result.Name = craftEntity.Name;

            var craftProcedureCutterMaps = await craftMaintainManager.GetCraftProcedureCutterMapsByCraftId(input.Id);

            foreach (var item in craftEntity.CraftProcesses)
            {
                var processe = new CraftPathMapDataProcesse()
                {
                    Id = item.Id,
                    Name = item.Name
                };

                var tong = new CraftPathMapDataTong() { Name = item.Tong.Name };

                tong.Children.AddRange(item.Tong.GetPrograms().Select(s => new CraftPathMapDataProgram()
                {
                    Name = $"{s.Key}:{s.Value}",
                    ProcedureNumber = s.Value
                }));

                processe.Children.Add(tong);

                result.Children.Add(processe);

                foreach (var program in tong.Children)
                {
                    var maps = craftProcedureCutterMaps.FindAll(x => x.CraftId.Equals(craftEntity.Id)
                                && x.CraftProcesseId.Equals(processe.Id)
                                && x.ProcedureNumber.Equals(program.ProcedureNumber));

                    program.Children.AddRange(maps.Select(s => new CraftPathMapDataCutter(s.Cutter.CutterNo, s.Cutter.Type)));
                }
            }

            return result;
        }
    }
}
