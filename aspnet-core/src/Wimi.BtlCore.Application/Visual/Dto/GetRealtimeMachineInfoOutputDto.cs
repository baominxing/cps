namespace Wimi.BtlCore.Visual.Dto
{
    using System.Collections.Generic;

    using Castle.Components.DictionaryAdapter;
    using DeviceMonitorFramework;

    public class GetRealtimeMachineInfoOutputDto
    {
        public GetRealtimeMachineInfoOutputDto()
        {
            this.DataItemInfo = new EditableList<DataItemInfo>();
        }

        public string Code { get; set; }

        public List<DataItemInfo> DataItemInfo { get; set; }

        public string Name { get; set; }
    }
}