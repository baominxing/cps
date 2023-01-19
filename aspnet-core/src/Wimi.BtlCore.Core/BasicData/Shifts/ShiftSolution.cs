using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.BasicData.Shifts
{
    /// <summary>
    /// 班次方案
    /// </summary>
    [Table("ShiftSolutions")]
    public class ShiftSolution : FullAuditedEntity
    {
        [Comment("班次方案名称")]
        public string Name { get; set; }
    }
}
