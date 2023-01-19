namespace Wimi.BtlCore.BasicData.DeviceGroups
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class DeviceGroupCacheItem
    {
        public const string CacheStoreName = "DeviceGroupCacheItem";

        static DeviceGroupCacheItem()
        {
            CacheExpireTime = TimeSpan.FromMinutes(120);
        }

        public DeviceGroupCacheItem()
        {
            this.CatchedDeviceGroups = new HashSet<FlatDeviceGroupDto>();
        }

        public static TimeSpan CacheExpireTime { get; private set; }

        public HashSet<FlatDeviceGroupDto> CatchedDeviceGroups { get; set; }
    }
}