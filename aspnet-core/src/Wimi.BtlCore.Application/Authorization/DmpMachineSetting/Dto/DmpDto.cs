using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities.Auditing;

namespace Wimi.BtlCore.Authorization.DmpMachineSetting.Dto
{
    public class DmpDto: CreationAuditedEntity
    {
        public string Code { get; set; }

        public string DisplayName { get; set; }

        public int MemberCount { get; set; }
    }
}
