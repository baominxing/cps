using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.MachineTypes;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Maintain.Dto;

namespace Wimi.BtlCore.Maintain
{
    public class RepairRequestAppService : BtlCoreAppServiceBase, IRepairRequestAppService
    {
        private IRepository<User,long> userRepository;
        private IRepository<Machine> machineRepository;
        private IRepository<MachineType> machineTypeRepository;
        private IRepository<RepairRequest> repairRequestRepository;
        public RepairRequestAppService(
            IRepository<User, long> userRepository,
            IRepository<Machine> machineRepository,
            IRepository <MachineType > machineTypeRepository,
            IRepository<RepairRequest> repairRequestRepository
            )
        {
            this.userRepository = userRepository;
            this.machineRepository = machineRepository;
            this.machineTypeRepository = machineTypeRepository;
            this.repairRequestRepository = repairRequestRepository;
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public async Task<IList<NameValueDto>> ListUser()
        {
            var users = await userRepository.GetAll().Select(u => new NameValueDto() { Name = u.Id.ToString(), Value = u.Name }).ToListAsync();
            return users;
        }
        /// <summary>
        /// 获取指定ID的设备类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MaintianMachineTypeDto> GetMachineType(EntityDto input)
        {
            var type = await (from machine in machineRepository.GetAll()
                               join machineType in machineTypeRepository.GetAll()
                               on machine.MachineTypeId equals machineType.Id
                               where machine.Id == input.Id
                               select machineType.Name
                       ).FirstOrDefaultAsync();

            return new MaintianMachineTypeDto() {  MachineType=type};         
        }
        /// <summary>
        /// 创建或者更新维修申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdate(MaintainRequestDto input)
        {
            //编辑
            if (input.Id!=0)
            {
                if (input.MachineId != 0 && input.RequestUserId != 0)
                {
                    var data = repairRequestRepository.Get(input.Id);
                    data.IsShutdown = input.IsShutdown;
                    data.RequestUserId = input.RequestUserId;
                    data.RequestMemo = input.RequestMemo;
                    data.RequestDate = input.RequestDate;
                    await repairRequestRepository.UpdateAsync(data);
                }
            }
            else
            {
                //创建
                if (input.MachineId != 0 && input.RequestUserId != 0)
                { 
                    var model = new RepairRequest();
                    ObjectMapper.Map(input, model);
                    model.Code= "RP" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    await repairRequestRepository.InsertAsync(model);
                }
              

            }
        }
        /// <summary>
        /// 根据Id获取一条维修申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public MaintainRequestDto GetMaintainRequest(EntityDto input)
        {
            var model =repairRequestRepository.Get(input.Id);
            var machineType = (from m in machineRepository.GetAll()
                               join mt in machineTypeRepository.GetAll()
                               on m.MachineTypeId equals mt.Id
                               where m.Id == model.MachineId
                               select mt.Name).FirstOrDefault();;
            var result= new MaintainRequestDto();
            ObjectMapper.Map(model,result);

            result.MachineType = machineType;
            return result;
        }
        /// <summary>
        /// 根据id查看维修申请的详细信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public MaintainRequestDto GetMaintainRequestOnLook(EntityDto input)
        {
            var model = repairRequestRepository.Get(input.Id);
            var machine = (from m in machineRepository.GetAll()
                           join mt in machineTypeRepository.GetAll()
                           on m.MachineTypeId equals mt.Id
                           where m.Id == model.MachineId
                           select new { MachineType = mt.Name, MachineCode = m.Code }).FirstOrDefault();
            var user = userRepository.Get(Convert.ToInt32(model.RequestUserId));
            var result = new MaintainRequestDto();
            ObjectMapper.Map(model, result);
            result.MachineType = machine.MachineType;
            result.MachineCode = machine.MachineCode;
            result.RequestUserName = user.Name;
            return result;
        }
        /// <summary>
        /// 获取维修申请列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<MaintainRequestDto>> ListRequest(MainRequestFilterDto input)
        {
            var query = from rr in repairRequestRepository.GetAll()
                join m in machineRepository.GetAll()
                    on rr.MachineId equals m.Id
                join u in userRepository.GetAll()
                    on rr.RequestUserId equals u.Id
                join mt in machineTypeRepository.GetAll()
                    on m.MachineTypeId equals mt.Id
                select new MaintainRequestDto()
                {
                    Code = rr.Code,
                    Status = rr.Status,
                    MachineCode = m.Code,
                    MachineName = m.Name,
                    MachineId = m.Id,
                    MachineType = mt.Name,
                    IsShutdown = rr.IsShutdown,
                    RequestUserName = u.Name,
                    RequestDate = rr.RequestDate,
                    Id = rr.Id,
                    CreationTime = rr.CreationTime
                };
            var endTime = input.EndTime.AddDays(1);
            var machineId = Convert.ToInt32(input.MachineId);
            //全
            if (input.Status == 3)
            {
               query=  query.Where(u => u.RequestDate >= input.StartTime && u.RequestDate <endTime);
            }
            else
            {
                query =query.Where(u => u.RequestDate >= input.StartTime && u.RequestDate <endTime && input.Status == (int)u.Status);
            }
            //工单号为null或者设备编号为null表示全部展示
            if (!string.IsNullOrEmpty(input.Code))
            {
                query = query.Where(l => l.Code.ToLower().Contains(input.Code.ToLower().Trim()));
            }

            if (machineId!=0)
            {
                query = query.Where(l => l.MachineId == machineId);
            }
            var totalCount = await query.CountAsync();
            var returnValue = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();
            var list = new List<MaintainRequestDto>();
            ObjectMapper.Map(returnValue,list);
            return new DatatablesPagedResultOutput<MaintainRequestDto>(totalCount, list)
            {
                Draw = input.Draw
            };
        }
        /// <summary>
        /// 根据Id删除维修申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task DeleteRequest(EntityDto input)
        {
           await repairRequestRepository.DeleteAsync(input.Id);
        }
        ////////////////////////////////////////////////////维修工单////////////////////////////////////
        /// <summary>
        /// 获取维修工单列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public MaintainRepairDto GetMaintainRepairList(EntityDto input)
        {
            var model = repairRequestRepository.Get(input.Id);
            var machine = (from m in machineRepository.GetAll()
                           join mt in machineTypeRepository.GetAll()
                           on m.MachineTypeId equals mt.Id
                           where m.Id == model.MachineId
                           select new { MachineType = mt.Name, MachineCode = m.Code }).FirstOrDefault();
            var user = userRepository.Get(Convert.ToInt32(model.RequestUserId));
            var result = new MaintainRepairDto();
            ObjectMapper.Map(model,result);
            result.MachineType = machine.MachineType;
            result.MachineCode = machine.MachineCode;
            result.RequestUserName = user.Name;
            return result;
        }
        /// <summary>
        /// 查看或者维修 工单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task LookOrRepair(RepairInputDto input)
        {
            if (input.Id != 0)
            {
                var model = await repairRequestRepository.GetAsync(input.Id);
                if (input.IsEditMode)
                {

                    model.StartTime = input.StartTime;
                    model.RepairMemo = input.RepairMemo;
                    //待维修
                    if (input.Status == EnumRepairRequestStatus.Undo)
                    {
                        model.Status = EnumRepairRequestStatus.Overtime;
                    }
                    else
                    {
                        //维修中
                        model.Cost = input.Cost;
                        model.EndTime = input.EndTime;
                        model.Status = EnumRepairRequestStatus.Done;
                    }
                }
               await repairRequestRepository.UpdateAsync(model);
            }
        }
    }
}
