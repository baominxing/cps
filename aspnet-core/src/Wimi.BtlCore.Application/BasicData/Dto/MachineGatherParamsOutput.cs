namespace Wimi.BtlCore.BasicData.Dto
{
    using Abp.AutoMapper;
    using Wimi.BtlCore.BasicData.Machines;

    [AutoMapFrom(typeof(MachineGatherParam))]
    public class MachineGatherParamsOutputDto : MachineGatherParamDto
    {
        public bool IsInMongo { get; set; }

        public string NameInHost { get; set; }
    }
}