using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Authorization.DmpMachineSetting.Dto
{
    public class RemoveMachineFromDmpInputDto
    {
        public int DmpId { get; set; }

        public int MachineId { get; set; }
    }
}
