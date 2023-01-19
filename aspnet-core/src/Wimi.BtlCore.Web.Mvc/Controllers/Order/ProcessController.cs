
using Abp.AspNetCore.Mvc.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Order.Processes;
using Wimi.BtlCore.Web.Models.Order.Process;

namespace Wimi.BtlCore.Web.Controllers.Order
{
    [AbpMvcAuthorize(PermissionNames.Pages_Order_Process)]
    public class ProcessController : BtlCoreControllerBase
    {
        // GET: Process
        private readonly IRepository<Process> processRepository;

        public ProcessController(IRepository<Process> processRepository)
        {
            this.processRepository = processRepository;
        }

        public async Task<PartialViewResult> CreateOrUpdateModal(int? id)
        {
            var model = new ProcessModel();
            if (id.HasValue)
            {
                var process = await this.processRepository.GetAsync(id.Value);
                if (process != null)
                {
                    ObjectMapper.Map(process, model);
                    model.IsEditMode = true;
                }
            }

            return this.PartialView("~/Views/Orders/Process/_CreateOrUpdateModal.cshtml", model);
        }

        public ActionResult Index()
        {
            return this.View("~/Views/Orders/Process/Index.cshtml");
        }
    }
}