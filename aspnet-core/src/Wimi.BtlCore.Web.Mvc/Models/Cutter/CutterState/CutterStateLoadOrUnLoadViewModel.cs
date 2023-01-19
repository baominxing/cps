using Abp.Application.Services.Dto;
using Castle.Components.DictionaryAdapter;
using System.Collections.Generic;
using Wimi.BtlCore.Cutter;

namespace Wimi.BtlCore.Web.Models.Cutter.CutterState
{
    public class CutterStateLoadOrUnLoadViewModel
    {
        public CutterStateLoadOrUnLoadViewModel()
        {
            this.UserList = new EditableList<NameValueDto>();
            this.MachineList = new EditableList<NameValueDto>();
        }

        public long? CurrentLoginUserId { get; set; }

        public int? CurrentMachineId { get; set; }

        public string CutterNo { get; set; }

        public int? CutterTVlaue { get; set; }

        public int? Id { get; set; }

        public List<NameValueDto> MachineList { get; set; }

        public EnumOperationType OperationType { get; set; }

        public List<NameValueDto> UserList { get; set; }
    }
}
