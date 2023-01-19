namespace Wimi.BtlCore.BasicData.Exporting
{
    using System.Collections.Generic;
    using Wimi.BtlCore.BasicData.Dto;
    using Wimi.BtlCore.Dto;

    public interface IExporter
    {
        FileDto ExportToFile(List<MachineSettingListDto> machineSettingListDtos);

        FileDto ExportMachineToFile(List<MachineDto> input);

        FileDto ExportMachineToXML(List<ExportMachineDto> machineList, List<ExportMachineDto> groupList);
    }
}