using System.Linq;
using System.Threading.Tasks;

using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.StatisticAnalysis.YieldAnalysisStatistics;

namespace Wimi.BtlCore.Web.Controllers.StatisticAnalysis
{
    public class YieldAnalysisStatisticsController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        private IRepository<StateInfo> stateInfoRepository;

        public YieldAnalysisStatisticsController(
            ICommonLookupAppService commonLookupAppService,
            IRepository<StateInfo> stateInfoRepository)
        {
            this.commonLookupAppService = commonLookupAppService;
            this.stateInfoRepository = stateInfoRepository;
        }

        // 比较设备产量弹出框
        public PartialViewResult CompareMachineYieldModal()
        {
            return this.PartialView("~/Views/StatisticAnalysis/YieldAnalysisStatistics/_CompareMachineYieldModal.cshtml");
        }

        // GET: YieldAnalysisStatistics
        public ActionResult Index()
        {
            var model = new YieldAnalysisStatisticsViewModel()
            {
                StateInfoList = this.stateInfoRepository.GetAll().ToList()
            };
            return this.View("~/Views/StatisticAnalysis/YieldAnalysisStatistics/Index.cshtml", model);
        }

        public async Task<PartialViewResult> SelectQueryConditionsModal()
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();
            var model = new GetQueryConditionsViewModel()
            {
                Machines = deviceGroupAndMachineWithPermissions.Machines,
                GrantedGroupIds = deviceGroupAndMachineWithPermissions.GrantedGroupIds,
                DeviceGroups = deviceGroupAndMachineWithPermissions.DeviceGroups
            };

            return this.PartialView("~/Views/StatisticAnalysis/YieldAnalysisStatistics/_SelectQueryConditionsModal.cshtml", model);
        }

        // 显示详情弹出框
        public PartialViewResult ShowDetailYieldModal()
        {
            return this.PartialView("~/Views/StatisticAnalysis/YieldAnalysisStatistics/_ShowDetailYieldModal.cshtml");
        }
    }
}