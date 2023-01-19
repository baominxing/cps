namespace Wimi.BtlCore.Configuration.Tenants.Dto
{
    public class LdapSettingsEditDto
    {
        public string Domain { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsModuleEnabled { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}