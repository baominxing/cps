using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Abp;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Order.Crafts
{
    /// <summary>
    /// 工艺
    /// </summary>
    [Table("Crafts")]
    public class Craft : FullAuditedEntity
    {
        public Craft()
        {
            this.CraftProcesses = new List<CraftProcess>();
        }

        /// <summary>
        /// 工艺代码
        /// </summary>
        [Comment("工艺代码")]
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 组合工序
        /// </summary>
        public virtual ICollection<CraftProcess> CraftProcesses { get; set; }

        /// <summary>
        /// 工艺备注
        /// </summary>
        [Comment("工艺备注")]
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// 工艺名称
        /// </summary>
        [Comment("工艺名称")]
        [MaxLength(50)]
        public string Name { get; set; }

        public void AddCraftProcessList(IEnumerable<CraftProcess> process)
        {
            var count = this.CraftProcesses.Count;
            foreach (var craftProcess in process)
            {
                craftProcess.ProcessOrder = ++count;
                this.CraftProcesses.Add(craftProcess);
            }
        }

        public void ResetProcessOrder()
        {
            var processOrder = 1;
            var count = this.CraftProcesses.Count;

            foreach (var craftProcess in this.CraftProcesses.OrderBy(c => c.ProcessOrder))
            {
                craftProcess.ProcessOrder = processOrder;
                craftProcess.IsLastProcess = processOrder == count;
                processOrder++;
            }
        }

        public void ResetProcessOrderAfterDelete(int deletedProcessId)
        {
            var processOrder = 1;
            var resumeCraftProcesses = this.CraftProcesses.Where(s => s.Id != deletedProcessId).ToList();
            var count = resumeCraftProcesses.Count;

            foreach (var craftProcess in resumeCraftProcesses.OrderBy(c => c.ProcessOrder))
            {
                craftProcess.ProcessOrder = processOrder;
                craftProcess.IsLastProcess = processOrder == count;
                processOrder++;
            }
        }

        public void UpdateProcessListOrder(IDictionary<int, int> processIdIncrementDict)
        {
            foreach (var key in processIdIncrementDict.Keys)
            {
                var process = this.CraftProcesses.FirstOrDefault(c => c.Id == key);
                if (process != null)
                {
                    process.ProcessOrder += processIdIncrementDict[key];
                }
            }
        }
    }
}
