namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    using Abp.AspNetCore.Mvc.Authorization;
    using Abp.Domain.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.BasicData.MachineTypes;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.Web.Models.BasicData.MachineType;

    [AbpMvcAuthorize(PermissionNames.Pages_BasicData_MachineType)]
    public class MachineTypeController : BtlCoreControllerBase
    {
        // GET: MachineType
        private readonly IRepository<MachineType> machineTypeRepository;

        public MachineTypeController(IRepository<MachineType> machineTypeRepository)
        {
            this.machineTypeRepository = machineTypeRepository;
        }

        public async Task<PartialViewResult> CreateOrUpdateModal(int? id)
        {
            var model = new MachineTypeModel();
            if (id.HasValue)
            {
                var machineType = await this.machineTypeRepository.GetAsync(id.Value);
                if (machineType != null)
                {
                    model = ObjectMapper.Map<MachineTypeModel>(machineType);
                    model.IsEditMode = true;
                }
            }

            return this.PartialView("~/Views/BasicData/MachineType/_CreateOrUpdateModal.cshtml", model);
        }

        // GET: StateInfo
        public ActionResult Index()
        {
            return this.View("~/Views/BasicData/MachineType/Index.cshtml");
        }
    }
}