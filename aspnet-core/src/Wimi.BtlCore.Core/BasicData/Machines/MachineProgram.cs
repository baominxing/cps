using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Wimi.BtlCore.BasicData.Machines
{
    [Table("MachinePrograms")]
    public class MachineProgram : FullAuditedEntity
    {
        [Comment("工序编号")]
        public string ProcessCode { get; set; }

        [Comment("程序号")]
        public string ProgramNo { get; set; }

        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Comment("设备编号")]
        public string MachineCode { get; set; }
    }
}
