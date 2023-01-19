using Abp;
using System.Collections.Generic;
using Wimi.BtlCore.Cutter.Dto;
using Wimi.BtlCore.Web.Models.Common;
namespace Wimi.BtlCore.Web.Models.Cutter.CutterState
{
    public class CutterStateViewModel : ICutterTypeViewModal
    {
        public CutterStateViewModel()
        {
            this.CutterTypes = new List<CutterTypeDto>();
            this.LifeStates = new List<NameValue<int>>();
            this.UsedStates = new List<NameValue<int>>();
        }

        public List<CutterTypeDto> CutterTypes { get; set; }

        public List<NameValue<int>> LifeStates { get; set; }

        public List<NameValue<int>> UsedStates { get; set; }

        public bool CutterLifeIsByCount { get; set; }
    }
}
