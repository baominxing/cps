using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using Wimi.BtlCore.CraftMaintain.Dtos;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.CraftMaintain
{
    public class FmsCutterSettingAppService : BtlCoreAppServiceBase, IFmsCutterSettingAppService
    {
        private readonly IRepository<FmsCutterSetting> fmsCutterSettingRepository;
        private readonly IRepository<CustomField> customFieldRepository;

        public FmsCutterSettingAppService(IRepository<FmsCutterSetting> fmsCutterSettingRepository, IRepository<CustomField> customFieldRepository)
        {
            this.fmsCutterSettingRepository = fmsCutterSettingRepository;
            this.customFieldRepository = customFieldRepository;
        }

        public async Task<IEnumerable<FmsCutterSettingItemDto>> ListColumns()
        {
            var query = from f in this.fmsCutterSettingRepository.GetAll()
                        join t in customFieldRepository.GetAll() on f.Code equals t.Code into g
                        from k in g.DefaultIfEmpty()
                        select new FmsCutterSettingItemDto()
                        {
                            Code = f.Code,
                            Id = f.Id,
                            IsShow = f.IsShow,
                            Seq = f.Seq,
                            Type = f.Type,
                            Name = k != null ? k.Name : string.Empty
                        };

            var resut = await query.Where(t=>t.IsShow).ToListAsync();
            resut.ForEach(r =>
            {
                r.Name = r.Name.IsNullOrEmpty() ? L(r.Code) : r.Name;

            });
            return resut.OrderBy(t => t.Seq);
        }

        public async Task CreateOrUpdate(FmsCutterSettingItemDto input)
        {
            if (input.Id.HasValue)
            {
                var entity = await this.fmsCutterSettingRepository.GetAsync(input.Id.Value);

                entity.IsShow = input.IsShow;
                entity.Seq = input.Seq;

            }
            else
            {
                await this.fmsCutterSettingRepository.InsertAsync(ObjectMapper.Map<FmsCutterSetting>(input));
            }
        }

        [HttpPost]
        public async Task<FmsCutterSettingDto> GetSettingDto()
        {
            var query = this.fmsCutterSettingRepository.GetAll().AsEnumerable()
                .GroupBy(t => t.Type, (key, g) => new
                {
                    Type = key,
                    List = g.Select(t => new FmsCutterSettingItemDto()
                    {
                        Id = t.Id,
                        Code = t.Code,
                        IsShow = t.IsShow,
                        Seq = t.Seq,
                        Type = t.Type,
                        Name = this.L(t.Code)
                    })
                }).ToList();



            var basicFields = query.FirstOrDefault(t => t.Type == EnumFieldType.Basics)?.List;
            if (basicFields != null)
            {
                foreach (var field in basicFields)
                {
                    field.Name = this.L(field.Code);
                }
            }

            var extendFields = query.FirstOrDefault(t => t.Type == EnumFieldType.Extend)?.List.ToList();

            if (extendFields != null && extendFields.Any())
            {
                var codeList = extendFields.Select(t => t.Code);
                var result = await this.customFieldRepository.GetAll().Where(t => codeList.Contains(t.Code))
                    .ToListAsync();

                extendFields.ForEach(r =>
                {
                    var name = result.FirstOrDefault(t => t.Code.Equals(r.Code))?.Name;
                    r.SetName(name);
                });
            }

            return new FmsCutterSettingDto
            {
                BasicFields = basicFields,
                ExtendFields = extendFields
            };

        }

        public async Task Update(IEnumerable<NameValueDto<int>> input)
        {
            // 全部失效
            var allEntity = await this.fmsCutterSettingRepository.GetAll().ToListAsync();
            foreach (var item in allEntity)
            {
                var result = input.FirstOrDefault(t => t.Name.ToLower().Equals(item.Code.ToLower()));
                if (result != null)
                {
                    item.IsShow = true;
                    item.Seq = result.Value;
                }
                else
                {
                    item.IsShow = false;
                    item.Seq = 0;
                }
            }
        }
    }
}