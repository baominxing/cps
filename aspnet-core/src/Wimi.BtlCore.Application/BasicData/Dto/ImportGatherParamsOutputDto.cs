using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using Wimi.BtlCore.Dmps;

namespace Wimi.BtlCore.BasicData.Dto
{
    [AutoMap(typeof(MachineVariable))]
    public class ImportGatherParamsOutputDto
    {
        [Display(Name = "序号")]
        public int Seq { get; set; }

        [Display(Name = "设备Id")]
        public string MachineId { get; set; }

        [Display(Name = "变量名称")]
        public string Code { get; set; }

        [Display(Name = "变量描述")]
        public string Description { get; set; }

        [Display(Name = "数据地址")]
        public string DeviceAddress { get; set; }

        [Display(Name = "数据类别")]
        public string DataType { get; set; }

        [Display(Name = "数据类别描述")]
        public string DataTypeString { get; set; }

        [Display(Name = "存取权限")]
        public int Access { get; set; }

        [Display(Name = "存取权限描述")]
        public string AccessString { get; set; }

        [Display(Name = "数据倍率")]
        public string ValueFactor { get; set; }

        [Display(Name = "初始值")]
        public string DefaultValue { get; set; }

        [Display(Name = "导入结果")]
        public string ErrorMessage { get; set; }
    }
}
