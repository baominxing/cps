namespace Wimi.BtlCore.ThirdpartyApis.Dto
{
    using Newtonsoft.Json;

    public class WorkShopDto
    {
        public int Id { get; set; }

        public string Code { get; set; }

        [JsonIgnore]
        public int MachineGroupId { get; set; }

        public string Name { get; set; }
    }
}