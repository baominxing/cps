using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.RDLCReport;
using Wimi.BtlCore.RDLCReport.Dto;

namespace Wimi.BtlCore.Web.Controllers.RDLCReport
{
    public class RDLCReportController : BtlCoreControllerBase
    {
        private readonly IRDLCReportService reportService;

        public RDLCReportController(IRDLCReportService reportService)
        {
            this.reportService = reportService;
        }

        // GET: ReportShow
        public ActionResult Index()
        {
            return View();
        }

        public IList<MachineUtilizationRateDto> GetMachineUtilizationRateData()
        {
            IList<MachineUtilizationRateDto> machineUtilizationRateDtos = new List<MachineUtilizationRateDto>();
            MachineUtilizationRateDto machineUtilizationRateDto = new MachineUtilizationRateDto();
            machineUtilizationRateDto.MachineGroup = "设备组8";
            machineUtilizationRateDto.Machine = "设备1";
            machineUtilizationRateDto.Date = DateTime.Today.ToString();
            machineUtilizationRateDto.Duration = 10;
            machineUtilizationRateDtos.Add(machineUtilizationRateDto);
            MachineUtilizationRateDto machineUtilizationRateDto2 = new MachineUtilizationRateDto();
            machineUtilizationRateDto2.MachineGroup = "设备组2";
            machineUtilizationRateDto2.Machine = "设备2";
            machineUtilizationRateDto.Date = DateTime.Today.ToString();
            machineUtilizationRateDto.Duration = 11;
            machineUtilizationRateDtos.Add(machineUtilizationRateDto2);
            return machineUtilizationRateDtos;
        }

        /// <summary>
        /// 设备稼动率
        /// </summary>
        /// <returns></returns>
        public ActionResult MachineUtilizationRateReport(ReportInputDto input)
        {
            if (input.StartTime == new DateTime())
            {
                input.StartTime = DateTime.Today;
                input.EndTime = DateTime.Today;
            }
            var returnReportModel = new ReturnReportModel
            {
                FrxName = "MachineUtilizationRate",
                StartTime = input.StartTime,
                EndTime = input.EndTime
            };
            return View(returnReportModel);
        }

        /// <summary>
        /// 设备稼动率
        /// </summary>
        /// <returns></returns>

        public ActionResult MachineUtilizationRateReportTest()
        {
            //var data = reportService.GetMachineUtilizationRateData(input);
            var data = GetMachineUtilizationRateData();
            WebReport webReport = new WebReport();

            webReport.Report.Load("wwwroot/FRX/MachineUtilizationRate.frx");
            webReport.Report.RegisterData(data, "DataInit");

            webReport.Report.Refresh();

            webReport.Report.Prepare();
            webReport.DesignerLocale = "zh-CHS";
            webReport.LocalizationFile = "wwwroot/FRX/Chinese.frl";
            ViewBag.webReport = webReport;

            return View();
        }

        /// <summary>
        /// 状态占比用时分析（设备综合分析）
        /// </summary>
        /// <returns></returns>
        public ActionResult StateConsumeTimeReport(ReportInputDto input)
        {
            if (input.StartTime == new DateTime())
            {
                input.StartTime = DateTime.Today;
                input.EndTime = DateTime.Today;
            }
            var returnReportModel = new ReturnReportModel
            {
                FrxName = "StateConsumeTimeReport",
                StartTime = input.StartTime,
                EndTime = input.EndTime
            };
            return View(returnReportModel);
        }

        /// <summary>
        /// 刷新使用
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateViewComponent(ReportInputDto input)
        {
            if (input.StartTime == new DateTime())
            {
                input.StartTime = DateTime.Today;
                input.EndTime = DateTime.Today;
            }

            var returnReportModel = new ReturnReportModel
            {
                FrxName = input.ReportName,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                ShiftSolutionId = input.ShiftSolutionId
            };

            return await Task.FromResult(ViewComponent("Reports", returnReportModel));

        }

        /// <summary>
        /// 人员产量
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonYieldReport(ReportInputDto input)
        {
            if (input.StartTime == new DateTime())
            {
                input.StartTime = DateTime.Today;
                input.EndTime = DateTime.Today;
            }
            var returnReportModel = new ReturnReportModel
            {
                FrxName = "PersonYield",
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                ShiftSolutionId = Convert.ToInt32(input.ShiftSolutionId)
            };
            return View(returnReportModel);
        }

        /// <summary>
        /// 人员绩效
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ActionResult PersonPerformanceReport(ReportInputDto input)
        {
            if (input.StartTime == new DateTime())
            {
                input.StartTime = DateTime.Today;
                input.EndTime = DateTime.Today;
            }
            var returnReportModel = new ReturnReportModel
            {
                FrxName = "PersonPerformance",
                StartTime = input.StartTime,
                EndTime = input.EndTime
            };
            return View(returnReportModel);
        }

        /// <summary>
        /// 计划达成报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ActionResult ReportOfPlan(ReportInputDto input)
        {
            if (input.StartTime == new DateTime())
            {
                input.StartTime = DateTime.Today;
                input.EndTime = DateTime.Today;
            }
            var returnReportModel = new ReturnReportModel
            {
                FrxName = "Plan",
                StartTime = input.StartTime,
                EndTime = input.EndTime
            };
            return View(returnReportModel);
        }
    }
}