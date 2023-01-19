using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Wimi.BtlCore.Archives.Dtos
{
    [AutoMap(typeof(ArchiveEntry))]
    public class ArchiveEntryDto : FullAuditedEntityDto<int>
    {
        public string TargetTable { get; set; }

        public string ArchivedTable { get; set; }

        public string ArchiveColumn { get; set; }

        public string ArchiveValue { get; set; }

        public long ArchiveCount { get; set; }

        public long ArchiveTotalCount { get; set; }

        public string ArchivedMessage { get; set; }

    }
}
