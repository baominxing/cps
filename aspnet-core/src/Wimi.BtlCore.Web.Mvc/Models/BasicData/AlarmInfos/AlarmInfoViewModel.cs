using Wimi.BtlCore.StatisticAnalysis.Alarms.Dto;

namespace Wimi.BtlCore.Web.Models.BasicData.AlarmInfos
{
    public class AlarmInfoViewModel
    {
        public AlarmInfoViewModel()
        {
            this.IsEditModal = false;
            this.AlarmInfo = new CreateOrEditAlarmInfoDto();
        }

        public bool IsEditModal { get; set; }

        public CreateOrEditAlarmInfoDto AlarmInfo { get; set; }
    }
}
