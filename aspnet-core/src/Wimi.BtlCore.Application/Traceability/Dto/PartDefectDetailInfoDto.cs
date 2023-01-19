using System;

namespace Wimi.BtlCore.Traceability.Dto
{
    public class PartDefectDetailInfoDto
    {
        public string DefectivePartName { get; set; }

        public string DefectiveReasonName { get; set; }

        public DateTime CreationTime { get; set; }

        public string CreatorUserName { get; set; }
    }
}
