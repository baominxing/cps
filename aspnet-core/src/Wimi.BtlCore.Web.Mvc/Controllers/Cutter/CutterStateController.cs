using Abp;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Cutter;
using Wimi.BtlCore.Web.Models.Cutter.CutterState;

namespace Wimi.BtlCore.Web.Controllers.Cutter
{
    public class CutterStateController : BtlCoreControllerBase
    {
        private readonly ICutterAppService cutterAppService;

        private readonly IRepository<CutterParameter> cutterParameterRepository;

        private readonly IRepository<CutterStates> cutterStatesRepository;

        private readonly IRepository<Machine> machineRepository;

        private readonly IRepository<User, long> userRepository;

        public CutterStateController(
            ICutterAppService cutterAppService,
            IRepository<CutterParameter> cutterParameterRepository,
            IRepository<CutterStates> cutterStatesRepository,
            IRepository<User, long> userRepository,
            IRepository<Machine> machineRepository)
        {
            this.cutterAppService = cutterAppService;
            this.cutterParameterRepository = cutterParameterRepository;
            this.cutterStatesRepository = cutterStatesRepository;
            this.userRepository = userRepository;
            this.machineRepository = machineRepository;
        }

        /// <summary>
        /// 新增或编辑刀具Modal
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// </returns>
        public async Task<PartialViewResult> CreateOrUpdateModal(int? id)
        {
            var mapQuery =
                (await this.cutterParameterRepository.GetAllListAsync()).Select(p => new NameValueDto(p.Code, p.Name));
            var model = new CutterStateCreateOrUpdateViewModel()
            {
                IsEditModal = id.HasValue,
                ParameterMap = mapQuery.ToList()
            };
            Console.WriteLine();

            if (!id.HasValue)
            {
                return this.PartialView("~/Views/Cutter/CutterState/_CreateOrUpdateModal.cshtml", model);
            }

            model.CutterState = await this.cutterStatesRepository.GetAsync(id.Value);
            model.Id = model.CutterState.Id;
            model.CountingMethod = model.CutterState.CountingMethod;
            model.CutterLifeStatus = model.CutterState.CutterLifeStatus;
            model.CreationTime = model.CutterState.CreationTime;
            model.CreatorUserId = model.CutterState.CreatorUserId;
            model.CutterUsedStatus = model.CutterState.CutterUsedStatus;

            return this.PartialView("~/Views/Cutter/CutterState/_CreateOrUpdateModal.cshtml", model);
        }

        public async Task<ActionResult> Index()
        {
            var modal = new CutterStateViewModel()
            {
                CutterTypes = (await this.cutterAppService.FindCutterType()).ToList()
            };
            modal.UsedStates.Add(new NameValue<int>(this.L("All"), 0));

            foreach (int stateValue in Enum.GetValues(typeof(EnumCutterUsedStates)))
            {
                modal.UsedStates.Add(new NameValue<int>()
                {
                    Name = L(Enum.GetName(typeof(EnumCutterUsedStates), stateValue)),
                    Value = stateValue
                });

            }

            // modal.UsedStates.AddRange(typeof(EnumCutterUsedStates).ToNameValueList<int>());
            modal.LifeStates.Add(new NameValue<int>(this.L("All"), 0));
            // modal.LifeStates.AddRange(typeof(EnumCutterLifeStates).ToNameValueList<int>());
            foreach (int stateValue in Enum.GetValues(typeof(EnumCutterLifeStates)))
            {
                modal.LifeStates.Add(new NameValue<int>()
                {
                    Name = L(Enum.GetName(typeof(EnumCutterLifeStates), stateValue)),
                    Value = stateValue
                });

            }

            modal.CutterLifeIsByCount = (await this.SettingManager.GetSettingValueForApplicationAsync(AppSettings.CutterManagement.LifeMethod)) == "bycount";
            return this.View("~/Views/Cutter/CutterState/Index.cshtml", modal);
        }

        /// <summary>
        /// 装卸刀具Modal
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// </returns>
        public async Task<PartialViewResult> LoadOrUnLoadCutterModal(int? id)
        {
            var model = new CutterStateLoadOrUnLoadViewModel()
            {
                CurrentLoginUserId = this.AbpSession.UserId,
                UserList =
                                    (await this.userRepository.GetAllListAsync())
                                    .Select(
                                        u =>
                                        new NameValueDto(
                                            $"{u.Name}",
                                            u.Id.ToString())).ToList(),
                MachineList =
                                    (await this.machineRepository.GetAllListAsync())
                                    .Select(
                                        m => new NameValueDto(m.Name, m.Id.ToString()))
                                    .ToList()
            };
            if (!id.HasValue)
            {
                return this.PartialView("~/Views/Cutter/CutterState/_LoadOrUnLoadCutterModal.cshtml", model);
            }

            var cutter = await this.cutterStatesRepository.FirstOrDefaultAsync(id.Value);
            if (cutter != null)
            {
                model.CutterNo = cutter.CutterNo;
                model.CutterTVlaue = cutter.CutterTValue;
                model.Id = cutter.Id;
                model.CurrentMachineId = cutter.MachineId;
            }

            return this.PartialView("~/Views/Cutter/CutterState/_LoadOrUnLoadCutterModal.cshtml", model);
        }

        public PartialViewResult UpdateRateModal()
        {
            return this.PartialView("~/Views/Cutter/CutterState/_UpdateRateModal.cshtml");
        }
    }
}
