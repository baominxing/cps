using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.Archives.Dtos;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.Archives;

namespace Wimi.BtlCore.Web.Controllers.Archives
{
    [AbpMvcAuthorize(PermissionNames.Pages_Archives_ArchiveEntry)]
    public class ArchiveEntryController : BtlCoreControllerBase
    {
        // GET: Process
        private readonly IArchiveEntryAppService archiveentryAppService;

        public ArchiveEntryController(IArchiveEntryAppService archiveentryAppService)
        {
            this.archiveentryAppService = archiveentryAppService;
        }

        public ActionResult Index()
        {
            return this.View("~/Views/Archives/ArchiveEntry/Index.cshtml");
        }

        public async Task<PartialViewResult> CreateOrUpdateModal(int? id)
        {
            var model = new ArchiveEntryViewModel();

            if (id.HasValue)
            {
                var dto = await this.archiveentryAppService.Get(new ArchiveEntryInputDto() { Id = id.Value });

                if (dto != null)
                {
                    model.Dto = dto;
                    model.IsEditMode = true;
                }
            }

            return this.PartialView("~/Views/Archives/ArchiveEntry/CreateOrUpdateModal.cshtml", model);
        }
    }

}
