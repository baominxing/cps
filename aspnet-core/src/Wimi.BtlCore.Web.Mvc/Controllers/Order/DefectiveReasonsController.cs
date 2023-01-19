using Abp.Extensions;
using System.Threading.Tasks;

using Abp.AutoMapper;
using Abp.Domain.Repositories;

using Wimi.BtlCore.Authorization;
using Abp.AspNetCore.Mvc.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Order.DefectiveReasons;
using Wimi.BtlCore.Order.DefectiveParts;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Web.Models.Order.DefectiveReason;
using Wimi.BtlCore.Web.Models.Order;

namespace Wimi.BtlCore.Web.Controllers.Order
{
    [AbpMvcAuthorize(PermissionNames.Pages_Order_DefectiveReasons)]
    public class DefectiveReasonsController : BtlCoreControllerBase
    {
        // GET: DefectiveReasons
        private readonly IRepository<DefectiveReason> defectiveReasonRepository;
        private readonly IRepository<DefectivePart> defectivePartRepository;

        public DefectiveReasonsController(IRepository<DefectiveReason> defectiveReasonRepository, IRepository<DefectivePart> defectivePartRepository)
        {
            this.defectiveReasonRepository = defectiveReasonRepository;
            this.defectivePartRepository = defectivePartRepository;
        }

        public async Task<PartialViewResult> CreateOrUpdateModal(int? id,int partId,string partCode)
        {
            var model = new DefectiveReasonModel();
            if (partId != 0 && !partCode.IsNullOrEmpty())
            {
                model.PartId = partId;
                model.PartCode = partCode;
            }
            if (id.HasValue)
            {
                var dectiveReason = await this.defectiveReasonRepository.GetAsync(id.Value);
                if (dectiveReason != null)
                {
                    ObjectMapper.Map(dectiveReason, model);
                    model.IsEditMode = true;
                }
            }

            return this.PartialView("~/Views/Orders/DefectiveReasons/_CreateOrUpdateModal.cshtml", model);
        }

        public PartialViewResult CreateDefectivePartModal(int? parentId)
        {
            return this.PartialView("~/Views/Orders/DefectiveReasons/_CreatePartModal.cshtml",
                new CreateDefectivePartModal(parentId));
        }

        public async Task<PartialViewResult> EditDefectivePartModal(int id)
        {
            var defectivePart = await this.defectivePartRepository.GetAsync(id);
            var modal = ObjectMapper.Map<EditDefectivePartModal>(defectivePart);
            return this.PartialView("~/Views/Orders/DefectiveReasons/_EditPartModal.cshtml", modal);
        }

        public ActionResult Index()
        {
            return this.View("~/Views/Orders/DefectiveReasons/Index.cshtml");
        }
    }
}