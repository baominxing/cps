namespace Wimi.BtlCore.OEE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Wimi.BtlCore.CommonEnums;

    public class OeeAnalysisManager: BtlCoreDomainServiceBase,IOeeAnalysisManager
    {
        private readonly IOeeRepository oeeRepository;

        public OeeAnalysisManager(IOeeRepository oppRepository)
        {
            this.oeeRepository = oppRepository;
        }

        public async Task<IEnumerable<int>> GetFilterIds(EnumQueryMethod type, IEnumerable<int> machineIdList, bool batch = true)
        {
            if (!batch) return machineIdList;

            if (type == EnumQueryMethod.ByMachine)
            {
                return machineIdList;
            }

            return await this.oeeRepository.ListMachineIdInGroup(machineIdList);
        }

        public async Task<IEnumerable<string>> ListRevisedDate(EnumStatisticalWays type, DateTime startTime, DateTime endTime)
        {
            return await this.oeeRepository.ListRevisedDate(type, startTime, endTime);
        }

        public async Task<OeeAnalysis> FormartInputDto(OeeAnalysis input)
        {
            var dateList = (await this.ListRevisedDate(input.StatisticalWays, input.StartTime, input.EndTime)).ToList();
            input.ShiftDayRanges = await this.oeeRepository.ListSummaryDate(input);
            if (!dateList.Any()) return input;
            input.StartTime = Convert.ToDateTime(dateList.First());
            input.EndTime = Convert.ToDateTime(dateList.Last());
            return input;
        }
    }
}