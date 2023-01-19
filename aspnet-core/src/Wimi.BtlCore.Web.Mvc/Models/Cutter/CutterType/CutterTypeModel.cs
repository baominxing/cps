namespace Wimi.BtlCore.Web.Models.Cutter.CutterType
{
    using Abp.AutoMapper;
    using Wimi.BtlCore.Cutter.Dto;

    [AutoMap(typeof(CutterTypeDto))]
    public class CutterTypeModel : CutterTypeDto
    {
        public CutterTypeModel()
        {
            this.IsEditMode = false;
            this.IsCreateFromContextMenu = false;
        }

        public bool IsCreateFromContextMenu { get; set; }

        public bool IsEditMode { get; set; }
    }
}
