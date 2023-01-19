using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Wimi.BtlCore.AppSystem.Net.MimeTypes;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Wimi.BtlCore.BasicData.Dto;
using Wimi.BtlCore.DataExporting.Excel.EpPlus;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.BasicData.Exporting
{
    public class Exporter : EpPlusExcelExporterBase, IExporter
    {
        public FileDto ExportToFile(List<MachineSettingListDto> machineSettingListDtos)
        {
            return this.CreateExcelPackage(
                "Machines.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(this.L("Machines"));
                    sheet.OutLineApplyStyle = true;

                    this.AddHeader(
                        sheet,
                        "Id",
                        "Name");

                    this.AddObjects(
                        sheet,
                        2,
                        machineSettingListDtos,
                        s => s.Id,
                        s => s.Name);
                });
        }

        public FileDto ExportMachineToFile(List<MachineDto> input)
        {
            return this.CreateExcelPackage(
                "Machines.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(this.L("Machines"));
                    sheet.OutLineApplyStyle = true;

                    this.AddHeader(
                        sheet,
                        this.L("MachineId"),
                        this.L("MachineCode"),
                        this.L("MachineName"),
                        this.L("DeviceType"),
                        this.L("SortSeq"),
                        this.L("IsEnable"),
                        this.L("IpAddress"),
                        this.L("TcpPort")
                        );

                    this.AddObjects(
                        sheet,
                        2,
                        input,
                        s => s.Id,
                        s => s.Code,
                        s => s.Name,
                        s => s.MachineTypeName,
                        s => s.SortSeq,
                        s => s.IsActive ? "是" : "否",
                        s => s.IpAddress,
                        s => s.TcpPort
                        );

                    SetBorders(8, 1 + input.Count, sheet);
                });
        }

        public FileDto ExportMachineToXML(List<ExportMachineDto> machineList, List<ExportMachineDto> groupList)
        {
            var xdoc = new XDocument();
            var root = new XElement("root", new XAttribute("type", "DMP.Configuration.Data"));
            var deviceData = new XElement("DeviceData");
            var groups = new XElement("groups");

            foreach (var item in groupList)
            {
                var group = new XElement("group",
                    new XAttribute("ID", item.GroupId),
                    new XAttribute("GroupID", item.ParentGroupId),
                    new XAttribute("Name", item.GroupName),
                    new XAttribute("Description", string.Empty)
                    );

                groups.Add(group);
            }

            deviceData.Add(groups);

            var devices = new XElement("devices");

            foreach (var item in machineList)
            {
                var device = new XElement("device",
                    new XAttribute("ID", item.MachineId),
                    new XAttribute("GroupID", item.GroupId),
                    new XAttribute("Name", item.MachineName),
                    new XAttribute("Description", item.MachineName),
                    new XAttribute("DriverName", item.DriverName),
                    new XAttribute("TypeName", item.TypeName),
                    new XAttribute("Enable", "True")
                    );

                this.AddSimulateVariables(device);

                devices.Add(device);
            }

            deviceData.Add(devices);

            root.Add(deviceData);

            xdoc.Add(root);

            var file = new FileDto("Machines.xml", MimeTypeNames.TextXml);
            var filePath = Path.Combine(this.AppFolders.TempFileDownloadFolder, file.FileToken);
            xdoc.Save(filePath);

            return file;
        }


        private void SetBorders(int col, int row, ExcelWorksheet sheet)
        {
            //设置边框
            for (int i = 1; i <= col; i++)
            {
                for (int n = 1; n <= row; n++)
                {
                    sheet.Cells[n, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[n, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[n, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[n, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[n, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[n, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    sheet.Cells[n, i].Style.Font.Name = "宋体";//字体
                    sheet.Cells[n, i].Style.Font.Size = 10;//字体大小
                }

                sheet.Column(i).AutoFit();
            }
        }

        private void AddSimulateVariables(XElement device)
        {
            var variables = new XElement("variables");

            variables.Add(new XElement("variable"
                , new XAttribute("ID", Guid.NewGuid().ToString())
                , new XAttribute("Index", "0")
                , new XAttribute("Name", "Name")
                , new XAttribute("Description", "Description")
                , new XAttribute("DeviceAddress", "DeviceAddress")
                , new XAttribute("DataLength", "0")
                , new XAttribute("DataType", "0")
                , new XAttribute("Access", "2")
                , new XAttribute("ValueFactor", "0")
                , new XAttribute("InitialValue", "0")
                ));
            device.Add(variables);

            var driver = new XElement("driver");

            driver.Add(
                new XElement("item", new XAttribute("Name", "IOTimeout"), new XAttribute("Value", "10000")),
                new XElement("item", new XAttribute("Name", "IpAddress"), new XAttribute("Value", "127.0.0.1")),
                new XElement("item", new XAttribute("Name", "ScanInterval"), new XAttribute("Value", "3000")),
                new XElement("item", new XAttribute("Name", "TargetName"), new XAttribute("Value", "Device0")),
                new XElement("item", new XAttribute("Name", "TcpPort"), new XAttribute("Value", "20170")),
                new XElement("item", new XAttribute("Name", "WriteProtect"), new XAttribute("Value", "false"))
                );

            device.Add(driver);
        }
    }
}