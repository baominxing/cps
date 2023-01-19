using Abp.AutoMapper;
using Abp.ObjectMapping;
using Microsoft.AspNetCore.Mvc.Rendering;
using Wimi.BtlCore.BasicData.Dto;
using Wimi.BtlCore.Web.Models.Common;

namespace Wimi.BtlCore.Web.Models.BasicData.GatherParamsSetting
{
    [AutoMapFrom(typeof(MachineGatherParamDto))]
    public class GatherParamsSettingEditViewModel : MachineGatherParamDto, IYesNoSelectListViewModel
    {
        public GatherParamsSettingEditViewModel(MachineGatherParamDto outParamDto)
        {
            this.Id = outParamDto.Id;
            this.Code = outParamDto.Code;

            this.DataType = outParamDto.DataType;
            this.DisplayStyle = outParamDto.DisplayStyle;
            this.Hexcode = outParamDto.Hexcode;
            this.IsShowForStatus = outParamDto.IsShowForStatus;
            this.IsShowForParam = outParamDto.IsShowForParam;
            this.IsShowForVisual = outParamDto.IsShowForVisual;
            this.MachineCode = outParamDto.MachineCode;
            this.MachineId = outParamDto.MachineId;
            this.Max = outParamDto.Max;
            this.Min = outParamDto.Min;
            this.Name = outParamDto.Name;
            this.SortSeq = outParamDto.SortSeq;
            this.TenantId = outParamDto.TenantId;
            this.Unit = outParamDto.Unit;
        }

        public SelectList YesNoModel { get; set; }

        public string[] FixedDataValue { get; set; }
    }
}
