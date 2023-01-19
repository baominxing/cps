namespace Wimi.BtlCore.BasicData.Dto
{
    using System.Collections.Generic;
    using Wimi.BtlCore.CommonEnums;

    public class ImportDataOutputDto
    {
        public ImportDataOutputDto()
        {
            this.UsersList = new List<ImportUsersOutputDto>();
            this.MachinesList = new List<ImportMachinesOutputDto>();
        }

        public List<ImportMachinesOutputDto> MachinesList { get; set; }

        public EnumImportTypes Type { get; set; }

        public List<ImportUsersOutputDto> UsersList { get; set; }

        public List<ImportGatherParamsOutputDto> GatherParamsList { get; set; }
    }
}