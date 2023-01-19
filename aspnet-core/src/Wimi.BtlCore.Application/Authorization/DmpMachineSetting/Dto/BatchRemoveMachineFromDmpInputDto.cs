using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Authorization.DmpMachineSetting.Dto
{
    public class BatchRemoveMachineFromDmpInputDto
    {
        public BatchRemoveMachineFromDmpInputDto()
        {
            MachineIds = new List<int>();
        }

        public int DmpId { get; set; }

        public List<int> MachineIds { get; set; }
    }
}
