using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore
{
    public interface IBtlFolders
    {
        string ConfigurationsFloder { get; set; }

        string IcosFolder { get; }

        string SampleProfileImagesFolder { get; }

        string TempFileDownloadFolder { get; }

        string TempFileUploadFolder { get; }

        string VisualImgFloder { get; set; }

        string WebLogsFolder { get; set; }

        string ComponentConfigFolder { get; set; }
    }
}
