using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Castle.Components.DictionaryAdapter;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    [AutoMap(typeof(FmsCutters.FmsCutter))]
    public class FmsCutterDto
    {
        public FmsCutterDto()
        {
            this.ExtendFields = new EditableList<NameValueDto<int>>();
        }

        public int Id { get; set; }

        public int? MachineId { get; set; }

        public string MachineName { get; set; }

        public string CutterStockNo { get; set; }

        public string CutterNo { get; set; }

        public string CutterCase { get; set; }

        public string Type { get; set; }

        public decimal Length { get; set; }

        public decimal Diameter { get; set; }

        public string CompensateNo { get; set; }

        public decimal LengthCompensate { get; set; }

        public decimal DiameterCompensate { get; set; }

        public decimal OriginalLife { get; set; }

        public decimal CurrentLife { get; set; }

        public decimal WarningLife { get; set; }

        public EnumFmsUseType UseType { get; set; }

        public string UseTypeName => this.UseType.ToString();

        public FmsCutters.EnumFmsCutterCountType CountType { get; set; }

        public string CountTypeName => this.CountType.ToString();

        public EnumFmsCutterState State { get; set; }

        public string StateName => this.State.ToString();

        /// <summary>
        /// 输入值
        /// </summary>
        public List<NameValueDto<int>>  ExtendFields { get; set; }

        public IEnumerable<FmsCutterExtendDto> CustomFileds { get; set; }
    }
}

