using Abp.Domain.Uow;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Wimi.BtlCore.RDLCReport;
using Wimi.BtlCore.RDLCReport.Dto;
using Wimi.BtlCore.Web.Controllers.RDLCReport;

namespace Wimi.BtlCore.Web.Views.Shared.Components.ReportView
{
    public class ReportsViewComponent : BtlCoreViewComponent
    {
        private readonly IRDLCReportService reportService;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public ReportsViewComponent(IRDLCReportService reportService, IUnitOfWorkManager unitOfWorkManager)
        {
            this.reportService = reportService;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        
        public async Task<IViewComponentResult> InvokeAsync(ReturnReportModel input)
        {
            var model = new HomeModel()
            {
                webReport = new WebReport()
            };

            using var unitOfWork = this.unitOfWorkManager.Begin();
            
            switch (input.FrxName)
            {
                case "StateConsumeTimeReport":
                    model.webReport.Report.Load("wwwroot/FRX/StateConsumeTime.frx");
                    var stateConsumeTimeReport = reportService.GetStateConsumeTimeReportData(new ReportInputDto() { StartTime = input.StartTime, EndTime = input.EndTime });
                    model.webReport.Report.RegisterData(stateConsumeTimeReport, "DataInit");
                    break;

                case "MachineUtilizationRate":
                    model.webReport.Report.Load("wwwroot/FRX/MachineUtilizationRate.frx");
                    var machineUtilizationRate = reportService.GetMachineUtilizationRateData(new ReportInputDto() { StartTime = input.StartTime, EndTime = input.EndTime });
                    model.webReport.Report.RegisterData(machineUtilizationRate, "DataInit");
                    break;

                case "PersonYield":
                    model.webReport.Report.Load("wwwroot/FRX/PersonYield.frx");
                    var personYield = await reportService.GetPersonYieldReportData(new ReportInputDto() { StartTime = input.StartTime, EndTime = input.EndTime, ShiftSolutionId = Convert.ToInt32(input.ShiftSolutionId) });
                    model.webReport.Report.RegisterData(personYield, "DataInit");
                    break;

                case "PersonPerformance":
                    model.webReport.Report.Load("wwwroot/FRX/PersonPerformance.frx");
                    var personPerformance = reportService.GetPersonPerformanceReportData(new ReportInputDto() { StartTime = input.StartTime, EndTime = input.EndTime });
                    model.webReport.Report.RegisterData(personPerformance, "DataInit");
                    break;

                case "Plan":
                    model.webReport.Report.Load("wwwroot/FRX/Plan.frx");
                    var plan = reportService.ListProcessPlanYield(new ProductPlanYieldInputDto { StartTime = input.StartTime, EndTime = input.EndTime });
                    model.webReport.Report.RegisterData(plan, "DataInit");
                    break;

                default:
                    break;
            }

            await unitOfWork.CompleteAsync();

            model.webReport.Report.Prepare();
            model.webReport.Report.Refresh();
            model.webReport.DesignerLocale = "zh-CHS";
            model.webReport.LocalizationFile = "wwwroot/FRX/Chinese.frl";
            model.webReport.ShowPdfExport = false;
            model.webReport.ShowXpsExport = false;
            model.webReport.ShowTextExport = false;
            model.webReport.ShowXmlExcelExport = false;
            model.webReport.ShowOdsExport = false;
            model.webReport.ShowCsvExport = false;
            model.webReport.ShowOdtExport = false;
            model.webReport.ShowPowerPoint2007Export = false;
            model.webReport.ShowRtfExport = false;
            model.webReport.ShowPreparedReport = false;
            //model.webReport.ShowToolbar = false;
            //await Task.FromResult(0);

            return View("_ReportsView", model);
        }
    }

    public class HomeModel
    {
        public WebReport webReport { get; set; }
    }
}