using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.CraftMaintain.Dtos;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.FmsCutter.Dto;
using Wimi.BtlCore.FmsCutters;
using System.Linq.Dynamic.Core;
using Wimi.BtlCore.CustomFields.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Wimi.BtlCore.CraftMaintain
{
    public class FmsCutterAppService : BtlCoreAppServiceBase, IFmsCutterAppService
    {
        private readonly IRepository<FmsCutters.FmsCutter> fmscutterRepository;
        private readonly IRepository<CustomField> customFileldRepository;
        private readonly IRepository<FmsCutterSetting> fmsCutterSettingRepository;
        private readonly IRepository<FmsCutterExtend> fmsCutterSettingExtendRepository;


        public FmsCutterAppService(IRepository<FmsCutters.FmsCutter> fmscutterRepository, IRepository<CustomField> customFileldRepository, IRepository<FmsCutterSetting> fmsCutterSettingRepository, IRepository<FmsCutterExtend> fmsCutterSettingExtendRepository)
        {
            this.fmscutterRepository = fmscutterRepository;
            this.customFileldRepository = customFileldRepository;
            this.fmsCutterSettingRepository = fmsCutterSettingRepository;
            this.fmsCutterSettingExtendRepository = fmsCutterSettingExtendRepository;
        }

        public async Task<DatatablesPagedResultOutput<FmsCutterDto>> ListFmsCutter(FmsCutterInputDto input)
        {
            var defaultExtendFileds = await this.fmsCutterSettingRepository.GetAll().Where(t => t.Type == EnumFieldType.Extend)
                .Select(t => new FmsCutterExtendDto
                {
                    Code = t.Code,
                    FieldValue = string.Empty

                }).ToListAsync();

            var query = from fmscutter in this.fmscutterRepository.GetAll()
                select new FmsCutterDto
                {
                    Id = fmscutter.Id,
                    MachineId = fmscutter.MachineId,
                    CutterNo = fmscutter.CutterNo,
                    CutterCase = fmscutter.CutterCase,
                    CutterStockNo = fmscutter.CutterStockNo,
                    Type = fmscutter.Type,
                    Length = fmscutter.Length,
                    Diameter = fmscutter.Diameter,
                    CompensateNo = fmscutter.CompensateNo,
                    LengthCompensate = fmscutter.LengthCompensate,
                    DiameterCompensate = fmscutter.DiameterCompensate,
                    OriginalLife = fmscutter.OriginalLife,
                    CurrentLife = fmscutter.CurrentLife,
                    WarningLife = fmscutter.WarningLife,
                    UseType = fmscutter.UseType,
                    CountType = fmscutter.CountType,
                    State = fmscutter.State,
                    CustomFileds = fmscutter.Items.Select(t => new FmsCutterExtendDto()
                    {
                        Code = t.CustomField.Code,
                        CustomFieldId = t.CustomFieldId,
                        FieldValue = t.FieldValue
                    })
                };

            query = query.WhereIf(!input.CutterNo.IsNullOrEmpty(), q => q.CutterNo.Contains(input.CutterNo))
                .WhereIf(!input.CutterCase.IsNullOrEmpty(), q => q.CutterCase.Contains(input.CutterCase))
                .WhereIf(!input.Type.IsNullOrEmpty(), q => q.Type.Contains(input.Type));

            var entitiyList = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();
            var entitiyListCount = await query.CountAsync();

            // 设置默认
            entitiyList.ForEach(t => { t.CustomFileds = t.CustomFileds.Any() ? t.CustomFileds : defaultExtendFileds; });

            return new DatatablesPagedResultOutput<FmsCutterDto>(
                       entitiyListCount,
                       ObjectMapper.Map<List<FmsCutterDto>>(entitiyList),
                       entitiyListCount)
            {
                Draw = input.Draw
            };
        }

        public async Task CreateFmsCutter(FmsCutterDto input)
        {
            var entity = ObjectMapper.Map<FmsCutters.FmsCutter>(input); 
         
            var id = await this.fmscutterRepository.InsertAndGetIdAsync(entity);

            foreach (var item in input.ExtendFields)
            {
                var field = new FmsCutterExtend
                {
                    FmsCutterId = id,
                    CustomFieldId = item.Value,
                    FieldValue = item.Name.IsNullOrEmpty()? string.Empty: item.Name.Trim(',')
                };
                await this.fmsCutterSettingExtendRepository.InsertAsync(field);
            }
        }

        public async Task UpdateFmsCutter(FmsCutterDto input)
        {
            var entity = this.fmscutterRepository.FirstOrDefault(s => s.Id == input.Id);

            entity.CutterNo = input.CutterNo;
            entity.CutterCase = input.CutterCase;
            entity.CutterStockNo = input.CutterStockNo;
            entity.Type = input.Type;
            entity.OriginalLife = input.OriginalLife;
            entity.WarningLife = input.WarningLife;
            entity.CountType = input.CountType;

            await this.fmscutterRepository.UpdateAsync(entity);

            // 处理拓展栏位
            foreach (var item in input.ExtendFields)
            {
                var field = await this.fmsCutterSettingExtendRepository.FirstOrDefaultAsync(t =>
                    t.FmsCutterId == entity.Id && t.CustomFieldId == item.Value);

                if (field != null)
                {
                    field.FieldValue = item.Name.IsNullOrEmpty() ? string.Empty : item.Name.Trim(',');
                }
                else
                {
                    var extend = new FmsCutterExtend
                    {
                        FmsCutterId = entity.Id,
                        CustomFieldId = item.Value,
                        FieldValue = item.Name.IsNullOrEmpty() ? string.Empty : item.Name.Trim(',')
                    };

                    await this.fmsCutterSettingExtendRepository.InsertAsync(extend);

                }
            }
        }

        public async Task DeleteFmsCutter(FmsCutterDto input)
        {
            var entity = this.fmscutterRepository.FirstOrDefault(s => s.Id == input.Id);

            await this.fmscutterRepository.DeleteAsync(entity);
            await this.fmsCutterSettingExtendRepository.DeleteAsync(t => t.FmsCutterId == entity.Id);
        }

        [HttpPost]
        public async Task<FmsCutterDto> GetFmsCutterForEdit(FmsCutterInputDto input)
        {
            var query = from fmscutter in this.fmscutterRepository.GetAll()
                where fmscutter.Id == input.Id
                select new FmsCutterDto
                {
                    Id = fmscutter.Id,
                    MachineId = fmscutter.MachineId,
                    CutterNo = fmscutter.CutterNo,
                    CutterStockNo = fmscutter.CutterStockNo,
                    CutterCase = fmscutter.CutterCase,
                    Type = fmscutter.Type,
                    Length = fmscutter.Length,
                    Diameter = fmscutter.Diameter,
                    CompensateNo = fmscutter.CompensateNo,
                    LengthCompensate = fmscutter.LengthCompensate,
                    DiameterCompensate = fmscutter.DiameterCompensate,
                    OriginalLife = fmscutter.OriginalLife,
                    CurrentLife = fmscutter.CurrentLife,
                    WarningLife = fmscutter.WarningLife,
                    UseType = fmscutter.UseType,
                    CountType = fmscutter.CountType,
                    State = fmscutter.State,
                    CustomFileds = fmscutter.Items.Select(t => new FmsCutterExtendDto()
                    {
                        FieldValue = t.FieldValue,
                        CustomFieldId = t.CustomFieldId
                    })
                };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CustomFieldDto>> ListCutomFields()
        {
            var query = await this.fmsCutterSettingRepository.GetAll().Where(t => t.Type == EnumFieldType.Extend && t.IsShow).Join(
                this.customFileldRepository.GetAll(), f => f.Code, c => c.Code, (f, c) => new
                {
                    f,
                    c
                }).Select(t => new CustomFieldDto()
                {
                    Id = t.c.Id,
                    Code = t.c.Code,
                    DisplayType = t.c.DisplayType,
                    RenderHtml = t.c.RenderHtml,
                    IsRequired = t.c.IsRequired,
                    Name = t.c.Name,
                    MaxLength = t.c.MaxLength,
                    
                }).ToListAsync();

            return query;
        }
    }
}


