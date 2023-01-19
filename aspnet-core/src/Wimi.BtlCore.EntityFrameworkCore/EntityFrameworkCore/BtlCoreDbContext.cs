using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization.Roles;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.Alarms;
using Wimi.BtlCore.BasicData.Capacities;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.MachineTypes;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.BasicData.States;
using Wimi.BtlCore.Cartons;
using Wimi.BtlCore.Chat;
using Wimi.BtlCore.Dmps;
using Wimi.BtlCore.Editions;
using Wimi.BtlCore.Feedback;
using Wimi.BtlCore.Friendships;
using Wimi.BtlCore.Maintain;
using Wimi.BtlCore.MultiTenancy;
using Wimi.BtlCore.Notices;
using Wimi.BtlCore.Notifications;
using Wimi.BtlCore.Order.Crafts;
using Wimi.BtlCore.Order.DefectiveParts;
using Wimi.BtlCore.Order.DefectiveReasons;
using Wimi.BtlCore.Order.MachineDefectiveRecords;
using Wimi.BtlCore.Order.MachineProcesses;
using Wimi.BtlCore.Order.PartDefects;
using Wimi.BtlCore.Order.Processes;
using Wimi.BtlCore.Order.ProductionPlans;
using Wimi.BtlCore.Order.Products;
using Wimi.BtlCore.Order.StandardTimes;
using Wimi.BtlCore.Order.WorkOrders;
using Wimi.BtlCore.PersistentLogs;
using Wimi.BtlCore.Plan;
using Wimi.BtlCore.StaffPerformance;
using Wimi.BtlCore.SummaryStatistics;
using Wimi.BtlCore.ThirdpartyApis;
using Wimi.BtlCore.Trace;
using Wimi.BtlCore.FmsCutters;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.Calendars;
using Wimi.BtlCore.WeChart;
using Wimi.BtlCore.Cutter;
using Abp;
using Wimi.BtlCore.ShiftTargetYiled;
using Wimi.BtlCore.Storage;
using Wimi.BtlCore.CraftMaintain;
using Wimi.BtlCore.Configuration;
using Abp.Application.Features;
using Wimi.BtlCore.Archives;

namespace Wimi.BtlCore.EntityFrameworkCore
{
    public class BtlCoreDbContext : AbpZeroDbContext<Tenant, Role, User, BtlCoreDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<Machine> Machines { get; set; }

        public virtual DbSet<MachineType> MachineTypes { get; set; }

        public virtual DbSet<Alarm> Alarms { get; set; }

        public virtual DbSet<AlarmInfo> AlarmInfos { get; set; }

        public virtual DbSet<Capacity> Capacities { get; set; }

        public virtual DbSet<Calendar> Calendars { get; set; }

        public virtual DbSet<DeviceGroup> DeviceGroups { get; set; }

        public virtual DbSet<MachineDeviceGroup> MachineDeviceGroups { get; set; }

        public virtual DbSet<MachineGatherParam> MachineGatherParams { get; set; }

        public virtual DbSet<MachineProgram> MachinePrograms { get; set; }

        public virtual DbSet<StateInfo> StateInfos { get; set; }

        public virtual DbSet<State> States { get; set; }

        public virtual DbSet<Process> Process { get; set; }

        public virtual DbSet<StandardTime> StandardTime { get; set; }

        public virtual DbSet<Craft> Crafts { get; set; }

