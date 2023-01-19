namespace Wimi.BtlCore.StatisticAnalysis.OEE.Dto
{
    using Abp.AutoMapper;
    using Wimi.BtlCore.OEE;

    [AutoMap(typeof(UnplannedPause))]
    public class UnplannedPauseDto
    {
        public decimal Duration { get; set; }

        public string StateCode { get; set; }

        public string Name { get; set; }

        public string Hexcode { get; set; }
    }
}