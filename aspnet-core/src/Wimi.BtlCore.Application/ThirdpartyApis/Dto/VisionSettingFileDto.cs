namespace Wimi.BtlCore.ThirdpartyApis.Dto
{
    using Newtonsoft.Json;

    public class VisionSettingFileDto
    {
        public string FileName => this.WorkShopCode;

        [JsonIgnore]
        public string WorkShopCode { get; set; }

        public string FullPath { get; set; }

        [JsonIgnore]
        public string Content { get; set; }

        public string Name { get; set; }
    }
}