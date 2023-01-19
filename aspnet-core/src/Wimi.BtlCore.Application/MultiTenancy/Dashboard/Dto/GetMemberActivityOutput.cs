namespace Wimi.BtlCore.MultiTenancy.Dashboard.Dto
{
    using System.Collections.Generic;

    public class GetMemberActivityOutputDto
    {
        public List<int> NewMembers { get; set; }

        public List<int> TotalMembers { get; set; }
    }
}