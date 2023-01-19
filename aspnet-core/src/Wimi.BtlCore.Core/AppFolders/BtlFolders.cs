namespace Wimi.BtlCore
{
    using Abp.Dependency;

    public class BtlFolders : IBtlFolders, ISingletonDependency
    {
        public string ConfigurationsFloder { get; set; }

        public string IcosFolder { get; set; }

        public string SampleProfileImagesFolder { get; set; }

        public string TempFileDownloadFolder { get; set; }

        public string TempFileUploadFolder { get; set; }

        public string VisualImgFloder { get; set; }

        public string WebLogsFolder { get; set; }

        public string ComponentConfigFolder { get; set; }
    }
}
