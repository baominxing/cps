using FastReport;
using FastReport.Export.Image;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;

namespace Wimi.BtlCore.Cartons
{
    public class PrinterManager : BtlCoreDomainServiceBase, IPrinterManager
    {
        private int m_currentPageIndex;
        private List<Stream> m_streams = new List<Stream>();

        public List<string> GetInstalledPrinters()
        {
            var allPrinter = PrinterSettings.InstalledPrinters.Cast<string>().ToList();

            return allPrinter;
        }

        public void PrintLabel(string reportPath, string printerName, string base64CartonNo, string cartonNo, bool isExport)
        {
            Report report = new Report();
            report.Load(reportPath);
            List<PrintModel> t = new List<PrintModel>();
            t.Add(new PrintModel()
            {
                Base64CartonNo = base64CartonNo,
                CartonNo = cartonNo
            });
            report.RegisterData(t, "DataInit");
            report.Prepare();

            MemoryStream ms = new MemoryStream();
            ImageExport img = new ImageExport();

            img.ImageFormat = ImageExportFormat.Metafile;
            img.SeparateFiles = false;
            //img.ResolutionX = 96;
            //img.ResolutionY = 96;
            //report.Export(img, "777.png");
            //report.Export(img, ms);
            img.Export(report, ms);

            ms.Flush();

            m_streams.Add(ms);

            foreach (Stream stream in m_streams)
            {
                stream.Position = 0;
            }

            m_currentPageIndex = 0;

            Print(printerName);
            Dispose();
        }

        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\" + name + "." + fileNameExtension, FileMode.Create);//为文件名加上时间
            m_streams.Add(stream);
            return stream;
        }

        private void Print(string printerName)
        {
            if (m_streams == null || m_streams.Count == 0)
                return;
            PrintDocument printDoc = new PrintDocument();
            if (printerName.Length > 0)
            {
                try
                {
                    printDoc.PrinterSettings.PrinterName = printerName;
                }
                catch (Exception ex)
                {
                    this.Logger.Info($"箱码打印失败:{ex}");
                }
            }

            if (!printDoc.PrinterSettings.IsValid)
            {
                string msg = string.Format("Can't find printer {0}", printerName);
                System.Diagnostics.Debug.WriteLine(msg);
                return;
            }

            foreach (PaperSize ps in printDoc.PrinterSettings.PaperSizes)
            {
                if (ps.PaperName == "User defined")
                {
                    printDoc.PrinterSettings.DefaultPageSettings.PaperSize = ps;
                    printDoc.DefaultPageSettings.PaperSize = ps;
                    // printDoc.PrinterSettings.IsDefaultPrinter;//知道是否是预设定的打印机
                }
                else if (ps.PaperName == "A4")
                {
                    printDoc.PrinterSettings.DefaultPageSettings.PaperSize = ps;
                    printDoc.DefaultPageSettings.PaperSize = ps;
                }
            }
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            printDoc.PrintController = new System.Drawing.Printing.StandardPrintController();
            printDoc.Print();
        }

        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            try
            {
                Metafile pageImage = new Metafile(m_streams[m_currentPageIndex] as Stream);
                //设置高质量插值法
                ev.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                //设置高质量,低速度呈现平滑程度
                ev.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //在指定位置并且按指定大小绘制原图片的指定部分 ev.Graphics.DrawImage(原始图片, new System.Drawing.Rectangle(0, 0, 缩略图片的宽, 缩略图片的高), new System.Drawing.Rectangle(0, 0, 原始图片的宽, 原始图片的高), System.Drawing.GraphicsUnit.Pixel);
                //ev.Graphics.DrawImage(pageImage, new System.Drawing.Rectangle(0, 0, pageImage.Width, pageImage.Height), new System.Drawing.Rectangle(0, 0, pageImage.Width, pageImage.Height), System.Drawing.GraphicsUnit.Pixel);

                ev.Graphics.DrawImage(pageImage, new System.Drawing.Point(0, 5));

                // ev.Graphics.DrawImage(pageImage, 0, 0, 650, 400);//設置打印尺寸 单位是像素
                m_currentPageIndex++;
                ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
            }
            catch (Exception ex)
            {
                var ss = ex;
                throw new Exception(ex.ToString());
            }
        }

        public void Dispose()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }
    }

    public class PrintModel
    {
        public string Base64CartonNo { get; set; }

        public string CartonNo { get; set; }
    }
}