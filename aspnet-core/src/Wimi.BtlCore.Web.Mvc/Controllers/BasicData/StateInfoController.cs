namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    using Abp.AspNetCore.Mvc.Authorization;
    using Abp.AutoMapper;
    using Abp.Domain.Repositories;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.BasicData.StateInfos;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.Web.Models.BasicData.StateInfo;

    [AbpMvcAuthorize(PermissionNames.Pages_BasicData_StateInfo)]
    public class StateInfoController : BtlCoreControllerBase
    {
        private readonly IRepository<StateInfo> stateInfoRepository;

        public StateInfoController(IRepository<StateInfo> stateInfoRepository)
        {
            this.stateInfoRepository = stateInfoRepository;
        }

        public async Task<PartialViewResult> CreateOrUpdateModal(int? id)
        {
            var model = new StateInfoViewModel()
                            {
                                IsEditMode = id.HasValue,
                                YesNoModel = this.GetYesNoSelectListItems()
                            };

            if (!id.HasValue) return this.PartialView("~/Views/BasicData/StateInfo/_CreateOrUpdateModal.cshtml", model);

            var state = await this.stateInfoRepository.GetAsync(id.Value);
            ObjectMapper.Map(state, model);

            return this.PartialView("~/Views/BasicData/StateInfo/_CreateOrUpdateModal.cshtml", model);
        }

        // GET: StateInfo
        public ActionResult Index()
        {
            return this.View("~/Views/BasicData/StateInfo/Index.cshtml");
        }
    }
}