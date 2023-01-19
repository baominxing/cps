using Abp;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Order.WorkOrders;

namespace Wimi.BtlCore.Web.Controllers.Order
{
    public class LoginReportController : BtlCoreControllerBase
    {
        // GET: LoginReport
        public ActionResult Index()
        {
            var list = new List<NameValue<int>>();
            foreach (int stateValue in Enum.GetValues(typeof(EnumWorkOrderState)))
            {
                list.Add(new NameValue<int>()
                {
                    Name = L(Enum.GetName(typeof(EnumWorkOrderState), stateValue)),
                    Value = stateValue
                });

            }
            return this.View("~/Views/Orders/LoginReport/Index.cshtml", list);
        }

        /// <summary>
        /// 设备报工
        /// </summary>
        /// <returns></returns>
        public PartialViewResult MachineReportModal()
        {
            return this.PartialView("~/Views/Orders/LoginReport/_MachineReportModal.cshtml");
        }

        /// <summary>
        /// 工单登录
        /// </summary>
        /// <returns></returns>
        public PartialViewResult WorkOrderLoginModal()
        {
            return this.PartialView("~/Views/Orders/LoginReport/_WorkOrderLoginModal.cshtml");
        }

        /// <summary>
        /// 工单报工
        /// </summary>
        /// <returns></returns>
        public PartialViewResult WorkOrderReportModal()
        {
            return this.PartialView("~/Views/Orders/LoginReport/_WorkOrderReportModal.cshtml");
        }
    }
}