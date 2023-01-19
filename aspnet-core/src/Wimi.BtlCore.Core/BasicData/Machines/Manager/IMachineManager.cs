using Abp;

namespace Wimi.BtlCore.BasicData.Machines.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services.Dto;
    using Abp.Domain.Services;

    public interface IMachineManager : IDomainService
    {
        Task<Machine> GetMachineByIdAsync(int machineId);

        string GetMachineImagePath(EntityDto<Guid?> input);

        IEnumerable<Machine> ListMachinesInDeviceGroup(string deviceGroup);

        Task<IEnumerable<NameValueDto<int>>> ListMachines();

        Task<IEnumerable<NameValueDto<int>>> ListDefaultSearchMachines();

        Task<IEnumerable<NameValueDto<int>>> ListOrderedMachines();

        Task DeleteMachine(Machine machine);
    }
}