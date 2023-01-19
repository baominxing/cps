using Newtonsoft.Json;
using System.Collections.Generic;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    public class CraftPathMapData
    {
        public string Name { get; set; }

        public List<CraftPathMapDataProcesse> Children { get; set; }
        = new List<CraftPathMapDataProcesse>();
    }

    public class CraftPathMapDataProcesse
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CraftPathMapDataTong> Children { get; set; }
        = new List<CraftPathMapDataTong>();
    }

    public class CraftPathMapDataTong
    { 
        public string Name { get; set; }
        public List<CraftPathMapDataProgram> Children { get; set; }
        = new List<CraftPathMapDataProgram>();

    }

    public class CraftPathMapDataProgram
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string ProcedureNumber { get; set; }
        public List<CraftPathMapDataCutter> Children { get; set; }
        = new List<CraftPathMapDataCutter>();
    }

    public class CraftPathMapDataCutter
    {
        public CraftPathMapDataCutter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
