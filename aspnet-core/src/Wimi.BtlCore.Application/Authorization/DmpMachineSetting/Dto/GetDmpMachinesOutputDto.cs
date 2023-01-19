using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.Authorization.DmpMachineSetting.Dto
{
    public class GetDmpMachinesOutputDto : EntityDto
    {
        public DateTime AddedTime { get; set; }

        public string Code { get; set; }

        public string Desc { get; set; }

        public Guid? ImageId { get; set; }

        public string Name { get; set; }

        public int SortSeq { get; set; }
    }
}
