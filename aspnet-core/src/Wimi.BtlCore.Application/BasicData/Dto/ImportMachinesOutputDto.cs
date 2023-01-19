namespace Wimi.BtlCore.BasicData.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class ImportMachinesOutputDto
    {
        [Display(Name = "序号")]
        public int Seq { get; set; }

        [Display(Name = "设备编号")]
        public string Code { get; set; }

        [Display(Name = "设备名称")]
        public string Name { get; set; }

        [Display(Name = "设备类型")]
        public string Type { get; set; }

        [Display(Name = "设备描述")]
        public string Description { get; set; }

        [Display(Name = "是否启用")]
        public bool IsActive { get; set; }

        [Display(Name = "导入结果")]
        public string ErrorMessage { get; set; }
    }
}