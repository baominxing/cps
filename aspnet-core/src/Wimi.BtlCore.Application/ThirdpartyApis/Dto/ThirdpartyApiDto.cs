namespace Wimi.BtlCore.ThirdpartyApis.Dto
{
    using System;

    using Abp.AutoMapper;

    using Newtonsoft.Json;


    [AutoMap(typeof(ThirdpartyApi))]
    public class ThirdpartyApiDto
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
    }
}