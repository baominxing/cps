namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    using Castle.Components.DictionaryAdapter;

    using Wimi.BtlCore.BasicData.Dto;

    public class GetMachineAlarmOutputDto : MachineStatusListDto
    {
        public GetMachineAlarmOutputDto()
        {
            this.GroupId = new EditableList<int>();
        }

        /// <summary>
        /// 设备报警信息
        /// </summary>
        public MongoAlarmInfo MongoAlarmInfo { get; set; }
    }
}