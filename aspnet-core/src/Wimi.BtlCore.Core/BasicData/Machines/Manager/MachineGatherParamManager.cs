using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Wimi.BtlCore.BasicData.Machines.Manager
{
    public class MachineGatherParamManager : BtlCoreDomainServiceBase, IMachineGatherParamManager
    {
        private readonly IRepository<MachineGatherParam, long> machineGatherParamRepository;

        public MachineGatherParamManager(IRepository<MachineGatherParam, long> machineGatherParamRepository)
        {
            this.machineGatherParamRepository = machineGatherParamRepository;
        }

        /// <summary>
        /// The check param item data type.
        /// </summary>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public IEnumerable<string> CheckParamItemDataType(
            Dictionary<string, NameValueDto<EnumParamsDisplayStyle>> input)
        {
            var query = from item in input
                        where
                            !item.Value.Name.ToLower().Equals("number")
                            && (item.Value.Value == EnumParamsDisplayStyle.GaugePanel
                                || item.Value.Value == EnumParamsDisplayStyle.LineChart)
                        select item.Key;

            return query;
        }
    }
}
