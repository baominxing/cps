using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Castle.Components.DictionaryAdapter;

namespace Wimi.BtlCore.Order.LoginReports.Dtos
{
    public class WorkOrderLoginDto : EntityDto
    {
        public WorkOrderLoginDto()
        {
            this.MachineIdList = new EditableList<int>();
        }

        public List<int> MachineIdList { get; set; }
    }
}
