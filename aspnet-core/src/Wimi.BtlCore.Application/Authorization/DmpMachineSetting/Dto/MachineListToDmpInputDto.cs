using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Authorization.DmpMachineSetting.Dto
{
    public class MachineListToDmpInputDto
    {
        public int DmpId { get; set; }

        public List<int> MachineIdList { get; set; }
    }
}
