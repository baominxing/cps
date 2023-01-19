namespace Wimi.BtlCore.Web.Startup
{
    public interface ITenancyNameFinder
    {
        string GetCurrentTenancyNameOrNull();
    }
}
