namespace Wimi.BtlCore.Visual.Dto
{
    using Newtonsoft.Json;

    public class StateRatioDto
    {
        public decimal StopDuration { get; set; }

        public decimal RunDuration { get; set; }

        public decimal FreeDuration { get; set; }

        public decimal Offlinetion { get; set; }

        public string MachineName { get; set; }

        [JsonIgnore]
        public string Code { get; set; }

        [JsonIgnore]
        public decimal Duration { get; set; }
    }
}