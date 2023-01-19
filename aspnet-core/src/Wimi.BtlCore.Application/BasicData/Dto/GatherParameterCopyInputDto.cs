using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Wimi.BtlCore.BasicData.Dto
{
    public class GatherParameterCopyInputDto:EntityDto<long>
    {
        /// <summary>
        /// 需要执行参数赋值的设备
        /// </summary>
       public IList<long> MachineIds { get; set; }
      
    }
}