using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Carton.CartonRules;
using Wimi.BtlCore.Carton.CartonRules.Dtos;
using Wimi.BtlCore.Cartons;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.DeviceGroups;
using Wimi.BtlCore.Shifts;
using Wimi.BtlCore.Web.Models.Carton.CartonRules;

namespace Wimi.BtlCore.Web.Controllers.Carton
{
    public class CartonRulesController : BtlCoreControllerBase
    {
        private readonly IRepository<CartonRule> cartonRuleRepository;
        private readonly IRepository<CartonRuleDetail> cartonRuleDetailRepository;
        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemRepository;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly IShiftAppService shiftAppService;
        private readonly IDeviceGroupAppService deviceGroupAppService;
        private readonly ICartonRuleAppService cartonRuleAppService;

        public CartonRulesController(IRepository<CartonRule> cartonRuleRepository,
                                     IRepository<CartonRuleDetail> cartonRuleDetailRepository,
                                     IRepository<ShiftSolutionItem> shiftSolutionItemRepository,
                                     IRepository<DeviceGroup> deviceGroupRepository,
                                     IShiftAppService shiftAppService,
                                     IDeviceGroupAppService deviceGroupAppService,
                                     ICartonRuleAppService cartonRuleAppService)
        {
            this.cartonRuleRepository = cartonRuleRepository;
            this.cartonRuleDetailRepository = cartonRuleDetailRepository;
            this.shiftSolutionItemRepository = shiftSolutionItemRepository;
            this.deviceGroupRepository = deviceGroupRepository;
            this.shiftAppService = shiftAppService;
            this.deviceGroupAppService = deviceGroupAppService;
            this.cartonRuleAppService = cartonRuleAppService;
        }

        // GET: CartonRules
        public ActionResult Index()
        {
            return View("/Views/Carton/CartonRules/Index.cshtml");
        }

        public PartialViewResult CreateOrUpdateCartonRules(int? id)
        {
            var model = new CartonRuleCreateOrUpdateModel()
            {
                IsEditMode = false
            };

            if (id.HasValue && id != 0)
            {
                var rule = this.cartonRuleRepository.FirstOrDefault(c => c.Id == id.Value);

                if (rule != null)
                {
                    model.IsEditMode = true;
                    model.Id = id;
                    model.IsActive = rule.IsActive;
                    model.Name = rule.Name;
                }
            }

            return this.PartialView("/Views/Carton/CartonRules/_CreateOrUpdateCartonRules.cshtml", model);
        }

        public async Task<PartialViewResult> CreateRulesDetail(int ruleId)
        {
            var deviceGroups = await this.deviceGroupAppService.ListFirstClassDeviceGroups();
            var shiftSolutions = await this.shiftAppService.GetShiftSolutions();
            var typeSelect = await this.cartonRuleAppService.GetTypeSeletById(new EntityDto<int>() { Id = ruleId });

            var model = new RuleDetailCreateModel()
            {
                RuleId = ruleId,
                FirstDeviceGroups = deviceGroups.Select(d => new NameValueDto<int>() { Name = d.DisplayName, Value = d.Id }).ToList(),
                ShiftSolutions = shiftSolutions.Items.Select(s => new NameValueDto<int>() { Name = s.Name, Value = s.Id }).ToList(),
                TypeSelect = typeSelect
            };

            return this.PartialView("/Views/Carton/CartonRules/_CreateRulesDetail.cshtml", model);
        }

        [HttpPost]
        public async Task<JsonResult> ImportData()
        {
            var json = new JsonResult(true);

            if (this.Request != null)
            {
                var file = this.Request.Form.Files["UploadFiles"];
                if (file == null || (file.FileName.Length == 0))
                {
                    json.Value = this.L("ChooseFiles");
                    return json;
                }

                if (!file.FileName.Contains("xls"))
                {
                    json.Value = this.L("NotSupportFileImport{0}", file.FileName.Split('.').Last());
                    return json;
                }

                if ((file.ContentType.Length > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    using (var package = new ExcelPackage(file.OpenReadStream()))
                    {
                        try
                        {
                            var workSheets = package.Workbook.Worksheets;
                            if (workSheets.Count != 0)
                            {
                                var ruleId = Convert.ToInt32(this.Request.Form["RuleId"]);
                                var data =
                                    this.GetImportData(workSheets[1], ruleId);

                                json.Value = await this.cartonRuleAppService.ImportCalibratorCodes(data);
                            }
                        }
                        catch (Exception ex)
                        {
                            json.Value = ex.Message;
                        }
                    }
                }
            }

            return this.Json(json);
        }

        public PartialViewResult Examine()
        {
            return this.PartialView("/Views/Carton/CartonRules/_Examine.cshtml");
        }

        private List<ImportDatasDto> GetImportData(ExcelWorksheet worksheet, int ruleId)
        {
            var returnList = new List<ImportDatasDto>();

            if (worksheet.Dimension.End.Row < 3 || worksheet.Cells[2, 1].Value.ToString() != "余数" || worksheet.Cells[2, 2].Value.ToString() != "检查符对应字符")
            {
                throw new Exception("导入Excel格式不符合模板格式");
            }
            for (var rowIterator = 3; rowIterator <= worksheet.Dimension.End.Row; rowIterator++)
            {
                for (var colIterator = 1; colIterator <= worksheet.Dimension.End.Column; colIterator = colIterator + 2)
                {
                    var key = worksheet.Cells[rowIterator, colIterator].Value;
                    var value = worksheet.Cells[rowIterator, colIterator + 1].Value;
                    if (key != null && value != null)
                    {
                        returnList.Add(new ImportDatasDto()
                        {
                            CartonRuleId = ruleId,
                            Key = Convert.ToInt32(key.ToString()),
                            Value = value.ToString()
                        });
                    }
                }
            }
            return returnList.OrderBy(r => r.Key).ToList();
        }

        public async Task<PartialViewResult> UpdateRulesDetail(int id)
        {
            var deviceGroups = await this.deviceGroupAppService.ListFirstClassDeviceGroups();


            var detailItem = await this.cartonRuleAppService.GetRuleDetailForEdit(new EntityDto<int>() { Id = id });
            var model = new RuleDetailUpdateModel()
            {
                RuleDetailItem = detailItem,
                RuleId = detailItem.CartonRuleId
            };

            if (detailItem.Type == EnumRuleType.Shift)
            {
                var shiftItem = await this.shiftSolutionItemRepository.GetAsync(detailItem.ExpansionKey);
                model.ShiftInfos = new InfosValueDto() { Id = shiftItem.Id, Name = shiftItem.Name, Value = detailItem.Value };
            }

            if (detailItem.Type == EnumRuleType.Line)
            {
                var deviceGroup = await this.deviceGroupRepository.GetAsync(detailItem.ExpansionKey);
                model.DeviceGroupInfos = new InfosValueDto() { Id = deviceGroup.Id, Name = deviceGroup.DisplayName, Value = detailItem.Value };
            }

            return this.PartialView("/Views/Carton/CartonRules/_UpdateRulesDetail.cshtml", model);
        }
    }
}
