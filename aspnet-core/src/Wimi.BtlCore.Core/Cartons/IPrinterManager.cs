using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Cartons
{
    public interface IPrinterManager : IDomainService
    {
        List<string> GetInstalledPrinters();

        /// <summary>
        /// 打印标签
        /// </summary>
        /// <param name="reportPath"> rdlcl路径,通过rdlc报表文件来设置标签布局</param>
        /// <param name="printerName">打印机名称</param>
        /// <param name="base64CartonNo">箱码的Base64编码</param>
        /// <param name="cartonNo">箱码</param>
        /// <param name="isExport">是否将箱码标签保存为本地图片</param>
        void PrintLabel(string reportPath, string printerName, string base64CartonNo, string cartonNo, bool isExport);
    }
}
