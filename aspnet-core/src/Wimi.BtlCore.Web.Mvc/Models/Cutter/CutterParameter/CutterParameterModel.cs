using Abp.AutoMapper;
using Wimi.BtlCore.Cutter.Dto;

namespace Wimi.BtlCore.Web.Models.Cutter.CutterParameter
{

    [AutoMap(typeof(CutterParameterDto))]
    public class CutterParameterModel : CutterParameterDto
    {
        public CutterParameterModel()
        {
            this.IsEditMode = false;
        }

        public bool IsEditMode { get; set; }
    }
}
