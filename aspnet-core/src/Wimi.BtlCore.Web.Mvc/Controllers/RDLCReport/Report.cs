using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;

namespace Wimi.BtlCore.Web.Controllers.RDLCReport
{
    /// <summary>
    /// FastReport.Report
    /// </summary>
    public class Report : FastReport.Report
    {
        /// <summary>
        /// 打印图片地址
        /// </summary>
        public string PrintfBmpUrl { get; set; }

        /// <summary>
        /// 打印图片地址集合
        /// </summary>
        public List<string> PrintfBmpUrlList { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string printerName { get; set; }

        /// <summary>
        /// 静默打印
        /// </summary>
        public void StaticPrintf()
        {
            var allPrinter = new List<string>();
            foreach (var item in PrinterSettings.InstalledPrinters)
            {
                allPrinter.Add(item.ToString());
            }
            if (!string.IsNullOrEmpty(printerName))
            {
                if (allPrinter.Any(t => t.ToLower().Equals(this.printerName.ToLower())))
                {
                    using (PrintDocument printDocument = new PrintDocument())
                    {
                        PrintDocument pd = new PrintDocument();
                        pd.PrintPage += new PrintPageEventHandler(printDocument_PrintA4Page);     //打印机名称
                        pd.PrintController = new StandardPrintController();
                        pd.Print();
                    }
                }
            }
            else
            {
                //没有指定打印机，就直接用默认打印机
                using (PrintDocument printDocument = new PrintDocument())
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler(printDocument_PrintA4Page);     //打印机名称
                    pd.PrintController = new StandardPrintController();
                    pd.Print();
                }
            }
        }

        private void printDocument_PrintA4Page(object sender, PrintPageEventArgs e)
        {
            if (string.IsNullOrEmpty(PrintfBmpUrl))
            {
                return;
            }
            Bitmap bitmap = new Bitmap(PrintfBmpUrl);
            e.Graphics.DrawImage(bitmap, new System.Drawing.Point(240, 10));
        }
    }
}