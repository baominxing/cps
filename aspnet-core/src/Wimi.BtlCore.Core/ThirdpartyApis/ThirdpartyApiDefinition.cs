namespace Wimi.BtlCore.ThirdpartyApis
{
    using Abp.AutoMapper;

    [AutoMap(typeof(ThirdpartyApi))]
    public class ThirdpartyApiDefinition
    {
        public ThirdpartyApiDefinition(string url, string code, string name)
        {
            this.Url = url;
            this.Code = code;
            this.Name = name;
            this.Type = EnumApiType.VisualComponent;
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public EnumApiType Type { get; set; }
    }
}