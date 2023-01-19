namespace Wimi.BtlCore.Configuration.Host.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class GeneralSettingsEditDto
    {
        public string Timezone { get; set; }

        /// <summary>
        ///     This value is only used for comparing user's timezone to default timezone
        /// </summary>
        public string TimezoneForComparison { get; set; }

        [MaxLength(128)]
        public string WebSiteRootAddress { get; set; }
    }
}