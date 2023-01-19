namespace Wimi.BtlCore.Cutter.Dto
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Abp.Domain.Entities.Auditing;

    public class CutterLoadAndUnloadRecordDto : CreationAuditedEntity
    {
        /// <summary>
        /// 寿命计数方式（0：按次数，1：按时间）
        /// </summary>
        public int CountingMethod { get; set; }

        public string CountingMethodName { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CreatorUserName { get; set; }

        public int CutterModelId { get; set; }

        /// <summary>
        /// 刀具型号名称
        /// </summary>
        public string CutterModelName { get; set; }

        /// <summary>
        /// 刀具编号
        /// </summary>
        public string CutterNo { get; set; }

        /// <summary>
        /// 刀位（刀具T值）
        /// </summary>
        public int? CutterTValue { get; set; }

        /// <summary>
        /// 刀具类型名称
        /// </summary>
        public string CutterTypeName { get; set; }

        public int MachineId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 0：卸刀，1：装刀
        /// </summary>
        public int OperationType { get; set; }

        public string OperationTypeName { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? OperatorTime { get; set; }

        /// <summary>
        /// 原始寿命
        /// </summary>
        public int OriginalLife { get; set; }

        /// <summary>
        /// 刀具参数
        /// </summary>
        [StringLength(50)]
        public string Parameter1 { get; set; }

        [StringLength(50)]
        public string Parameter10 { get; set; }

        [StringLength(50)]
        public string Parameter2 { get; set; }

        [StringLength(50)]
        public string Parameter3 { get; set; }

        [StringLength(50)]
        public string Parameter4 { get; set; }

        [StringLength(50)]
        public string Parameter5 { get; set; }

        [StringLength(50)]
        public string Parameter6 { get; set; }

        [StringLength(50)]
        public string Parameter7 { get; set; }

        [StringLength(50)]
        public string Parameter8 { get; set; }

        [StringLength(50)]
        public string Parameter9 { get; set; }

        /// <summary>
        /// 剩余寿命
        /// </summary>
        public int RestLife { get; set; }

        /// <summary>
        /// 已用寿命
        /// </summary>
        public int UsedLife { get; set; }
    }
}