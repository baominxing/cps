using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Wimi.BtlCore.Authorization
{
    public class BtlCoreAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(PermissionNames.Pages)
                        ?? context.CreatePermission(PermissionNames.Pages, L("Pages"));

            var administration = pages.CreateChildPermission(PermissionNames.Pages_Administration, L("Administration"));

            var roles = administration.CreateChildPermission(PermissionNames.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(PermissionNames.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(PermissionNames.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(PermissionNames.Pages_Administration_Roles_Delete, L("DeletingRole"));
            roles.CreateChildPermission(PermissionNames.Pages_Administration_Roles_ResetPassword, L("ResetPassword"));

            //var wechatNotification =
            //    administration.CreateChildPermission(
            //        PermissionNames.Pages_Administration_WeChatNotifications,
            //        L("Notifications"));
            //wechatNotification.CreateChildPermission(
            //    PermissionNames.Pages_Administration_WeChatNotifications_ManageNotificationTypes,
            //    L("ManageNotificationTypes"));
            //wechatNotification.CreateChildPermission(
            //    PermissionNames.Pages_Administration_WeChatNotifications_ManageMembers,
            //    L("ManageMembers"));
            //var maintain = administration.CreateChildPermission(PermissionNames.Pages_Administration_Maintain, L("Maintain"));

            var machineDmpSetting = administration.CreateChildPermission(PermissionNames.Pages_Administration_DmpMachineSetting,
                    L("DmpMachineSetting"));
            machineDmpSetting.CreateChildPermission(
                PermissionNames.Pages_Administration_DmpMachineSetting_Manage,
                L("DmpMachineSetting_Manage"));

            var machineShiftSetting =
                administration.CreateChildPermission(
                    PermissionNames.Pages_Administration_MachineShiftSetting,
                    L("PagesBasicDataMachineShiftSetting"));
            machineShiftSetting.CreateChildPermission(
                PermissionNames.Pages_Administration_MachineShiftSetting_Manage,
                L("PagesBasicDataMachineShiftSetting_Manage"));

            var shiftSetting = machineShiftSetting.CreateChildPermission(
                PermissionNames.Pages_BasicData_ShiftSetting, L("PagesBasicDataShiftSetting"));
            shiftSetting.CreateChildPermission(
                PermissionNames.Pages_BasicData_ShiftSetting_Manage,
                L("PagesBasicDataShiftSetting_Manage"));

            var users = administration.CreateChildPermission(PermissionNames.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(
                PermissionNames.Pages_Administration_Users_ChangePermissions,
                L("ChangingPermissions"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Impersonation, L("LoginForUsers"));

            var languages = administration.CreateChildPermission(
                PermissionNames.Pages_Administration_Languages,
                L("Languages"),
                multiTenancySides: MultiTenancySides.Host);
            languages.CreateChildPermission(
                PermissionNames.Pages_Administration_Languages_Create,
                L("CreatingNewLanguage"),
                multiTenancySides: MultiTenancySides.Host);
            languages.CreateChildPermission(
                PermissionNames.Pages_Administration_Languages_Edit,
                L("EditingLanguage"),
                multiTenancySides: MultiTenancySides.Host);
            languages.CreateChildPermission(
                PermissionNames.Pages_Administration_Languages_Delete,
                L("DeletingLanguages"),
                multiTenancySides: MultiTenancySides.Host);
            languages.CreateChildPermission(
                PermissionNames.Pages_Administration_Languages_ChangeTexts,
                L("ChangingTexts"),
                multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(
                PermissionNames.Pages_Administration_AuditLogs,
                L("AuditLogs"),
                multiTenancySides: MultiTenancySides.Host);

            var organizationUnits =
                administration.CreateChildPermission(
                    PermissionNames.Pages_Administration_OrganizationUnits,
                    L("OrganizationUnits"),
                    multiTenancySides: MultiTenancySides.Host);
            organizationUnits.CreateChildPermission(
                PermissionNames.Pages_Administration_OrganizationUnits_ManageOrganizationTree,
                L("ManagingOrganizationTree"),
                multiTenancySides: MultiTenancySides.Host);
            organizationUnits.CreateChildPermission(
                PermissionNames.Pages_Administration_OrganizationUnits_ManageMembers,
                L("ManagingMembers"),
                multiTenancySides: MultiTenancySides.Host);

            #region 客户

            pages.CreateChildPermission(
                PermissionNames.Pages_Tenant_Dashboard,
                L("Dashboard"),
                multiTenancySides: MultiTenancySides.Tenant);
            administration.CreateChildPermission(
                PermissionNames.Pages_Administration_Tenant_Settings,
                L("Settings"),
                multiTenancySides: MultiTenancySides.Tenant);

            #endregion

            #region Host

            var editions = pages.CreateChildPermission(
                PermissionNames.Pages_Editions,
                L("Editions"),
                multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(
                PermissionNames.Pages_Editions_Create,
                L("CreatingNewEdition"),
                multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(
                PermissionNames.Pages_Editions_Edit,
                L("EditingEdition"),
                multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(
                PermissionNames.Pages_Editions_Delete,
                L("DeletingEdition"),
                multiTenancySides: MultiTenancySides.Host);

            var tenants = pages.CreateChildPermission(
                PermissionNames.Pages_Tenants,
                L("Tenants"),
                multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(
                PermissionNames.Pages_Tenants_Create,
                L("CreatingNewTenant"),
                multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(
                PermissionNames.Pages_Tenants_Edit,
                L("EditingTenant"),
                multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(
                PermissionNames.Pages_Tenants_ChangeFeatures,
                L("ChangingFeatures"),
                multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(
                PermissionNames.Pages_Tenants_Delete,
                L("DeletingTenant"),
                multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(
                PermissionNames.Pages_Tenants_Impersonation,
                L("LoginForTenants"),
                multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(
                PermissionNames.Pages_Administration_Host_Settings,
                L("Settings"),
                multiTenancySides: MultiTenancySides.Host);
            administration.CreateChildPermission(
                PermissionNames.Pages_Administration_Host_Maintenance,
                L("Maintenance"),
                multiTenancySides: MultiTenancySides.Host);

            #endregion

            #region 基础数据

            var basicData = pages.CreateChildPermission(
                PermissionNames.Pages_BasicData,
                L("BasicData"),
                multiTenancySides: MultiTenancySides.Tenant);

            var deviceGroup = basicData.CreateChildPermission(
                PermissionNames.Pages_BasicData_DeviceGroups,
                L("DeviceGroups"));
            deviceGroup.CreateChildPermission(
                PermissionNames.Pages_BasicData_DeviceGroups_ManageDeviceTree,
                L("ManageDeviceGroups"),
                multiTenancySides: MultiTenancySides.Tenant);
            deviceGroup.CreateChildPermission(
                PermissionNames.Pages_BasicData_DeviceGroups_ManageMembers,
                L("ManageDeviceMembers"),
                multiTenancySides: MultiTenancySides.Tenant);

            var machineSetting = basicData.CreateChildPermission(
                PermissionNames.Pages_BasicData_MachineSetting,
                L("MachineSetting"));
            machineSetting.CreateChildPermission(
                PermissionNames.Pages_BasicData_MachineSetting_Create,
                L("MachineSetting_Create"));
            machineSetting.CreateChildPermission(
                PermissionNames.Pages_BasicData_MachineSetting_Edit,
                L("MachineSetting_Edit"));
            machineSetting.CreateChildPermission(
                PermissionNames.Pages_BasicData_MachineSetting_Delete,
                L("MachineSetting_Delete"));
            //machineSetting.CreateChildPermission(
            //    PermissionNames.Pages_BasicData_MachineSetting_Deal,
            //    L("MachineSetting_Deal"));
            //machineSetting.CreateChildPermission(
            //    PermissionNames.Pages_BasicData_MachineSetting_Taking,
            //    L("MachineSetting_Taking"),
            //    multiTenancySides: MultiTenancySides.Tenant);

            var gatherParamsSetting =
                basicData.CreateChildPermission(
                    PermissionNames.Pages_BasicData_GatherParamsSetting,
                    L("GatherParamsSetting"));
            gatherParamsSetting.CreateChildPermission(
                PermissionNames.Pages_BasicData_GatherParamsSetting_Manage,
                L("GatherParamsSetting_Manage"));

            //var shiftSetting = basicData.CreateChildPermission(
            //    PermissionNames.Pages_BasicData_ShiftSetting,
            //    L("PagesBasicDataShiftSetting"));
            //shiftSetting.CreateChildPermission(
            //    PermissionNames.Pages_BasicData_ShiftSetting_Manage,
            //    L("PagesBasicDataShiftSetting_Manage"));

            var stateInfo = basicData.CreateChildPermission(PermissionNames.Pages_BasicData_StateInfo, L("StateInfo"));
            stateInfo.CreateChildPermission(PermissionNames.Pages_BasicData_StateInfo_Manage, L("StateInfo_Manage"));

            var importData = basicData.CreateChildPermission(
                PermissionNames.Pages_BasicData_ImportData,
                L("ImportData"));
            importData.CreateChildPermission(PermissionNames.Pages_BasicData_ImportData_Manage, L("ImportData_Manage"));

            var machineType = basicData.CreateChildPermission(
                PermissionNames.Pages_BasicData_MachineType,
                L("MachineType"));
            machineType.CreateChildPermission(
                PermissionNames.Pages_BasicData_MachineType_Manage,
                L("MachineType_Manage"));

            //var shiftTargetYiled = basicData.CreateChildPermission(
            //    PermissionNames.Pages_BasicData_ShiftTargetYiled,
            //    L("ShiftTargetYiled"));
            //shiftTargetYiled.CreateChildPermission(
            //    PermissionNames.Pages_BasicData_ShiftTargetYiled_Manage,
            //    L("ShiftTargetYiled_Manage"));

            var alarmInfo = basicData.CreateChildPermission(PermissionNames.Pages_BasicData_AlarmInfo, L("AlarmInfo"));
            alarmInfo.CreateChildPermission(PermissionNames.Pages_BasicData_AlarmInfo_Manage, L("AlarmInfo_Manage"));

            var planProduct = basicData.CreateChildPermission(PermissionNames.Pages_Order_Product, L("Product"));
            planProduct.CreateChildPermission(PermissionNames.Pages_Order_Product_Manage, L("ProductManage"));

            #endregion

            #region 设备监测

            var devicesMonitoring = pages.CreateChildPermission(
                PermissionNames.Pages_DevicesMonitoring,
                L("DevicesMonitoring"),
                multiTenancySides: MultiTenancySides.Tenant);

            var devicesStates = devicesMonitoring.CreateChildPermission(
                PermissionNames.Pages_DevicesMonitoring_States,
                L("DevicesMonitoring_States"));

            //devicesStates.CreateChildPermission(
            //    PermissionNames.Pages_DevicesMonitoring_States_SelectTenants,
            //    L("DevicesMonitoring_States_SelectTenants"));

            devicesMonitoring.CreateChildPermission(
                PermissionNames.Pages_DevicesMonitoring_Parameters,
                L("DevicesMonitoring_Parameters"));

            //devicesMonitoring.CreateChildPermission(PermissionNames.Pages_DevicesAlarms, L("DevicesAlarms"));

            devicesMonitoring.CreateChildPermission(PermissionNames.Pages_DevicesRealtimeAlarms, L("DevicesAlarms"));

            #endregion

            #region 统计分析

            var statisticAnalysis = pages.CreateChildPermission(
                PermissionNames.Pages_StatisticAnalysis,
                L("StatisticAnalysis"),
                multiTenancySides: MultiTenancySides.Tenant);
            statisticAnalysis.CreateChildPermission(PermissionNames.Pages_AlarmStatistics, L("AlarmStatistics"));
            statisticAnalysis.CreateChildPermission(PermissionNames.Pages_HistoryParameters, L("HistoryParameters"));
            statisticAnalysis.CreateChildPermission(PermissionNames.Pages_EfficiencyTrends, L("EfficiencyTrends"));
            statisticAnalysis.CreateChildPermission(PermissionNames.Pages_TimeStatistics, L("TimeStatistics"));
            statisticAnalysis.CreateChildPermission(PermissionNames.Pages_YieldStatistics, L("YieldStatistics"));
            //statisticAnalysis.CreateChildPermission(PermissionNames.Pages_YieldAnalysisStatistics, L("ComprehensiveStatistics"));
            //statisticAnalysis.CreateChildPermission(PermissionNames.Pages_OEE, L("OEE"));
            statisticAnalysis.CreateChildPermission(PermissionNames.Pages_QualifyStatistics, L("QualifyStatistics"));
            statisticAnalysis.CreateChildPermission(PermissionNames.Pages_StatusDistributionMap, L("StatusDistributionMap"));
            #endregion

            #region 人员绩效

            var staffPerformance = pages.CreateChildPermission(
                PermissionNames.Pages_StaffPerformance,
                L("StaffPerformance"),
                multiTenancySides: MultiTenancySides.Tenant);
            staffPerformance.CreateChildPermission(
                PermissionNames.Pages_StaffPerformance_OnlineOrOffline,
                L("StaffPerformance_OnlineOrOffline"));
            staffPerformance.CreateChildPermission(
                PermissionNames.Pages_StaffPerformance_OnlineOrOfflineRecord,
                L("StaffPerformance_OnlineOrOfflineRecord"));
            staffPerformance.CreateChildPermission(
                PermissionNames.Pages_StaffPerformance_StaffPerformance,
                L("StaffPerformance_StaffPerformance"));
            staffPerformance.CreateChildPermission(
                PermissionNames.Pages_StaffPerformance_StaffYield,
                L("StaffPerformance_StaffYield"));

            #endregion

            #region 看板Visaul

            var visual = pages.CreateChildPermission(PermissionNames.Pages_Visual, L("Visual"));
            visual.CreateChildPermission(PermissionNames.Pages_VisualView, L("VisualView"));
            var notice = visual.CreateChildPermission(PermissionNames.Pages_VisualNotice, L("VisualNotice"));
            notice.CreateChildPermission(PermissionNames.Pages_VisualNotice_Manage, L("VisualNoticeManage"));

            #endregion

            #region 刀具模块

            var cutter = pages.CreateChildPermission(PermissionNames.Pages_Cutter, L("Cutter"));
            var cutterParameter = cutter.CreateChildPermission(
                PermissionNames.Pages_Cutter_CutterParameter,
                L("CutterParameter"));
            cutterParameter.CreateChildPermission(
                PermissionNames.Pages_Cutter_CutterParameter_Manage,
                L("CutterParameterManage"));
            var cutterType = cutter.CreateChildPermission(PermissionNames.Pages_Cutter_CutterType, L("CutterType"));
            cutterType.CreateChildPermission(PermissionNames.Pages_Cutter_CutterType_Manage, L("CutterTypeManage"));
            var cutterState = cutter.CreateChildPermission(PermissionNames.Pages_Cutter_CutterState, L("CutterState"));
            cutterState.CreateChildPermission(PermissionNames.Pages_Cutter_CutterState_Manage, L("CutterStateManage"));
            cutter.CreateChildPermission(
                PermissionNames.Pages_Cutter_CutterLoadAndUnloadRecord,
                L("CutterLoadAndUnloadRecord"));

            #endregion

            #region 工单

            var order = pages.CreateChildPermission(PermissionNames.Pages_Order, L("Order"));

            var orderProcess = order.CreateChildPermission(PermissionNames.Pages_Order_Process, L("Process"));
            orderProcess.CreateChildPermission(PermissionNames.Pages_Order_Process_Manage, L("ProcessManage"));
            var orderStandardTime = order.CreateChildPermission(
                PermissionNames.Pages_Order_StandardTime,
                L("StandardTime"));
            orderStandardTime.CreateChildPermission(
                PermissionNames.Pages_Order_StandardTime_Manage,
                L("StandardTimeManage"));
            var orderCraft = order.CreateChildPermission(PermissionNames.Pages_Order_Craft, L("Craft"));
            orderCraft.CreateChildPermission(PermissionNames.Pages_Order_Craft_Manage, L("CraftManage"));

            var machineProcess = order.CreateChildPermission(PermissionNames.Pages_Order_MachineProcess, L("MachineProcess"));
            machineProcess.CreateChildPermission(PermissionNames.Pages_Order_MachineProcess_Manage, L("MachineProcessManage"));

            var orderProductionPlan = order.CreateChildPermission(
                PermissionNames.Pages_Order_ProductionPlan,
                L("ProductionPlan"));
            orderProductionPlan.CreateChildPermission(
                PermissionNames.Pages_Order_ProductionPlan_Manage,
                L("ProductionPlanManage"));
            var orderLoginReport = order.CreateChildPermission(
                PermissionNames.Pages_Order_LoginReport,
                L("LoginReport"));
            orderLoginReport.CreateChildPermission(
                PermissionNames.Pages_Order_LoginReport_Manage,
                L("LoginReportManage"));
            var orderDefectiveStatistics = order.CreateChildPermission(
                PermissionNames.Pages_Order_DefectiveStatistics,
                L("DefectiveStatistics"));
            orderDefectiveStatistics.CreateChildPermission(
                PermissionNames.Pages_Order_DefectiveStatistics_Manage,
                L("DefectiveStatisticsManage"));

            var hourlyYieldAnalysis = order.CreateChildPermission(PermissionNames.Pages_Order_MachineReport, L("MachineReport"));
            hourlyYieldAnalysis.CreateChildPermission(
                PermissionNames.Pages_Order_MachineReport_Manage,
                L("MachineReportManage"));

            #endregion

            #region 计划产量

            //var plan = pages.CreateChildPermission(PermissionNames.Pages_Plan, L("Plan"), multiTenancySides: MultiTenancySides.Tenant);
            //plan.CreateChildPermission(PermissionNames.Pages_Plan_Manage, L("PlanManage"), multiTenancySides: MultiTenancySides.Tenant);

            var plan = pages.CreateChildPermission(PermissionNames.Pages_Plan_Manage, L("Plan"), multiTenancySides: MultiTenancySides.Tenant);

            #endregion

            #region 追溯管理
            var tracking = pages.CreateChildPermission(PermissionNames.Pages_Traceability, L("Traceability"));

            var partsTracking = tracking.CreateChildPermission(PermissionNames.Pages_Traceability_CatalogQuery, L("TraceCatalogQuery"));
            var traceabilitySettings = tracking.CreateChildPermission(PermissionNames.Pages_Traceability_Settings, L("TraceSetings"));
            var orderDefectiveReasons = order.CreateChildPermission(
                PermissionNames.Pages_Order_DefectiveReasons,
                L("DefectiveReasons"));
            orderDefectiveReasons.CreateChildPermission(
                PermissionNames.Pages_Order_DefectiveReasons_Manage,
                L("DefectiveReasonsManage"));
            var traceabilityNgParts = tracking.CreateChildPermission(PermissionNames.Pages_Traceability_NgParts, L("TraceNgParts"));
            var traceabilityNgPartsManage = traceabilityNgParts.CreateChildPermission(PermissionNames.Pages_Traceability_NgParts_Manage, L("TraceNgPartsManage"));
            #endregion

            #region 消息通知管理
            var notification = pages.CreateChildPermission(PermissionNames.Pages_Notification, L("Notification"));
            var notificationRules = notification.CreateChildPermission(PermissionNames.Pages_Notification_Rules, L("NotificationRules"));
            notificationRules.CreateChildPermission(PermissionNames.Pages_Notification_Rules_Manage, L("NotificationRulesManage"));
            notification.CreateChildPermission(PermissionNames.Pages_Notification_Records, L("NotificationRecords"));
            #endregion

            #region 原因反馈

            var feedback = pages.CreateChildPermission(PermissionNames.Pages_ReasonFeedback, L("ReasonFeedback"));
            var reasonFeedback = feedback.CreateChildPermission(PermissionNames.Pages_ReasonFeedback_Feedback, L("ReasonFeedback"));
            reasonFeedback.CreateChildPermission(PermissionNames.Pages_ReasonFeedback_Feedback_Manage, L("ReasonFeedbackManage"));

            feedback.CreateChildPermission(PermissionNames.Pages_ReasonFeedback_Analysis, L("ReasonFeedbackAnalysis"));
            var calendar = feedback.CreateChildPermission(PermissionNames.Pages_ReasonFeedback_Calendar, L("FeedbackCalendar"));
            calendar.CreateChildPermission(PermissionNames.Pages_ReasonFeedback_Calendar_Manage,
                L("FeedbackCalendarManage"));

            #endregion

            #region 报表

            var report = pages.CreateChildPermission(PermissionNames.Pages_RDLCReport, L("RDLCReport"));
            //report.CreateChildPermission(PermissionNames.Pages_RDLCReport_ShiftYield, L("ShiftYield"));
            report.CreateChildPermission(PermissionNames.Pages_RDLCReport_StateConsumeTime, L("StateConsumeTime"));
            report.CreateChildPermission(PermissionNames.Pages_RDLCReport_MachineUtilizationRate, L("MachineUtilizationRate"));
            report.CreateChildPermission(PermissionNames.Pages_RDLCReport_PersonYield, L("PersonYield"));
            report.CreateChildPermission(PermissionNames.Pages_RDLCReport_PersonPerformance, L("PersonPerformance"));
            report.CreateChildPermission(PermissionNames.Pages_RDLCReport_Plan, L("ReportOfPlan"));

            #endregion

            #region 设备维护
            var maintain = pages.CreateChildPermission(PermissionNames.Pages_Maintain, L("Maintain"));
            var maintainPlan = maintain.CreateChildPermission(PermissionNames.Pages_Maintain_Plan, L("MaintainPlan"));
            maintainPlan.CreateChildPermission(PermissionNames.Pages_Maintain_Plan_Manage, L("MaintainPlanManage"));
            var maintainOrder = maintain.CreateChildPermission(PermissionNames.Pages_Maintain_Order, L("MaintainOrder"));
            maintainOrder.CreateChildPermission(PermissionNames.Pages_Maintain_Order_Manage, L("MaintainOrderManage"));
            var maintainRequest = maintain.CreateChildPermission(PermissionNames.Pages_Maintain_Request, L("MaintainRequest"));
            maintainRequest.CreateChildPermission(PermissionNames.Pages_Maintain_Request_Manage, L("MaintainRequestManage"));
            var maintainRepair = maintain.CreateChildPermission(PermissionNames.Pages_Maintain_Repair, L("MaintainRepair"));
            maintainRepair.CreateChildPermission(PermissionNames.Pages_Maintain_Repair_Manage, L("MaintainRepairManage"));
            #endregion

            #region 语言模块
            //pages.CreateChildPermission(PermissionNames.Pages_MultiLanguage, L("Language"));
            #endregion

            #region 包装
            var carton = pages.CreateChildPermission(PermissionNames.Pages_Carton, L("Carton"));

            carton.CreateChildPermission(PermissionNames.Pages_Carton_CartonRules, L("CartonRules"));

            carton.CreateChildPermission(PermissionNames.Pages_Carton_CartonSetting, L("CartonSetting"));

            carton.CreateChildPermission(PermissionNames.Pages_Carton_CartonTraceability, L("CartonTraceability"));

            carton.CreateChildPermission(PermissionNames.Pages_Carton_CartonPrinting, L("CartonPrinting"));
            #endregion

            #region 工艺管理
            var CraftMaintain = pages.CreateChildPermission(PermissionNames.Pages_CraftMaintain, L("CraftMaintain"));


            var CraftMaintain_TongsMaintain = CraftMaintain.CreateChildPermission(PermissionNames.Pages_CraftMaintain_TongsMaintain, L("TongsMaintain"));
            CraftMaintain_TongsMaintain.CreateChildPermission(PermissionNames.Pages_CraftMaintain_TongsMaintain_Manage, L("TongsMaintainManage"));

            CraftMaintain.CreateChildPermission(PermissionNames.Pages_CraftMaintain_FlexibleCraftPath, L("FlexibleCraftPath"))
            .CreateChildPermission(PermissionNames.Pages_CraftMaintain_FlexibleCraftPath_Manage, L("FlexibleCraftPathManage"));


            var CraftMaintain_FmsCutter = CraftMaintain.CreateChildPermission(PermissionNames.Pages_CraftMaintain_FmsCutter, L("FmsCutter"));
            CraftMaintain_FmsCutter.CreateChildPermission(PermissionNames.Pages_CraftMaintain_FmsCutter_Manage, L("FmsCutterManage"));
            #endregion
            #region Archives 
            //var Archives = pages.CreateChildPermission(PermissionNames.Pages_Archives, L("Archives"));

            ////如果是生成的是页面，则拷贝下面两行即可，如果是一个新的模块的页面，则拷贝整个region内的代码
            //var Archives_ArchiveEntry = Archives.CreateChildPermission(PermissionNames.Pages_Archives_ArchiveEntry, L("ArchiveEntry"));
            //Archives_ArchiveEntry.CreateChildPermission(PermissionNames.Pages_Archives_ArchiveEntry_Manage, L("ArchiveEntryManage"));
            #endregion
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, BtlCoreConsts.LocalizationSourceName);
        }

    }
}
