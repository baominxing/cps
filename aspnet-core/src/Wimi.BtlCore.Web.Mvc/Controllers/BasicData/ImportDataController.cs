using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData;
using Wimi.BtlCore.BasicData.Dto;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Dmps;
using Wimi.BtlCore.Web.Models.BasicData.ImportData;

namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    [AbpMvcAuthorize(PermissionNames.Pages_BasicData_ImportData)]
    public class ImportDataController : BtlCoreControllerBase
    {
        private readonly IBasicDataAppService basicDataAppService;

        public ImportDataController(IBasicDataAppService basicDataAppService)
        {
            this.basicDataAppService = basicDataAppService;
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

                if ((file.ContentType.Length> 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    using (var package = new ExcelPackage(file.OpenReadStream()))
                    {
                        try
                        {
                            var workSheets = package.Workbook.Worksheets;
                            if (workSheets.Count != 0)
                            {
                                var data =
                                    this.GetImportData(
                                        new ImportDataInputDto
                                            {
                                                ExcelWorksheet = workSheets[1], 
                                                Type = this.Request.Form["Type"]
                                            });

                                json.Value = await this.basicDataAppService.ValidateImportData(data);
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

        public PartialViewResult ImportDataModal()
        {
            return this.PartialView("~/Views/BasicData/ImportData/_ImportDataModal.cshtml");
        }

        /// <summary>
        /// ImportData
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var modal = new ImportDataViewModal
                            {
                                ImportType =
                                    Enum.GetValues(typeof(EnumImportTypes)).Cast<EnumImportTypes>()
                            };

            return this.View("~/Views/BasicData/ImportData/Index.cshtml", modal);
        }

   

        private ImportDataOutputDto GetImportData(ImportDataInputDto input)
        {
            EnumImportTypes inputType;
            if (!Enum.TryParse(input.Type, out inputType))
            {
                inputType = EnumImportTypes.Users;
            }

            ImportDataOutputDto returnValue;
            switch (inputType)
            {
                case EnumImportTypes.Users:
                    returnValue = this.GetUsersData(input.ExcelWorksheet);
                    break;
                case EnumImportTypes.Machines:
                    returnValue = this.GetMachinesData(input.ExcelWorksheet);
                    break;
                default:
                    returnValue = this.GetGatherParamsData(input.ExcelWorksheet);
                    break;
            }

            returnValue.Type = inputType;
            return returnValue;
        }

        private ImportDataOutputDto GetMachinesData(ExcelWorksheet excelWorksheet)
        {
            var machineList = new List<ImportMachinesOutputDto>();
            for (var rowIterator = 2; rowIterator <= excelWorksheet.Dimension.End.Row; rowIterator++)
            {
                var isActive = excelWorksheet.Cells[rowIterator, 5].Value?.Equals("Y");

                machineList.Add(
                    new ImportMachinesOutputDto
                        {
                            Seq = rowIterator - 1, 
                            Code = excelWorksheet.Cells[rowIterator, 1].Value?.ToString().Trim(), 
                            Name = excelWorksheet.Cells[rowIterator, 2].Value?.ToString().Trim(), 
                            Type = excelWorksheet.Cells[rowIterator, 3].Value?.ToString().Trim(), 
                            Description =
                                excelWorksheet.Cells[rowIterator, 4].Value?.ToString().Trim(), 
                            IsActive = isActive != null && (bool)isActive
                        });
            }

            return new ImportDataOutputDto { MachinesList = machineList };
        }

        private ImportDataOutputDto GetUsersData(ExcelWorksheet excelWorksheet)
        {
            var usersList = new List<ImportUsersOutputDto>();
            for (var rowIterator = 2; rowIterator <= excelWorksheet.Dimension.End.Row; rowIterator++)
            {
                var isActive = excelWorksheet.Cells[rowIterator, 6].Value?.Equals("Y");
                var shouldChangePasswordOnNextLogin = excelWorksheet.Cells[rowIterator, 7].Value?.Equals("Y");

                // 读取数据
                usersList.Add(
                    new ImportUsersOutputDto
                        {
                            Seq = rowIterator - 1,  
                            Name = excelWorksheet.Cells[rowIterator, 1].Value?.ToString().Trim(), 
                            UserName = excelWorksheet.Cells[rowIterator, 2].Value?.ToString().Trim(), 
                            Password = excelWorksheet.Cells[rowIterator, 3].Value?.ToString().Trim(), 
                            RolesName = excelWorksheet.Cells[rowIterator, 4].Value?.ToString().Trim(), 
                            WeChatId = excelWorksheet.Cells[rowIterator, 5].Value?.ToString().Trim(), 
                            IsActive = isActive != null && (bool)isActive, 
                            ShouldChangePasswordOnNextLogin =
                                shouldChangePasswordOnNextLogin != null
                                && (bool)shouldChangePasswordOnNextLogin
                        });
            }

            return new ImportDataOutputDto { UsersList = usersList };
        }

        private ImportDataOutputDto GetGatherParamsData(ExcelWorksheet excelWorksheet)
        {
            var gatherParamsList = new List<ImportGatherParamsOutputDto>();
            var dataTypeList = this.GetDataTypeList();
            var accessList = this.GetAccessList();

            for (var rowIterator = 2; rowIterator <= excelWorksheet.Dimension.End.Row; rowIterator++)
            {
                var dataTypeItem = dataTypeList.FirstOrDefault(x=> excelWorksheet.Cells[rowIterator, 5].Value?.ToString().Trim() == x.Name);
                var dataType = dataTypeItem == null ? string.Empty : dataTypeItem.Value;

                var accessItem = accessList.FirstOrDefault(x => excelWorksheet.Cells[rowIterator, 6].Value?.ToString().Trim() == x.Name);
                var access = accessItem == null ? 0 : accessItem.Value;
                // 读取数据
                gatherParamsList.Add(
                    new ImportGatherParamsOutputDto
                    {
                        Seq = rowIterator - 1,
                        MachineId = excelWorksheet.Cells[rowIterator,1].Value?.ToString().Trim(),
                        Code = excelWorksheet.Cells[rowIterator, 2].Value?.ToString().Trim(),
                        Description = excelWorksheet.Cells[rowIterator, 3].Value?.ToString().Trim(),
                        DeviceAddress = excelWorksheet.Cells[rowIterator, 4].Value?.ToString().Trim(),
                        DataType = dataType,
                        DataTypeString = excelWorksheet.Cells[rowIterator, 5].Value?.ToString().Trim(),
                        Access = access,
                        AccessString = excelWorksheet.Cells[rowIterator, 6].Value?.ToString().Trim(),
                        ValueFactor = excelWorksheet.Cells[rowIterator, 7].Value?.ToString().Trim(),
                        DefaultValue = excelWorksheet.Cells[rowIterator, 8].Value?.ToString().Trim()
                    });
            }

            return new ImportDataOutputDto { GatherParamsList = gatherParamsList };
        }

        private List<NameValueDto> GetDataTypeList ()
        {
            var query = new List<NameValueDto<int>>();
            foreach (var e in Enum.GetValues(typeof(EnumDataType)))
            {
                FieldInfo field = e.GetType().GetField(e.ToString());
                object[] objs = field.GetCustomAttributes(typeof(DisplayAttribute), false);    //获取描述属性
                DisplayAttribute displayAttribute = (DisplayAttribute)objs[0];
                query.Add(new NameValueDto<int> { Name = displayAttribute.Name, Value = (int)e });
            }
            return query.Select(x => new NameValueDto { Name = x.Name, Value = x.Value.ToString() }).ToList();
        }

        private List<NameValueDto<int>> GetAccessList()
        {
            var query = new List<NameValueDto<int>>();
            foreach (var e in Enum.GetValues(typeof(EnumAccess)))
            {
                FieldInfo field = e.GetType().GetField(e.ToString());
                object[] objs = field.GetCustomAttributes(typeof(DisplayAttribute), false);    //获取描述属性
                DisplayAttribute displayAttribute = (DisplayAttribute)objs[0];
                query.Add(new NameValueDto<int> { Name = displayAttribute.Name, Value = (int)e });
            }
            return query;
        }

    }
}