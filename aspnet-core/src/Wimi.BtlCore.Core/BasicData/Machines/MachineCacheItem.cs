namespace Wimi.BtlCore.BasicData.Machines
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class MachineCacheItem
    {
        public const string CacheStoreName = "MachineCacheItem";
        public const string CacheAllStoreName = "MachineAllCacheItem";

        static MachineCacheItem()
        {
            CacheExpireTime = TimeSpan.FromMinutes(120);
        }

        public MachineCacheItem()
        {
            this.CatchedMachines = new HashSet<FlatMachineDto>();
        }

        public MachineCacheItem(int machineId)
            : this()
        {
            this.MachineId = machineId;
        }

        public static TimeSpan CacheExpireTime { get; private set; }

        public HashSet<FlatMachineDto> CatchedMachines { get; set; }

        public int MachineId { get; set; }
    }
}