using System;

using Abp.Application.Editions;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;

namespace Wimi.BtlCore.Editions.Dto
{
    [AutoMapFrom(typeof(Edition))]
    public class EditionListDto : EntityDto, IHasCreationTime
    {
        public DateTime CreationTime { get; set; }

        public string DisplayName { get; set; }

        public string Name { get; set; }
    }
}