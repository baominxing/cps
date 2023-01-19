namespace Wimi.BtlCore.Web.Startup
{
    public class PageNames
    {
        public const string Home = "Home";
        public const string About = "About";
        public const string Tenants = "Tenants";
        public const string Users = "Users";
        public const string Roles = "Roles";

        public static class App
        {
            public static class Common
            {
                public const string Administration = "Administration";

                public const string AuditLogs = "Administration.AuditLogs";

                public const string Languages = "Administration.Languages";

                public const string Notification = "Administration.WeChatNotification";
                public const string OrganizationUnits = "Administration.OrganizationUnits";

                public const string Roles = "Administration.Roles";

                public const string Users = "Administration.Users";
            }

            public static class Host
            {
                public const string Dashboard = "Dashboard.Host";

                public const string Editions = "Editions";

                public const string Maintenance = "Administration.Maintenance";

                public const string Settings = "Administration.Settings.Host";

                public const string Tenants = "Tenants";
            }

            public static class Page
            {
                public const string Administration_DmpMachineSetting = "Administration.DmpMachineSetting";

                public const string BasicData = "BasicData";

                public const string BasicData_AlarmInfo = "BasicData.AlarmInfo";

                public const string BasicData_DeviceGroup = "BasicData.DeviceGroup";

                public const string BasicData_GatherParamsGroup = "BasicData.GatherParamsGroup";

                public const string BasicData_GatherParamsSetting = "BasicData.GatherParamsSetting";

                public const string BasicData_ImportData = "BasicData.ImportData";

                public const string BasicData_MachineSetting = "BasicData.MachineSetting";

                public const string BasicData_MachineShiftSetting = "BasicData.MachineShiftSetting";
                public const string BasicData_Maintain = "BasicData.Maintain";
                public const string BasicData_MachineType = "BasicData.MachineType";

                public const string BasicData_ShiftSetting = "BasicData.ShiftSetting";

                public const string BasicData_ShiftTargetYiled = "Pages.BasicData.ShiftTargetYiled";

                public const string BasicData_StateInfo = "BasicData.StateInfo";

                public const string Cutter = "Cutter";

                public const string Cutter_CutterLoadAndUnloadRecord = "Cutter.CutterLoadAndUnloadRecord";

                public const string Cutter_CutterParameter = "Cutter.CutterParameter";

                public const string Cutter_CutterState = "Cutter.CutterState";

                public const string Cutter_CutterType = "Cutter.CutterType";

                public const string DevicesMonitoring = "DevicesMonitoring";

                public const string DevicesMonitoring_DevicesAlarms = "DevicesMonitoring.DevicesAlarms";

                public const string DevicesMonitoring_DevicesRealtimeAlarms = "DevicesMonitoring.DevicesRealtimeAlarms";

                public const string DevicesMonitoring_Parameters = "DevicesMonitoring.Parameters";

                public const string DevicesMonitoring_States = "DevicesMonitoring.States";

                public const string DevicesMonitoring_States_SelectTenants = "DevicesMonitoring.States.SelectTenants";

                public const string Order = "Order";

                public const string Order_Craft = "Order.Craft";

                public const string Order_DefectiveReasons = "Order.DefectiveReasons";

                public const string Order_DefectiveStatistics = "Order.DefectiveStatistics";

                public const string Order_LoginReport = "Order.LoginReport";

                public const string Order_Process = "Order.Process";

                public const string Order_Product = "Order.Product";

                public const string Order_ProductionPlan = "Order.ProductionPlan";

                public const string Order_StandardTime = "Order.StandardTime";

                public const string Order_MachineProcess = "Order.MachineProcess";

                public const string Order_MachineReport = "Order.MachineReport";

                public const string StaffPerformance = "StaffPerformance";

                public const string StaffPerformance_OnlineOrOffline = "StaffPerformance.OnlineOrOffline";

                public const string StaffPerformance_OnlineOrOfflineRecord = "StaffPerformance.OnlineOrOfflineRecord";

                public const string StaffPerformance_StaffPerformance = "StaffPerformance.StaffPerformance";

                public const string StaffPerformance_StaffYield = "StaffPerformance.StaffYield";

                public const string StatisticAnalysis = "StatisticAnalysis";

                public const string StatisticAnalysis_AlarmStatistics = "StatisticAnalysis.AlarmStatistics";

                public const string StatisticAnalysis_EfficiencyTrends = "StatisticAnalysis.EfficiencyTrends";

                public const string StatisticAnalysis_HistoryParameters = "StatisticAnalysis.HistoryParameters";

                public const string StatisticAnalysis_TimeStatistics = "StatisticAnalysis.TimeStatistics";

                public const string StatisticAnalysis_YieldAnalysisStatistics =
                    "StatisticAnalysis.YieldAnalysisStatistics";

                public const string StatisticAnalysis_YieldStatistics = "StatisticAnalysis.YieldStatistics";

                public const string StatisticAnalysis_OEE = "StatisticAnalysis.OEE";

                public const string StatisticAnalysis_QualifyStatistics = "StatisticAnalysis.QualifyStatistics";

                public const string StatisticAnalysis_StatusDistributionMap = "StatisticAnalysis_StatusDistributionMap";

                public const string Visual = "Visual";

                public const string Visual_Notice = "Visual.Notice";

                public const string Visual_View = "Visual.View";

                public const string Traceability = "Traceability";
                public const string Traceability_Catalog_Query = "Traceability.CatalogQuery";
                public const string Traceability_Settings = "Traceability.Settings";
                public const string Traceability_NgParts = "Traceability.NgParts";

                public const string Notification = "Notification";

                public const string Notification_Rules = "Notification.Rules";

                public const string Notification_Records = "Notification.Records";

                public const string ReasonFeedback = "ReasonFeedback";

                public const string ReasonFeedback_Feedback = "ReasonFeedback.Feedback";

                public const string ReasonFeedback_Analysis = "ReasonFeedback.Analysis";

                public const string ReasonFeedback_Calendar = "ReasonFeedback.Calendar";

                //public const string ProducePlan = "ProducePlan";

                public const string ProducePlan_Plans = "ProducePlan_Plans";

                public const string RDLCReport = "RDLCReport";
                public const string RDLCReport_ShiftYield = "RDLCReport.ShiftYield";
                public const string RDLCReport_MachineUtilizationRate = "RDLCReport.MachineUtilizationRate";
                public const string RDLCReport_StateConsumeTime = "RDLCReport.StateConsumeTime";
                public const string RDLCReport_PersonYield = "RDLCReport.PersonYield";
                public const string RDLCReport_PersonPerformance = "RDLCReport.PersonPerformance";
                public const string RDLCReport_Plan = "RDLCReport.Plan";


                public const string Maintain = "Maintain";

                public const string Maintain_Plan = "Maintain.Plan";

                public const string Maintain_Order = "Maintain.Order";

                public const string Maintain_Request = "Maintain.Request";

                public const string Maintain_Repair = "Maintain.Repair";
                public const string MultiLanguage = "MultiLanguage";
                public const string MultiLanguage_LanguageManage = "MultiLanguage.LanguageManage";

                public const string Carton = "Carton";

                public const string Carton_CartonRules = "Carton.CartonRules";
                public const string Carton_CartonSetting = "Carton.CartonSetting";
                public const string Carton_CartonPrinting = "Carton.CartonPrinting";

                public const string Carton_CartonTraceability = "Carton.Traceability";

                #region չ
                public const string CraftMaintain = "CraftMaintain";

                public const string CraftMaintain_TongsMaintain = "CraftMaintain.TongsMaintain";

                public const string CraftMaintain_FlexibleCraftPath = "CraftMaintain.FlexibleCraftPath";

                public const string CraftMaintain_TongsMaintain_Manage = "CraftMaintain.TongsMaintain.Manage";
                public const string CraftMaintain_FmsCutter = "CraftMaintain.FmsCutter";
                #endregion
                #region Archives
                public const string Archives = "Archives";

                //如果是在已有模块下新增页面，只需要拷贝下面一行代码，如果需要新增一个模块，则拷贝整个region代码块
                public const string Archives_ArchiveEntry = "Archives.ArchiveEntry";
                #endregion

            }

            public static class Tenant
            {
                public const string Dashboard = "Dashboard.Tenant";

                public const string Settings = "Administration.Settings.Tenant";
            }
        }

        public static class Frontend
        {
            public const string About = "Frontend.About";

            public const string Home = "Frontend.Home";
        }

    }


}
