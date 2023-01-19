using System.Collections.Generic;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class ImportDataDto
    {
        public ImportDataDto()
        {
            this.ImportData = new List<AlarmInfoDto>();
        }

        public int MachineTypeId { get; set; }

        public int MachineId { get; set; }

        public IEnumerable<AlarmInfoDto> ImportData { get; set; }
    }
}
