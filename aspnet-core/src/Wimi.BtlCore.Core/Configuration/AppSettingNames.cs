using System;
using System.Configuration;
using Wimi.BtlCore.Authorization;

namespace Wimi.BtlCore.Configuration
{
    public static class AppSettingNames
    {
        public const string UiTheme = "App.UiTheme";

        public static Menu DevicesMonitoringStates => new Menu { Name = "DevicesMonitoring.States" };

        public static Menu DevicesRealtimeAlarms => new Menu { Name = "DevicesMonitoring.DevicesRealtimeAlarms" };

        public static Menu VisualView => new Menu { Name = "Visual.View" };
    }
}
