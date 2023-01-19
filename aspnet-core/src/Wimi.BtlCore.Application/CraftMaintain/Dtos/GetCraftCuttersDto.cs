using System.Collections.Generic;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    public class GetCraftCuttersDto
    {
        public List<CraftPathCutterDto> CraftPathCutters { get; set; }= new List<CraftPathCutterDto>();
    }
}
