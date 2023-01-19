namespace Wimi.BtlCore.BasicData.Dto
{
    using Abp.AutoMapper;
    using Wimi.BtlCore.BasicData.MachineTypes;

    [AutoMap(typeof(MachineType))]
    public class MachineTypeDto
    {
        public string Desc { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}