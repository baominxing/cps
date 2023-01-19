namespace Wimi.BtlCore.Parameters.Dto
{
    /// <summary>
    /// 实时参数
    /// </summary>
    public class ParamsItem
    {
        public string Hexcode { get; set; }

        public double Max { get; set; }

        public double Min { get; set; }

        public string Name { get; set; }

        public int Seq { get; set; }

        public int Type { get; set; }

        public string Value { get; set; }
    }
}
