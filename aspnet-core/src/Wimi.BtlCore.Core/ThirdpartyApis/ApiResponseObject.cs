namespace Wimi.BtlCore.ThirdpartyApis
{
    using System.Collections.Generic;

    public class ApiResponseObject
    {
        public ApiResponseObject(string itemType, string targetType = ApiTargetType.One)
        {
            this.Target = new List<string>();
            this.FixedSeries = false;
            this.ItemType = itemType;
            this.TargetType = targetType;
            this.Legend = new List<string>();
            this.Series = new List<IEnumerable<dynamic>>();
            this.Data = new List<IEnumerable<dynamic>>();
        }

        public List<string> Target { get; set; }

        public List<string> Legend { get; set; }

        public string ItemType { get; set; }

        public string TargetType { get; set; }

        public bool FixedSeries { get; set; }

        public List<IEnumerable<dynamic>> Series { get; set; }

        public List<IEnumerable<dynamic>> Data { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}