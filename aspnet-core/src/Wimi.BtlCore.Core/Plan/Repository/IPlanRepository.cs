using Abp.Application.Services.Dto;
using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Plan.Repository.Dto;
using Wimi.BtlCore.Trace;

namespace Wimi.BtlCore.Plan.Repository
{
    public interface IPlanRepository : ITransientDependency
    {
        IEnumerable<PlanResponse> ListStatisticalWayYieldByCapacity(ProcessPlanInput input);

        IEnumerable<PlanResponse> ListStatisticalWayYieldByTrace(ProcessPlanInput input);

        IEnumerable<PlanResponse> ListProcessPlans(ProcessPlanInput input);

        IEnumerable<SummaryDateDto> ListSummaryDate(ProcessPlanInput input);

        void HandlerLinePlan(TraceCatalog traceCatalog, bool qualified);

        Task<List<NameValueDto>> GetShiftSolutionName(List<int>machineIds ,DateTime? planStartTime);

        void DeletePlanTarget(int planId);
    }
}
