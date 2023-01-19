using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.CustomFields.Dto;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.CustomFields
{
    public class CustomFieldAppService : BtlCoreAppServiceBase, ICustomFieldAppService
    {
        private readonly IRepository<CustomField> customFieldRepository;
        private readonly IRepository<FmsCutterSetting> fmsCutterSettingRepository;
        private readonly IRepository<FmsCutterExtend> fmsCutterExtendRepository;

        public CustomFieldAppService(IRepository<CustomField> customFieldRepository, IRepository<FmsCutterSetting> fmsCutterSettingRepository, IRepository<FmsCutterExtend> fmsCutterExtendRepository)
        {
            this.customFieldRepository = customFieldRepository;
            this.fmsCutterSettingRepository = fmsCutterSettingRepository;
            this.fmsCutterExtendRepository = fmsCutterExtendRepository;
        }

        public async Task Delete(EntityDto<List<int>> input)
        {
            foreach (var i in input.Id)
            {
                var setting = await this.fmsCutterSettingRepository.GetAsync(i);
                await this.fmsCutterSettingRepository.DeleteAsync(i);

                var field = await this.customFieldRepository.FirstOrDefaultAsync(t => t.Code.Equals(setting.Code));

                if (field != null)
                {
                    await this.customFieldRepository.DeleteAsync(field.Id);
                    await this.fmsCutterExtendRepository.DeleteAsync(t => t.CustomFieldId == field.Id);
                }
            }
        }

        public async Task CreateOrUpdate(CustomFieldDto input)
        {
            await this.CheckName(input);

            if (input.Id.HasValue)
            {
                var entity = await this.customFieldRepository.GetAsync(input.Id.Value);

                entity.Name = input.Name;
                entity.MaxLength = input.MaxLength;
                entity.HtmlTemplate = input.HtmlTemplate;
                entity.RenderHtml = input.RenderHtml;
                entity.IsRequired = input.IsRequired;
            }
            else
            {
                var entity = ObjectMapper.Map<CustomField>(input);  
                entity.Code = CustomField.GetRandomCode();

                var seq = await this.fmsCutterSettingRepository.GetAll().MaxAsync(t=>t.Seq);

                await this.customFieldRepository.InsertAsync(entity);
                await fmsCutterSettingRepository.InsertAsync(new FmsCutterSetting(entity.Code, seq+1, EnumFieldType.Extend, false));
            }
        }

        private async Task CheckName(CustomFieldDto input)
        {
            var result = await this.customFieldRepository.GetAll()
                .WhereIf(input.Id.HasValue, q => q.Id != input.Id.Value)
                .Where(c => c.Name.Trim().ToLower().Equals(input.Name.Trim().ToLower())).ToListAsync();

            if (result.Any())
                throw new UserFriendlyException("已经存在相同名称的记录");

        }
    }
}