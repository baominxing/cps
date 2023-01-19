using Abp.Application.Services;
using System;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives.Dtos;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Archives
{
    public interface IArchiveEntryAppService : IApplicationService
    {
        Task<DatatablesPagedResultOutput<ArchiveEntryDto>> ListArchiveEntry(ArchiveEntryInputDto input);

        Task Create(ArchiveEntryInputDto input);

        Task Update(ArchiveEntryInputDto input);

        Task Delete(ArchiveEntryInputDto input);

        Task<ArchiveEntryDto> Get(ArchiveEntryInputDto input);

        Task ArchiveFromStartTimeToEndTime(ArchiveEntryTestDto input);

        Task Archive(string archiveTable, DateTime archiveDateTime);

        Task ArchiveForTraceCatalogs(DateTime archiveDateTime);
    }
}
