namespace Wimi.BtlCore.Cutter.Dto
{
    using System.Collections.Generic;

    using Abp.Application.Services.Dto;

    using Castle.Components.DictionaryAdapter;
    using Wimi.BtlCore.RealtimeIndicators.Parameters.Dto;

    public class GetCutterStatesColumnsDto
    {
        public GetCutterStatesColumnsDto()
        {
            this.ParameterMap = new EditableList<NameValueDto>();
        }

        public List<DataTablesColumns> DataTablesColumns { get; set; }

        /// <summary>
        /// 刀具参数匹配关系
        /// </summary>
        public List<NameValueDto> ParameterMap { get; set; }
    }
}