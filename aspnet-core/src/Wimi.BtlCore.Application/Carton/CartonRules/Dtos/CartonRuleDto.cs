namespace Wimi.BtlCore.Carton.CartonRules.Dtos
{
    public class CartonRuleDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 规则名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
    }
}
