namespace Wimi.BtlCore.Cutter.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.AutoMapper;

    [AutoMapFrom(typeof(CutterModel))]
    public class GetCutterModelDefaultDto
    {
        public int CountingMethod { get; set; }

        public string CutterNo { get; set; }

        [MaxLength(50)]
        public string CutterNoPrefix { get; set; }

        public int CutterTypeId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public int OriginalLife { get; set; }

        public string Parameter1 { get; set; }

        public string Parameter10 { get; set; }

        public string Parameter2 { get; set; }

        public string Parameter3 { get; set; }

        public string Parameter4 { get; set; }

        public string Parameter5 { get; set; }

        public string Parameter6 { get; set; }

        public string Parameter7 { get; set; }

        public string Parameter8 { get; set; }

        public string Parameter9 { get; set; }

        public int WarningLife { get; set; }
    }
}