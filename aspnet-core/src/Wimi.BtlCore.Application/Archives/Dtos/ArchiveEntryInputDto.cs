using Abp.AutoMapper;
using Abp.Runtime.Validation;
using System;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Archives.Dtos
{
    [AutoMap(typeof(ArchiveEntry))]
    public class ArchiveEntryInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int Id { get; set; }

        public string TargetTable { get; set; }

        public string ArchivedTable { get; set; }

        public string ArchiveColumn { get; set; }

        public string ArchiveValue { get; set; }

        public long ArchiveCount { get; set; }

        public long ArchiveTotalCount { get; set; }

        public string ArchivedMessage { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public long? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public long? CreatorUserId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Id";
            }
        }
    }
}
