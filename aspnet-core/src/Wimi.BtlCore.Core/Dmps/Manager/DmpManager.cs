namespace Wimi.BtlCore.Dmps.Manager
{
    using System;
    using System.Linq;
    using System.Net;

    using Abp.Domain.Repositories;
    using Abp.Domain.Services;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;

    public class DmpManager : BtlCoreDomainServiceBase
    {
        private readonly IRepository<Dmp> dmpRepository;
        private readonly IRepository<MachineVariable, Guid> machineVariableRepository; 
        private readonly IRepository<MachineDriver> machineDriveRepository;
        private readonly IRepository<DmpMachine> dmpMachineRepository;
        private readonly IRepository<DriverConfig> driverConfigRepository;
        private readonly IRepository<MachineGatherParam, long> machineGatherParamRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private const string RootAddress = "Webapi/Configure/DeviceManagement/RemoveDevice";

        public DmpManager(IRepository<Dmp> dmpRepository,
            IRepository<DmpMachine> dmpMachineRepository, 
            IRepository<MachineDriver> machineDriveRepository, 
            IRepository<MachineVariable, Guid> machineVariableRepository,
            IRepository<DriverConfig> driverConfigRepository, 
            IRepository<MachineGatherParam, long> machineGatherParamRepository,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository)
        {
            this.dmpRepository = dmpRepository;
            this.dmpMachineRepository = dmpMachineRepository;
            this.machineDriveRepository = machineDriveRepository;
            this.machineVariableRepository = machineVariableRepository;
            this.driverConfigRepository = driverConfigRepository;
            this.machineGatherParamRepository = machineGatherParamRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
        }

        public void Delete(Guid dmpMachineid, int machineId)
        {
            this.dmpMachineRepository.Delete(d=>d.MachineId == machineId);
            this.machineDriveRepository.Delete(m=>m.DmpMachineId == dmpMachineid);
            this.machineVariableRepository.Delete(m=>m.DmpMachineId == dmpMachineid);
            this.driverConfigRepository.Delete(d=>d.DmpMachineId == dmpMachineid);
            this.machineGatherParamRepository.Delete(mg => mg.MachineId == machineId);
            this.machineDeviceGroupRepository.Delete(md => md.MachineId == machineId);
            /*
            var dmps = this.dmpRepository.GetAll().ToList();
            dmps.ForEach(
                d =>
                    {
                        try
                        {
                            var host = $"http://{d.IpAdress}:{d.WebPort}/{RootAddress}?id={dmpMachineid}";
                            this.Logger.Info("DmpManager 调用删除接口:" + host);
                            var client = new WebClient();
                            client.DownloadString(host);
                        }
                        catch (Exception e)
                        {
                            this.Logger.Info("DmpManager 删除接口失败:" + e);
                        }
                    });
                    */
        }
    }
}