        public virtual DbSet<CraftProcess> CraftProcesses { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ProductGroup> ProductGroups { get; set; }

        public virtual DbSet<ProductionPlan> ProductionPlans { get; set; }

        public virtual DbSet<WorkOrder> WorkOrders { get; set; }

        public virtual DbSet<PartDefect> PartDefects { get; set; }

        public virtual DbSet<DefectivePart> DefectiveParts { get; set; }

        public virtual DbSet<DefectivePartReason> DefectivePartReasons { get; set; }

        public virtual DbSet<DefectiveReason> DefectiveReasons { get; set; }

        public virtual DbSet<MachineProcess> MachineProcesses { get; set; }

        public virtual DbSet<MachineDefectiveRecord> MachineDefectiveRecords { get; set; }

        public virtual DbSet<ShiftSolution> ShiftSolutions { get; set; }

        public virtual DbSet<ShiftSolutionItem> ShiftSolutionItems { get; set; }

        public virtual DbSet<ShiftHistory> ShiftHistories { get; set; }

        public virtual DbSet<MachinesShiftDetail> MachinesShiftDetails { get; set; }

        public virtual DbSet<MachineShiftEffectiveInterval> MachineShiftEffectiveIntervals { get; set; }

        public virtual DbSet<MachineShiftChangeLog> MachineShiftChangeLogs { get; set; }

        public virtual DbSet<PlanTarget> PlanTargets { get; set; }

        public virtual DbSet<ProcessPlan> ProcessPlans { get; set; }

        public virtual DbSet<MaintainPlan> MaintainPlans { get; set; }

        public virtual DbSet<RepairRequest> RepairRequests { get; set; }

        public virtual DbSet<MaintainOrder> MaintainOrders { get; set; }

        public virtual DbSet<ReasonFeedbackRecord> ReasonFeedbackRecords { get; set; }

        public virtual DbSet<TraceCatalog> TraceCatalogs { get; set; }

        public virtual DbSet<TraceFlowRecord> TraceFlowRecords { get; set; }

        public virtual DbSet<TraceFlowSetting> TraceFlowSettings { get; set; }

        public virtual DbSet<TraceRelatedMachine> TraceRelatedMachines { get; set; }

        public virtual DbSet<CutterType> CutterTypes { get; set; }

        public virtual DbSet<CutterStates> CutterStates { get; set; }

        public virtual DbSet<CutterParameter> CutterParameters { get; set; }

        public virtual DbSet<CutterModel> CutterModels { get; set; }

        public virtual DbSet<CutterLoadAndUnloadRecord> CutterLoadAndUnloadRecords { get; set; }

        public virtual DbSet<ThirdpartyApi> ThirdpartyApis { get; set; }

        public virtual DbSet<Carton> Cartons { get; set; }

        public virtual DbSet<CalibratorCode> CalibratorCodes { get; set; }

        public virtual DbSet<CartonRecord> CartonRecords { get; set; }

        public virtual DbSet<CartonRule> CartonRules { get; set; }

        public virtual DbSet<FmsCutter> FmsCutters { get; set; }

        public virtual DbSet<CartonRuleDetail> CartonRuleDetails { get; set; }

        public virtual DbSet<CartonSerialNumber> CartonSerialNumbers { get; set; }

        public virtual DbSet<CartonSetting> CartonSettings { get; set; }

        public virtual DbSet<Dmp> Dmps { get; set; }

        public virtual DbSet<DmpMachine> DmpMachines { get; set; }

        public virtual DbSet<DriverConfig> DriverConfigs { get; set; }

        public virtual DbSet<MachineDriver> MachineDrivers { get; set; }

        public virtual DbSet<MachineVariable> MachineVariables { get; set; }

        public virtual DbSet<DailyStatesSummary> DailyStatesSummaries { get; set; }

        public virtual DbSet<DeviceGroupPermissionSetting> DeviceGroupPermissions { get; set; }

        public virtual DbSet<DeviceGroupRolePermissionSetting> DeviceGroupRolePermissions { get; set; }

        public virtual DbSet<DeviceGroupUserPermissionSetting> DeviceGroupUserPermissions { get; set; }

        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

        public virtual DbSet<InfoLogDetail> InfoLogDetails { get; set; }

        public virtual DbSet<InfoLog> InfoLogs { get; set; }

        public virtual DbSet<Notice> Notices { get; set; }

        public virtual DbSet<NotificationType> NotificationTypes { get; set; }

        public virtual DbSet<OnlineAndOfflineLog> OnlineAndOfflineLogs { get; set; }

        public virtual DbSet<PerformancePersonnelOnDevice> PerformancePersonnelOnDevices { get; set; }

        public virtual DbSet<NotificationRule> NotificationRules { get; set; }

        public virtual DbSet<NotificationRuleDetail> NotificationRuleDetails { get; set; }

        public virtual DbSet<NotificationRecord> NotificationRecords { get; set; }

        public virtual DbSet<DeviceGroupYieldMachine> DeviceGroupYieldMachines { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<StateInfo> StatusInfos { get; set; }

        public virtual DbSet<ShortcutMenu> ShortcutMenus { get; set; }

        public virtual DbSet<FmsCutterSetting> FmsCutterSettings { get; set; }

        public virtual DbSet<ShiftCalendar> ShiftCalendars { get; set; }

        public virtual DbSet<MachineNetConfig> MachineNetConfigs { get; set; }

        public virtual DbSet<ShiftTargetYileds> ShiftTargetYileds { get; set; }

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }


        public virtual DbSet<WorkOrderTasks> WorkOrderTasks { get; set; }

        public virtual DbSet<WorkOrderDefectiveRecords> WorkOrderDefectiveRecords { get; set; }

        public virtual DbSet<Tong> Tongs { get; set; }

        public virtual DbSet<FlexibleCraft> FlexibleCrafts { get; set; }

        public virtual DbSet<FlexibleCraftProcesse> FlexibleCraftProcesses { get; set; }

        public virtual DbSet<FlexibleCraftProcesseMap> FlexibleCraftProcesseMaps { get; set; }

        public virtual DbSet<FlexibleCraftProcedureCutterMap> FlexibleCraftProcedureCutterMaps { get; set; }

        public virtual DbSet<CustomField> CustomFields { get; set; }

        public virtual DbSet<FmsCutterExtend> FmsCutterExtends { get; set; }

        public virtual DbSet<WeChatNotification> WeChatNotifications { get; set; }

        public virtual DbSet<SyncDataFlag> SyncDataFlags { get; set; }
        public virtual DbSet<FeedbackCalendar> FeedbackCalendars { get; set; }
        public virtual DbSet<FeedbackCalendarDetail> FeedbackCalendarDetails { get; set; }

        public virtual DbSet<ArchiveEntry> ArchiveEntries { get; set; }

        public BtlCoreDbContext(DbContextOptions<BtlCoreDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ChangeAbpTablePrefix<Tenant, Role, User>(null);
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TraceRelatedMachine>().HasOne(q => q.TraceFlowSetting).WithMany(q => q.RelatedMachines).HasForeignKey(q => q.TraceFlowSettingId).IsRequired();
            modelBuilder.Entity<FmsCutterExtend>().HasOne(q => q.FmsCutter).WithMany(q => q.Items)
                .HasForeignKey(q => q.FmsCutterId).IsRequired();

            modelBuilder.Entity<Capacity>().HasIndex(p => new { p.IsLineOutput });
            modelBuilder.Entity<MachineGatherParam>().Property(m => m.IsShowForParam).HasDefaultValue(0);
            modelBuilder.Entity<MachineGatherParam>().Property(m => m.IsShowForStatus).HasDefaultValue(0);
            modelBuilder.Entity<MachineGatherParam>().Property(m => m.IsShowForVisual).HasDefaultValue(0);
            modelBuilder.Entity<Capacity>().Property(c => c.IsLineOutputOffline).HasDefaultValue(0);
            modelBuilder.Entity<Capacity>().Property(c => c.IsLineOutput).HasDefaultValue(0);

        }
    }
}
