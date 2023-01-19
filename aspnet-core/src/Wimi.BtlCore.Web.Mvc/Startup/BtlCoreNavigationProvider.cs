using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Localization;
using Wimi.BtlCore.Authorization;

namespace Wimi.BtlCore.Web.Startup
{
    /// <summary>
    /// This class defines menus for the application.
    /// </summary>
    public class BtlCoreNavigationProvider : NavigationProvider
    {
        public const string MenuName = "Mpa";

        public override void SetNavigation(INavigationProviderContext context)
        {
            var menu =
              context.Manager.Menus[MenuName] = new MenuDefinition(MenuName, new FixedLocalizableString("Main Menu"));

            menu.AddItem(
                new MenuItemDefinition(
                    PageNames.App.Host.Tenants,
                    L("Tenants"),
                    url: "Tenants",
                    icon: "fa fa-user-secret",
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Tenants)));

            menu.AddItem(
                new MenuItemDefinition(
                    PageNames.App.Tenant.Dashboard,
                    L("Dashboard"),
                    url: "Dashboard/Index",
                    icon: "fa fa-home",
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Tenant_Dashboard)));

            #region Administration

            menu.AddItem(
                new MenuItemDefinition(PageNames.App.Common.Administration, L("Administration"), "fa fa-hourglass")
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Common.Roles,
                            L("Roles"),
                            url: "Roles",
                            icon: "fa fa-graduation-cap",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Administration_Roles)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Common.Users,
                            L("Users"),
                            url: "Users",
                            icon: "fa fa-user",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Administration_Users)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Host.Editions,
                            L("Editions"),
                            url: "Editions",
                            icon: "fa fa-th-large",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Editions)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Common.OrganizationUnits,
                            L("OrganizationUnits"),
                            url: "OrganizationUnits",
                            icon: "fa fa-users",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Administration_OrganizationUnits)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Common.Languages,
                            L("Languages"),
                            url: "Languages",
                            icon: "fa fa-flag",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Administration_Languages)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Common.AuditLogs,
                            L("AuditLogs"),
                            url: "AuditLogs",
                            icon: "fa fa-lock",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Administration_AuditLogs)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Host.Maintenance,
                            L("Maintenance"),
                            url: "Maintenance",
                            icon: "fa fa-wrench",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Administration_Host_Maintenance)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Host.Settings,
                            L("Settings"),
                            url: "HostSettings",
                            icon: "fa fa-cog",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Administration_Host_Settings)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.BasicData_MachineShiftSetting,
                            L("MachineShiftSetting"),
                            url: "MachineShiftSetting",
                            icon: "fa fa-address-card-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Administration_MachineShiftSetting)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Administration_DmpMachineSetting,
                            L("DmpMachineSetting"),
                            url: "DmpMachineSetting",
                            icon: "fa fa-window-restore",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Administration_DmpMachineSetting)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Tenant.Settings,
                            L("Settings"),
                            url: "Settings",
                            icon: "fa fa-cog",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Administration_Tenant_Settings)))

                            //.AddItem(
                            //    new MenuItemDefinition(
                            //        PageNames.App.Common.Notification,
                            //        L("Notifications"),
                            //        url: "WeChatNotifications",
                            //        icon: "fa fa-comments-o",
                            //        permissionDependency: PermissionNames.Pages_Administration_WeChatNotifications))
                            //.AddItem(
                            // new MenuItemDefinition(
                            //     PageNames.App.Page.BasicData_Maintain,
                            //     L("Maintain"),
                            //     url: "Maintain",
                            //     icon: "fa fa-upload",
                            //     permissionDependency: PermissionNames.Pages_Administration_Maintain))

                            );

            #endregion

            #region BasicData

            menu.AddItem(
                new MenuItemDefinition(
                    PageNames.App.Page.BasicData,
                    L("BasicData"),
                    url: "BasicData",
                    icon: "fa fa-files-o",
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_BasicData)).AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.BasicData_DeviceGroup,
                            L("DeviceGroups"),
                            url: "DeviceGroups",
                            icon: "fa fa-object-group",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_BasicData_DeviceGroups)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.BasicData_MachineType,
                            L("MachineType"),
                            url: "MachineType",
                            icon: "fa fa-tags",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_BasicData_MachineType)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.BasicData_MachineSetting,
                            L("MachineSetting"),
                            url: "MachineSetting",
                            icon: "fa fa-desktop",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_BasicData_MachineSetting)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.BasicData_GatherParamsSetting,
                            L("GatherParamsSetting"),
                            url: "GatherParamsSetting",
                            icon: "fa fa-tachometer ",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_BasicData_GatherParamsSetting)))
                    //.AddItem(
                    //    new MenuItemDefinition(
                    //        PageNames.App.Page.BasicData_ShiftSetting,
                    //        L("ShiftSetting"),
                    //        url: "ShiftSetting",
                    //        icon: "fa fa-list-ul",
                    //        permissionDependency: PermissionNames.Pages_BasicData_ShiftSetting))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.BasicData_StateInfo,
                            L("StateInfo"),
                            url: "StateInfo",
                            icon: "fa fa-eraser",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_BasicData_StateInfo)))

                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.BasicData_AlarmInfo,
                            L("AlarmInfo"),
                            url: "AlarmInfo",
                            icon: "fa fa-exclamation-triangle",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_BasicData_AlarmInfo)))
                    //.AddItem(
                    //    new MenuItemDefinition(
                    //        PageNames.App.Page.BasicData_ShiftTargetYiled,
                    //        L("ShiftTargetYiled"),
                    //        url: "ShiftTargetYiled",
                    //        icon: "fa fa-calendar-check-o",
                    //        permissionDependency: PermissionNames.Pages_BasicData_ShiftTargetYiled))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.BasicData_ImportData,
                            L("ImportData"),
                            url: "ImportData",
                            icon: "fa fa-upload",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_BasicData_ImportData)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Order_Product,
                            L("Product"),
                            url: "Product",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order_Product)))
                            //.AddItem(
                            //     new MenuItemDefinition(
                            //         "vue集成",
                            //         L("vue集成"),
                            //         url: "/app#/login",
                            //         icon: "fa fa-upload",
                            //         customData: "true",
                            //         permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_BasicData_ImportData)))
                            );

            #endregion

            #region DevicesMonitoring

            menu.AddItem(
                new MenuItemDefinition(
                    PageNames.App.Page.DevicesMonitoring,
                    L("DevicesMonitoring"),
                    url: "DevicesMonitoring",
                    icon: "fa fa-trello",
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_DevicesMonitoring)).AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.DevicesMonitoring_States,
                            L("DevicesMonitoring_States"),
                            url: "MachineStates",
                            icon: "fa fa-circle-o",
                            customData: new { IsDefaultShortcut = true },
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_DevicesMonitoring_States)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.DevicesMonitoring_Parameters,
                            L("DevicesMonitoring_Parameters"),
                            url: "MachineParameters",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_DevicesMonitoring_Parameters)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.DevicesMonitoring_DevicesRealtimeAlarms,
                            L("DevicesAlarms"),
                            url: "MachineRealtimeAlarms",
                            icon: "fa fa-circle-o",
                            customData: new { IsDefaultShortcut = true },
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_DevicesRealtimeAlarms))));

            #endregion

            #region Statistic

            menu.AddItem(
                new MenuItemDefinition(
                        PageNames.App.Page.StatisticAnalysis,
                        L("StatisticAnalysis"),
                        url: "StatisticAnalysis",
                        icon: "fa fa-percent",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_StatisticAnalysis))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StatisticAnalysis_AlarmStatistics,
                            L("AlarmStatistics"),
                            url: "AlarmStatistics",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_AlarmStatistics)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StatisticAnalysis_HistoryParameters,
                            L("HistoryParameters"),
                            url: "HistoryParameters",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_HistoryParameters)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StatisticAnalysis_EfficiencyTrends,
                            L("EfficiencyTrends"),
                            url: "EfficiencyTrends",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_EfficiencyTrends)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StatisticAnalysis_TimeStatistics,
                            L("TimeStatistics"),
                            url: "TimeStatistics",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_TimeStatistics)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StatisticAnalysis_YieldStatistics,
                            L("YieldStatistics"),
                            url: "YieldStatistics",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_YieldStatistics)))
                     .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StatisticAnalysis_QualifyStatistics,
                            L("QualifyStatistics"),
                            url: "QualifyStatistics",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_QualifyStatistics)))
                    //.AddItem(
                    //    new MenuItemDefinition(
                    //        PageNames.App.Page.StatisticAnalysis_YieldAnalysisStatistics,
                    //        L("ComprehensiveStatistics"),
                    //        url: "YieldAnalysisStatistics",
                    //        icon: "fa fa-circle-o",
                    //        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_YieldAnalysisStatistics)))
                    //.AddItem(
                    //    new MenuItemDefinition(
                    //        PageNames.App.Page.StatisticAnalysis_OEE,
                    //        L("OEE"),
                    //        url: "OEE",
                    //        icon: "fa fa-circle-o",
                    //        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_OEE)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StatisticAnalysis_StatusDistributionMap,
                            L("StatusDistributionMap"),
                            url: "StatusDistributionMap",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_StatusDistributionMap)))
                );

            #endregion

            #region Order

            menu.AddItem(
                new MenuItemDefinition(
                    PageNames.App.Page.Order,
                    L("Order"),
                    url: "Order",
                    icon: "fa fa-barcode",
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Order_Process,
                            L("Process"),
                            url: "Process",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order_Process)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Order_StandardTime,
                            L("StandardTime"),
                            url: "StandardTime",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order_StandardTime)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Order_Craft,
                            L("Craft"),
                            url: "Craft",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order_Craft)))

                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Order_MachineProcess,
                            L("MachineProcess"),
                            url: "MachineProcess",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order_MachineProcess)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Order_ProductionPlan,
                            L("ProductionPlan"),
                            url: "ProductionPlan",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order_ProductionPlan)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Order_LoginReport,
                            L("LoginReport"),
                            url: "LoginReport",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order_LoginReport)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Order_DefectiveStatistics,
                            L("DefectiveStatistics"),
                            url: "DefectiveStatistics",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order_DefectiveStatistics)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Order_MachineReport,
                            L("MachineReport"),
                            url: "MachineReport",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order_MachineReport))));

            #endregion

            #region ProducePlan
            menu.AddItem(
            new MenuItemDefinition(
                PageNames.App.Page.ProducePlan_Plans,
                L("Plan"),
                url: "Plans",
                icon: "fa fa-calendar-check-o",
                permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Plan_Manage)));


            //menu.AddItem(
            //new MenuItemDefinition(
            //    PageNames.App.Page.ProducePlan,
            //    L("PlanModule"),
            //    url: "Plan",
            //    icon: "fa fa-circle-o",
            //    permissionDependency: PermissionNames.Pages_Plan)
            //        .AddItem(
            //            new MenuItemDefinition(
            //                PageNames.App.Page.ProducePlan_Plans,
            //                L("Plan"),
            //                url: "Plans",
            //                icon: "fa fa-calendar-check-o",
            //                permissionDependency: PermissionNames.Pages_Plan_Manage)));
            #endregion 

            #region MemberPerfomance

            menu.AddItem(
                new MenuItemDefinition(
                    PageNames.App.Page.StaffPerformance,
                    L("StaffPerformance"),
                    url: "StaffPerformance",
                    icon: "fa fa-line-chart",
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_StaffPerformance)).AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StaffPerformance_OnlineOrOffline,
                            L("StaffPerformance_OnlineOrOffline"),
                            url: "StaffPerformance/OnlineOrOffline",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_StaffPerformance_OnlineOrOffline)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StaffPerformance_OnlineOrOfflineRecord,
                            L("StaffPerformance_OnlineOrOfflineRecord"),
                            url: "StaffPerformance/OnlineOrOfflineRecord",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_StaffPerformance_OnlineOrOfflineRecord)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StaffPerformance_StaffPerformance,
                            L("StaffPerformance_StaffPerformance"),
                            url: "StaffPerformance/StaffPerformance",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_StaffPerformance_StaffPerformance)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.StaffPerformance_StaffYield,
                            L("StaffPerformance_StaffYield"),
                            url: "StaffPerformance/StaffYield",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_StaffPerformance_StaffYield))));

            #endregion

            #region Visual

            menu.AddItem(
                new MenuItemDefinition(
                    PageNames.App.Page.Visual,
                    L("Visual"),
                    url: "Visual",
                    icon: "fa fa-bar-chart",
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Visual)).AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Visual_View,
                            L("VisualView"),
                            url: "vision/",
                            icon: "fa fa-area-chart",
                            customData: new { IsDefaultShortcut = true },
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_VisualView)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Visual_Notice,
                            L("VisualNotice"),
                            url: "VisualView/VisualNotice",
                            icon: "fa fa-newspaper-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_VisualNotice))));

            #endregion

            #region Cutter

            menu.AddItem(
                new MenuItemDefinition(
                    PageNames.App.Page.Cutter,
                    L("Cutter"),
                    url: "Cutter",
                    icon: "fa fa-deviantart",
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Cutter)).AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Cutter_CutterParameter,
                            L("CutterParameter"),
                            url: "CutterParameter",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Cutter_CutterParameter)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Cutter_CutterType,
                            L("CutterType"),
                            url: "CutterType",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Cutter_CutterType)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Cutter_CutterState,
                            L("CutterState"),
                            url: "CutterState",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Cutter_CutterState)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Cutter_CutterLoadAndUnloadRecord,
                            L("CutterLoadAndUnloadRecord"),
                            url: "CutterLoadAndUnloadRecord",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Cutter_CutterLoadAndUnloadRecord))));

            #endregion

            #region Traceability

            menu.AddItem(
                new MenuItemDefinition(
                    PageNames.App.Page.Traceability,
                    L("Traceability"),
                    url: "Traceability",
                    icon: "fa fa-globe",
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Traceability)).AddItem(
                    new MenuItemDefinition(
                        PageNames.App.Page.Traceability_Catalog_Query,
                        L("TraceCatalogQuery"),
                        url: "Traceability",
                        icon: "fa fa-history",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Traceability_CatalogQuery)))

                     .AddItem(
                    new MenuItemDefinition(
                        PageNames.App.Page.Traceability_NgParts,
                        L("TraceNgParts"),
                        url: "Traceability/NgParts",
                        icon: "fa fa-history",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Traceability_NgParts)))
                    .AddItem(
                    new MenuItemDefinition(
                        PageNames.App.Page.Traceability_Settings,
                        L("TraceSetings"),
                        url: "Traceability/Settings",
                        icon: "fa fa-history",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Traceability_Settings)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Order_DefectiveReasons,
                            L("DefectiveReasons"),
                            url: "DefectiveReasons",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Order_DefectiveReasons)))
                        );

            #endregion

            #region Notification

            menu.AddItem(
                new MenuItemDefinition(
                    PageNames.App.Page.Notification,
                    L("Notification"),
                    url: "Notification",
                    icon: "fa fa-commenting",
                    permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Notification)).AddItem(
                    new MenuItemDefinition(
                        PageNames.App.Page.Notification_Rules,
                        L("NotificationRules"),
                        url: "NotificationRules",
                        icon: "fa fa-circle-o",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Notification_Rules))).AddItem(
                    new MenuItemDefinition(
                        PageNames.App.Page.Notification_Records,
                        L("NotificationRecords"),
                        url: "NotificationRecords",
                        icon: "fa fa-circle-o",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Notification_Records))));

            #endregion

            #region ReasonFeedback

            menu.AddItem(
                new MenuItemDefinition(
                        PageNames.App.Page.ReasonFeedback,
                        L("ReasonFeedback"),
                        url: "Feedback",
                        icon: "fa fa-reply",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_ReasonFeedback))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.ReasonFeedback_Calendar,
                            L("FeedbackCalendar"),
                            url: "FeedbackCalendar",
                            icon: "fa fa-calendar-check-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_ReasonFeedback_Calendar)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.ReasonFeedback_Feedback,
                            L("ReasonFeedback"),
                            url: "ReasonFeedback",
                            icon: "fa fa-cogs",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_ReasonFeedback_Feedback)))

                   .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.ReasonFeedback_Analysis,
                            L("ReasonFeedbackAnalysis"),
                            url: "ReasonFeedbackAnalysis",
                            icon: "fa fa-bar-chart",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_ReasonFeedback_Analysis)))
                );

            #endregion

            #region RDLCReport

            menu.AddItem(
               new MenuItemDefinition(
                       PageNames.App.Page.RDLCReport,
                       L("RDLCReport"),
                       url: "Report",
                       icon: "fa fa-rss-square",
                       permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_RDLCReport))
                   .AddItem(
                       new MenuItemDefinition(
                           PageNames.App.Page.RDLCReport_StateConsumeTime,
                           L("StateConsumeTime"),
                           url: "RDLCReport/StateConsumeTimeReport",
                           icon: "fa fa-cogs",
                           permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_RDLCReport_StateConsumeTime)))

                    .AddItem(
                       new MenuItemDefinition(
                           PageNames.App.Page.RDLCReport_MachineUtilizationRate,
                           L("MachineUtilizationRate"),
                           url: "RDLCReport/MachineUtilizationRateReport",
                           icon: "fa fa-cogs",
                           permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_RDLCReport_MachineUtilizationRate)))
                     .AddItem(
                       new MenuItemDefinition(
                           PageNames.App.Page.RDLCReport_PersonYield,
                           L("PersonYield"),
                           url: "RDLCReport/PersonYieldReport",
                           icon: "fa fa-cogs",
                           permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_RDLCReport_PersonYield)))
                     .AddItem(
                       new MenuItemDefinition(
                           PageNames.App.Page.RDLCReport_PersonPerformance,
                           L("PersonPerformance"),
                           url: "RDLCReport/PersonPerformanceReport",
                           icon: "fa fa-cogs",
                           permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_RDLCReport_PersonPerformance)))
                     .AddItem(
                       new MenuItemDefinition(
                           PageNames.App.Page.RDLCReport_Plan,
                           L("ReportOfPlan"),
                           url: "RDLCReport/ReportOfPlan",
                           icon: "fa fa-cogs",
                           permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_RDLCReport_Plan)))

                );

            #endregion

            #region Maintain
            menu.AddItem(
                new MenuItemDefinition(
                        PageNames.App.Page.Maintain,
                        L("Maintain"),
                        url: "Maintain",
                        icon: "fa fa-cubes",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Maintain))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Maintain_Plan,
                            L("MaintainPlan"),
                            url: "MaintainPlan",
                            icon: "fa fa-cogs",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Maintain_Plan)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Maintain_Order,
                            L("MaintainOrder"),
                            url: "MaintainOrder",
                            icon: "fa fa-cogs",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Maintain_Order)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Maintain_Request,
                            L("MaintainRequest"),
                            url: "MaintainRequest",
                            icon: "fa fa-cogs",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Maintain_Request)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Maintain_Repair,
                            L("MaintainRepair"),
                            url: "MaintainRepair",
                            icon: "fa fa-cogs",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Maintain_Repair)))
            );
            #endregion

           // menu.AddItem(
           //new MenuItemDefinition(
           //        PageNames.App.Page.MultiLanguage,
           //       L("Language"),
           //        url: "MultiLanguage",
           //        icon: "fa fa-language",
           //        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_MultiLanguage))

           //        );


            #region Carton

            menu.AddItem(
            new MenuItemDefinition(
                       PageNames.App.Page.Carton,
                       L("Carton"),
                       url: "Carton",
                       icon: "fa fa-inbox",
                       permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Carton))
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.App.Page.Carton_CartonRules,
                        L("CartonRules"),
                        url: "CartonRules",
                        icon: "fa fa-cogs",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Carton_CartonRules)))
            .AddItem(
                       new MenuItemDefinition(
                           PageNames.App.Page.Carton_CartonSetting,
                           L("CartonSetting"),
                           url: "CartonSetting",
                           icon: "fa fa-cogs",
                           permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Carton_CartonSetting)))
            .AddItem(
                       new MenuItemDefinition(
                           PageNames.App.Page.Carton_CartonPrinting,
                           L("CartonPrinting"),
                           url: "CartonPrinting",
                           icon: "fa fa-suitcase",
                           permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Carton_CartonPrinting)))
            .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.Carton_CartonTraceability,
                            L("CartonTraceability"),
                            url: "CartonTraceability",
                            icon: "fa fa-globe",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Carton_CartonTraceability)))
            );

            #endregion

            #region CraftMaintain

            menu.AddItem(
                new MenuItemDefinition(
                        PageNames.App.Page.CraftMaintain,
                        L("CraftMaintain"),
                        url: "CraftMaintain",
                        icon: "fa fa-circle",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_CraftMaintain))
                        .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.CraftMaintain_TongsMaintain,
                            L("TongsMaintain"),
                            url: "TongsMaintain",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_CraftMaintain_TongsMaintain)))

                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.CraftMaintain_FmsCutter,
                            L("FmsCutter"),
                            url: "FmsCutter",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_CraftMaintain_FmsCutter)))
                    .AddItem(
                        new MenuItemDefinition(
                            PageNames.App.Page.CraftMaintain_FlexibleCraftPath,
                            L("FlexibleCraftPath"),
                            url: "FlexibleCraftPath",
                            icon: "fa fa-circle-o",
                            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_CraftMaintain_FlexibleCraftPath)))
            );

            #endregion

            #region Archives
            //menu.AddItem(
            //    new MenuItemDefinition(
            //        PageNames.App.Page.Archives,
            //        L("Archives"),
            //        url: "Archives",
            //        icon: "fa fa-circle",
            //        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Archives))
            //    //如果是在已有模块下新增页面，则拷贝下面代码即可，如果是同时新增一个模块，则拷贝整个region代码块
            //    //======================================================================================
            //    .AddItem(
            //        new MenuItemDefinition(
            //            PageNames.App.Page.Archives_ArchiveEntry,
            //            L("ArchiveEntry"),
            //            url: "ArchiveEntry",
            //            icon: "fa fa-circle-o",
            //            permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Archives_ArchiveEntry)))
            //    //======================================================================================  
            //    );
            #endregion

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, BtlCoreConsts.LocalizationSourceName);
        }
    }
}
