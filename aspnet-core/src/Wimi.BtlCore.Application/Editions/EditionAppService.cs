using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

using Abp;
using Abp.Application.Editions;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Editions.Dto;
using Wimi.BtlCore.Editions.Exporting;

namespace Wimi.BtlCore.Editions
{
    [AbpAuthorize(PermissionNames.Pages_Editions)]
    public class EditionAppService : BtlCoreAppServiceBase, IEditionAppService
    {
        private readonly EditionManager editionManager;

        private readonly IRepository<Edition> editionRepository;

        private readonly IEditionsListExcelExporter editionsListExcelExporter;

        public EditionAppService(
            EditionManager editionManager,
            IRepository<Edition> editionRepository,
            IEditionsListExcelExporter editionsListExcelExporter)
        {
            this.editionManager = editionManager;
            this.editionRepository = editionRepository;
            this.editionsListExcelExporter = editionsListExcelExporter;
        }

        [AbpAuthorize(PermissionNames.Pages_Editions_Create, PermissionNames.Pages_Editions_Edit)]
        public async Task CreateOrUpdateEdition(CreateOrUpdateEditionDto input)
        {
            if (!input.Edition.Id.HasValue)
            {
                await this.CreateEditionAsync(input);
            }
            else
            {
                await this.UpdateEditionAsync(input);
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Editions_Delete)]
        public async Task DeleteEdition(EntityDto input)
        {
            var edition = await this.editionManager.GetByIdAsync(input.Id);
            await this.editionManager.DeleteAsync(edition);
        }

        [HttpPost]
        public async Task<List<ComboboxItemDto>> GetEditionComboboxItems(int? selectedEditionId = null)
        {
            var editions = await this.editionManager.Editions.ToListAsync();
            var editionItems =
                new ListResultDto<ComboboxItemDto>(
                    editions.Select(e => new ComboboxItemDto(e.Id.ToString(), e.DisplayName)).ToList()).Items.ToList();

            var defaultItem = new ComboboxItemDto("null", this.L("NotAssigned"));
            editionItems.Insert(0, defaultItem);

            if (selectedEditionId.HasValue)
            {
                var selectedEdition = editionItems.FirstOrDefault(e => e.Value == selectedEditionId.Value.ToString());
                if (selectedEdition != null)
                {
                    selectedEdition.IsSelected = true;
                }
            }
            else
            {
                defaultItem.IsSelected = true;
            }

            return editionItems;
        }

        [AbpAuthorize(PermissionNames.Pages_Editions_Create, PermissionNames.Pages_Editions_Edit)]
        [HttpPost]
        public async Task<GetEditionForEditOutputDto> GetEditionForEdit(NullableIdDto input)
        {
            var features = this.FeatureManager.GetAll();

            EditionEditDto editionEditDto;
            List<NameValue> featureValues;

            if (input.Id.HasValue)
            {
                // Editing existing edition?
                var edition = await this.editionManager.FindByIdAsync(input.Id.Value);
                featureValues = (await this.editionManager.GetFeatureValuesAsync(input.Id.Value)).ToList();
                editionEditDto = ObjectMapper.Map<EditionEditDto>(edition);
            }
            else
            {
                editionEditDto = new EditionEditDto();
                featureValues = features.Select(f => new NameValue(f.Name, f.DefaultValue)).ToList();
            }

            return new GetEditionForEditOutputDto
            {
                Edition = editionEditDto,
                Features = ObjectMapper.Map<List<FlatFeatureDto>>(features)
                               .OrderBy(f => f.DisplayName)
                               .ToList(),
                FeatureValues =
                               featureValues.Select(fv => new NameValueDto(fv)).ToList()
            };
        }

        //public async Task<ListResultDto<EditionListDto>> GetEditions()
        //{
        //    var editions = await this.editionManager.Editions.ToListAsync();
        //    return new ListResultDto<EditionListDto>(ObjectMapper.Map<List<EditionListDto>>(editions));
        //}

        [HttpPost]
        public async Task<PagedResultDto<EditionListDto>> GetEditions(GetEditionsInputDto input)
        {
            var query = this.editionManager.Editions.WhereIf(
                !input.Search.Value.IsNullOrWhiteSpace(),
                t => t.DisplayName.Contains(input.Search.Value) || t.Name.Contains(input.Search.Value));

            var editionCount = await query.CountAsync();
            var editions = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            var editionsListDtos = ObjectMapper.Map<List<EditionListDto>>(editions);

            return new DatatablesPagedResultOutput<EditionListDto>(
                editionCount,
                editionsListDtos,
                editionsListDtos.Count)
            {
                Draw = input.Draw
            };
        }

        [HttpPost]
        public async Task<FileDto> GetEditionToExcel()
        {
            var editionList = await this.editionRepository.GetAllListAsync();
            var editionListDtos = ObjectMapper.Map<List<EditionListDto>>(editionList);
            return this.editionsListExcelExporter.ExportToFile(editionListDtos);
        }

        [AbpAuthorize(PermissionNames.Pages_Editions_Create)]
        protected virtual async Task CreateEditionAsync(CreateOrUpdateEditionDto input)
        {
            var edition = new Edition(input.Edition.DisplayName);

            await this.editionManager.CreateAsync(edition);
            await this.CurrentUnitOfWork.SaveChangesAsync(); // It's done to get Id of the edition.

            await this.SetFeatureValues(edition, input.FeatureValues);
        }

        [AbpAuthorize(PermissionNames.Pages_Editions_Edit)]
        protected virtual async Task UpdateEditionAsync(CreateOrUpdateEditionDto input)
        {
            Debug.Assert(input.Edition.Id != null, "input.Edition.Id should be set.");

            var edition = await this.editionManager.GetByIdAsync(input.Edition.Id.Value);
            edition.DisplayName = input.Edition.DisplayName;

            await this.SetFeatureValues(edition, input.FeatureValues);
        }

        private Task SetFeatureValues(Edition edition, List<NameValueDto> featureValues)
        {
            return this.editionManager.SetFeatureValuesAsync(
                edition.Id,
                featureValues.Select(fv => new NameValue(fv.Name, fv.Value)).ToArray());
        }
    }
}