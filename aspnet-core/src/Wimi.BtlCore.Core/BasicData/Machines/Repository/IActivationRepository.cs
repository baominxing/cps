using Abp.Dependency;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Wimi.BtlCore.EfficiencyTrendas.Dtos;

namespace Wimi.BtlCore.BasicData.Machines.Repository
{
    public interface IActivationRepository : ITransientDependency
    {
        Task<List<ExpandoObject>> GetMachineActivationOriginalData(EfficiencyTrendsInputDto input);
    }
}
