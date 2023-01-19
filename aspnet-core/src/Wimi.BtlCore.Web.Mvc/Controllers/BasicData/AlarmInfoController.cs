using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Alarms;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.StatisticAnalysis.Alarms;
using Wimi.BtlCore.StatisticAnalysis.Alarms.Dto;
using Wimi.BtlCore.Web.Models.BasicData.AlarmInfos;

namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    public class AlarmInfoController : BtlCoreControllerBase
    {
        private readonly IRepository<AlarmInfo, long> alarmRepository;

        private readonly IAlarmsAppService alarmsAppService;

        public AlarmInfoController(IRepository<AlarmInfo, long> alarmRepository, IAlarmsAppService alarmsAppService)
        {
            this.alarmRepository = alarmRepository;
            this.alarmsAppService = alarmsAppService;
        }

        public async Task<PartialViewResult> CreateOrEditModal(long? id)
        {
            var modal = new AlarmInfoViewModel();
            if (id.HasValue)
            {
                var alarmInfo = await this.alarmRepository.GetAsync(id.Value);
                if (alarmInfo != null)
                {
                    modal.AlarmInfo = ObjectMapper.Map<CreateOrEditAlarmInfoDto>(alarmInfo);
                    modal.IsEditModal = true;
                }
            }
            return this.PartialView("~/Views/BasicData/AlarmInfo/_CreateOrEditModal.cshtml", modal);
        }

        [HttpPost]
        public async Task<JsonResult> ImportData([FromHeader]ImportAlarmInfoDataInput input)
        {
            var json = new JsonResult(true);
            if (this.Request != null)
            {
                var file = this.Request.Form.Files["UploadFiles"];
                if (file == null || (file.FileName.Length == 0))
                {
                    json.Value = this.L("ChooseFiles");
                    return this.Json(json);
                }

                if (!file.FileName.Contains("xls") || !file.FileName.Contains("xlsx"))
                {
                    json.Value = this.L("NotSupportFileImport{0}", file.FileName.Split('.').Last());
                    return this.Json(json);
                }

                // 1.获取文件数据并保存到DataTable对象   
                using (var package = new ExcelPackage(file.OpenReadStream()))
                {
                    try
                    {
                        var workSheets = package.Workbook.Worksheets;
                        if (workSheets.Count != 0)
                        {
                            Debug.Assert(input.MachineId != null, "machineId != null");
                            var importList = this.GetDataSourceFromUploadedFile(workSheets[1], (int)input.MachineId);
                            if (input.ImportType == 0)
                            {
                                await this.alarmsAppService.ImportDataByCover(
                                    new ImportDataDto() { ImportData = importList, MachineId = (int)input.MachineId });
                            }
                            else
                            {
                                await this.alarmsAppService.ImportDataByIncrement(
                                    new ImportDataDto() { ImportData = importList });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        json.Value = ex.Message;
                    }
                }
            }

            return this.Json(json);
        }

        public PartialViewResult ImportModal()
        {
            return this.PartialView("~/Views/BasicData/AlarmInfo/_ImportModal.cshtml");
        }

        // GET: AlarmInfo
        public ActionResult Index()
        {
            return this.View("~/Views/BasicData/AlarmInfo/Index.cshtml");
        }

        private IEnumerable<AlarmInfoDto> GetDataSourceFromUploadedFile(ExcelWorksheet excelWorksheet, int machineId)
        {
            var noOfRowOfTool = excelWorksheet.Dimension.End.Row;
            var addList = new List<AlarmInfoDto>();
            for (var rowIterator = 2; rowIterator <= noOfRowOfTool; rowIterator++)
            {
                var code = excelWorksheet.Cells[rowIterator, 1].Value?.ToString();
                if (string.IsNullOrEmpty(code))
                {
                    continue;
                }

                // 报警内容
                var message = excelWorksheet.Cells[rowIterator, 2].Value?.ToString();

                var userId = this.AbpSession.GetUserId();

                var alarmInfo = addList.FirstOrDefault(a => a.Code.Equals(code));
                if (alarmInfo != null)
                {
                    throw new UserFriendlyException(this.L("RepeatDataInFile{0}", code));
                }

                addList.Add(
                    new AlarmInfoDto()
                    {
                        Code = code,
                        Message = message,
                        CreationTime = DateTime.Now,
                        CreatorUserId = (int)userId,
                        MachineId = machineId
                    });
            }

            return addList;
        }
    }

    public class ImportAlarmInfoDataInput
    {
        public int? MachineId { get; set; }

        public int? ImportType { get; set; }
    }
}