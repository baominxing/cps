using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Order.MachineProcesses.Dtos;
using Wimi.BtlCore.Order.Processes;
using Wimi.BtlCore.Order.Products;
using Wimi.BtlCore.Order.StandardTimes;
using Abp.Linq.Extensions;
using Wimi.BtlCore.Dto;
using AutoMapper;
using Abp.AutoMapper;
using Wimi.BtlCore.Machines.Mongo;

namespace Wimi.BtlCore.Order.MachineProcesses
{
    public class MachineProcessAppService : BtlCoreAppServiceBase, IMachineProcessAppService
    {
        private readonly IRepository<Products.Product> productRepository;

        private readonly IRepository<Process> processRepository;

        private readonly IRepository<MachineProcess> machineProcessRepository;

        private readonly IRepository<Machine> machineRepository;

        private readonly UserManager userManager;

        private readonly IMachineProcessManager machineProcessManager;

        private readonly IRepository<StandardTime> standardTimeRepository;

        private readonly MongoMachineManager mongoMachineManager;


        public MachineProcessAppService(
            IRepository<Products.Product> productRepository,
            IRepository<Process> processRepository,
            IRepository<MachineProcess> machineProcessRepository,
            UserManager userManager,
            IRepository<Machine> machineRepository,
            IMachineProcessManager machineProcessManager,
            IRepository<StandardTime> standardTimeRepository,
            MongoMachineManager mongoMachineManager)
        {
            this.productRepository = productRepository;
            this.processRepository = processRepository;
            this.userManager = userManager;
            this.machineRepository = machineRepository;
            this.machineProcessManager = machineProcessManager;
            this.standardTimeRepository = standardTimeRepository;
            this.machineProcessRepository = machineProcessRepository;
            this.mongoMachineManager = mongoMachineManager;
        }

        /// <summary>
        /// 设备换产-获取所有信息
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// </returns>
        public async Task<PagedResultDto<MachineProcessDto>> ListMachineProcess(MachineProcessPageDto filter)
        {
            var query = from mpr in this.machineProcessRepository.GetAll()
                        join mr in this.machineRepository.GetAll() on mpr.MachineId equals mr.Id into tbMachine
                        from mrData in tbMachine.DefaultIfEmpty()
                        join process in this.processRepository.GetAll() on mpr.ProcessId equals process.Id into
                            tbProcess
                        from processData in tbProcess.DefaultIfEmpty()
                        join product in this.productRepository.GetAll() on mpr.ProductId equals product.Id into tbProduct
                        from productData in tbProduct.DefaultIfEmpty()
                        orderby mpr.EndTime
                        select new { mpr, mrData, processData, productData, mpr.EndTime, mpr.CreationTime };
            var users = await this.userManager.Users.ToListAsync();
            query = query.WhereIf(filter.MachineId != 0, m => m.mpr.MachineId == filter.MachineId).WhereIf(
                filter.ProductId != 0,
                m => m.mpr.ProductId == filter.ProductId);
            var totalCount = await query.CountAsync();

            var returnValue = await query.OrderBy(filter.Sorting).PageBy(filter).ToListAsync();
            var machineProcessList = new List<MachineProcessDto>();
            foreach (var data in returnValue)
            {
                var machineProcessDto = new MachineProcessDto();
                if (data.mpr != null)
                {
                    machineProcessDto.Id = data.mpr.Id;
                    var createUser = users.FirstOrDefault(u => u.Id == data.mpr.CreatorUserId);
                    var changeUser = users.FirstOrDefault(u => u.Id == data.mpr.ChangeProductUserId);
                    if (createUser != null)
                        machineProcessDto.CreateUserName = createUser.Name;
                    if (changeUser != null)
                        machineProcessDto.ChangeProductUserName = changeUser.Name;
                    machineProcessDto.CreationTime = data.mpr.CreationTime;
                    machineProcessDto.EndTime = data.mpr.EndTime;
                }

                if (data.mrData != null)
                {
                    machineProcessDto.MachineCode = data.mrData.Code;
                    machineProcessDto.MachineName = data.mrData.Name;
                }

                if (data.processData != null)
                {
                    machineProcessDto.ProcessCode = data.processData.Code;
                    machineProcessDto.ProcessName = data.processData.Name;
                }

                if (data.productData != null)
                {
                    machineProcessDto.ProductCode = data.productData.Code;
                    machineProcessDto.ProductName = data.productData.Name;
                }

                machineProcessList.Add(machineProcessDto);
            }

            return new DatatablesPagedResultOutput<MachineProcessDto>(totalCount, machineProcessList);
        }

        /// <summary>
        /// 获取所有的产品类型的信息
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<NameValueDto>> ListProductType()
        {
            return await this.productRepository.GetAll().Select(m => new NameValueDto() { Name = m.Id.ToString(), Value = m.Name }).ToListAsync();
        }

        /// <summary>
        /// 删除设备换产数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteMachineProcess(EntityDto input)
        {
            var machineProcess = await this.machineProcessRepository.GetAsync(input.Id);
            if (machineProcess != null)
            {
                this.machineProcessManager.CheckExsist(input.Id);
                await this.machineProcessRepository.DeleteAsync(input.Id);
                this.SaveProductIdIntoMongo(machineProcess.MachineId, 0, 0);
            }
        }

        /// <summary>
        /// 获取工序类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<NameValueDto>> ListProcessType(EntityDto input)
        {
            var processIdList = await this.standardTimeRepository.GetAll().Where(s => s.ProductId == input.Id).Select(s => s.ProcessId).ToListAsync();
            return await this.processRepository.GetAll().Where(p => processIdList.Contains(p.Id)).Select(pr => new NameValueDto() { Name = pr.Id.ToString(), Value = pr.Name }).ToListAsync();
        }

        /// <summary>
        /// 检查设备是否正在生产
        /// OK：可以直接换产No：表示禁止换产 Yes:表示可以换产，弹出提示框选择是否换产
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> CheckMachineRecord(MachineProcessDto input)
        {
            var machineProcessRecord = await this.machineProcessRepository.GetAll()
                                           .FirstOrDefaultAsync(mp => mp.MachineId == input.MachineId && mp.EndTime == null);
            if (machineProcessRecord == null)
                return "OK";
            if (machineProcessRecord.ProcessId == input.ProcessId && machineProcessRecord.ProductId == input.ProductId)
            {
                return "No";
            }

            return "Yes";

        }

        /// <summary>
        /// 设备换产
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task ChangeMachineProduct(MachineProcessDto input)
        {
            var machineProcess = ObjectMapper.Map<MachineProcess>(input);
            await this.machineProcessManager.ChangeMachineProduct(machineProcess);
            this.SaveProductIdIntoMongo(
               machineProcess.MachineId,
               machineProcess.ProductId,
               machineProcess.ProcessId);
        }

        private void SaveProductIdIntoMongo(int machineId, int productId, int processId)
        {
            mongoMachineManager.SaveProductIdIntoMongo(machineId, productId, processId);
        }

    }
}
