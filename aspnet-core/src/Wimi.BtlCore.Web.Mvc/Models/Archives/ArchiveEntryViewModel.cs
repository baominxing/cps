using Wimi.BtlCore.Archives.Dtos;

namespace Wimi.BtlCore.Web.Models.Archives
{
    public class ArchiveEntryViewModel
    {
        public ArchiveEntryDto Dto { get; set; } = new ArchiveEntryDto();

        public bool IsEditMode { get; set; }
    }

}
