using Abp.AutoMapper;
using Abp.Organizations;

namespace Wimi.BtlCore.Web.Models.App
{
    [AutoMapFrom(typeof(OrganizationUnit))]
    public class EditOrganizationUnitModalViewModel
    {
        public string DisplayName { get; set; }

        public long? Id { get; set; }
    }
}
