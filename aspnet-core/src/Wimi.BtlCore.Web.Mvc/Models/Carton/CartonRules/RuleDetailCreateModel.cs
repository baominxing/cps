using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Wimi.BtlCore.Web.Models.Carton.CartonRules
{
    public class RuleDetailCreateModel
    {
        public int RuleId { get; set; }

        public IEnumerable<NameValueDto<int>> TypeSelect { get; set; }

        public IEnumerable<NameValueDto<int>> FirstDeviceGroups { get; set; }

        public IEnumerable<NameValueDto<int>> ShiftSolutions { get; set; }
    }
}
