using Abp.Application.Services.Dto;
using Castle.Components.DictionaryAdapter;
using System;
using System.Collections.Generic;
using Wimi.BtlCore.Cutter;

namespace Wimi.BtlCore.Web.Models.Cutter.CutterState
{
    public class CutterStateCreateOrUpdateViewModel
    {
        public CutterStateCreateOrUpdateViewModel()
        {
            this.ParameterMap = new EditableList<NameValueDto>();
        }

        public EnumCountingMethod CountingMethod { get; set; }

        public EnumCutterLifeStates CutterLifeStatus { get; set; }

        public CutterStates CutterState { get; set; }

        public EnumCutterUsedStates CutterUsedStatus { get; set; }

        public int? Id { get; set; }

        public bool IsEditModal { get; set; }

        public List<NameValueDto> ParameterMap { get; set; }

        public DateTime CreationTime { get; set; }

        public long? CreatorUserId { get; set; }
    }
}
