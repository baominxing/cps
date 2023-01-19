using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using Wimi.BtlCore.Cartons;

namespace Wimi.BtlCore.Carton.CartonRules.Dtos
{
    [AutoMap(typeof(CalibratorCode))]
    public class ImportDatasDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 包装规则Id
        /// </summary>
        public int CartonRuleId { get; set; }

        public int Key { get; set; }

        [MaxLength(10)]
        public string Value { get; set; }
    }
}
