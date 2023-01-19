using System.IO;
using System.Linq;

using Abp.Authorization;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.AppSystem.IO;
using Wimi.BtlCore.AppSystem.Logging.Dto;
using Wimi.BtlCore.AppSystem.Net.MimeTypes;
using Microsoft.AspNetCore.Mvc;

namespace Wimi.BtlCore.AppSystem.Logging
{
    [AbpAuthorize(PermissionNames.Pages_Administration_Host_Maintenance)]
    public class WebLogAppService : BtlCoreAppServiceBase, IWebLogAppService
    {
        private readonly IBtlFolders appFolders;

        public WebLogAppService(IBtlFolders appFolders)
        {
            this.appFolders = appFolders;
        }

        public FileDto DownloadWebLogs()
        {
            var zipFileDto = new FileDto("WebSiteLogs.zip", MimeTypeNames.ApplicationZip);
            var outputZipFilePath = Path.Combine(this.appFolders.TempFileDownloadFolder, zipFileDto.FileToken);          

            using (var outputZipFileStream = File.Create(outputZipFilePath))
            {
                using (var zipStream = new ZipOutputStream(outputZipFileStream))
                {
                    var directory = new DirectoryInfo(this.appFolders.WebLogsFolder);
                    var logFiles = directory.GetFiles("*.txt", SearchOption.AllDirectories).ToList();

                    foreach (var logFile in logFiles)
                    {
                        var logFileInfo = new FileInfo(logFile.FullName);
                        var logZipEntry = new ZipEntry(logFile.Name)
                                              {
                                                  DateTime = logFileInfo.LastWriteTime, 
                                                  Size = logFileInfo.Length
                                              };

                        zipStream.PutNextEntry(logZipEntry);

                        using (
                            var fs = new FileStream(
                                logFile.FullName, 
                                FileMode.Open, 
                                FileAccess.Read, 
                                FileShare.ReadWrite, 
                                0x1000, 
                                FileOptions.SequentialScan))
                        {
                            StreamUtils.Copy(fs, zipStream, new byte[4096]);
                        }

                        zipStream.CloseEntry();
                    }

                    // Makes the Close also Close the underlying stream
                    zipStream.IsStreamOwner = true;
                }
            }

            return zipFileDto;
        }

        [HttpPost]
        public GetLatestWebLogsOutputDto GetLatestWebLogs()
        {
            var directory = new DirectoryInfo(this.appFolders.WebLogsFolder);
            var lastLogFile =
                directory.GetFiles("*.txt", SearchOption.AllDirectories)
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault();

            if (lastLogFile == null)
            {
                return new GetLatestWebLogsOutputDto();
            }

            var lines = AppFileHelper.ReadLines(lastLogFile.FullName).Reverse().Take(1000).Reverse().ToList();
            var logLineCount = 0;
            var lineCount = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("DEBUG") || line.StartsWith("INFO") || line.StartsWith("WARN")
                    || line.StartsWith("ERROR") || line.StartsWith("FATAL"))
                {
                    logLineCount++;
                }

                lineCount++;

                if (logLineCount == 100)
                {
                    break;
                }
            }

            return new GetLatestWebLogsOutputDto { LatesWebLogLines = lines.Take(lineCount).ToList() };
        }
    }
}