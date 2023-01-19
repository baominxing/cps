namespace Wimi.BtlCore.BasicData.Machines.Manager
{
    using Abp.Application.Services.Dto;
    using Abp.Domain.Services;
    using System.Collections.Generic;

    public interface IMachineGatherParamManager : IDomainService
    {
        /// <summary>
        /// 检查模板给定的配置是否可以使用到配置信息上
        /// </summary>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        /// 将不符合的模板项返回
        /// </returns>
        IEnumerable<string> CheckParamItemDataType(Dictionary<string, NameValueDto<EnumParamsDisplayStyle>> input);

    }
}
