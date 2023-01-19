using System.Collections.Generic;
using MongoDB.Bson;

namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    public class ParamComparisonOutput
    {
        public IEnumerable<string> CreationTimes { get; set; }

        public List<ParamComparisonItem> Items { get; set; }
         = new List<ParamComparisonItem>();
    }

    public class ParamComparisonItem
    {
        public ParamComparisonItem(string name, IEnumerable<BsonValue> paramValues)
        {
            Name = name;
            ParamValues = paramValues;
        }
        public string Name { get; set; }

        public IEnumerable<BsonValue> ParamValues { get; set; }
    }
}