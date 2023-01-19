namespace Wimi.BtlCore.ThirdpartyApis
{
    using System.Collections.Generic;

    using Abp.Dependency;

    public class ThirdpartyApiProvider : ITransientDependency
    {
        private const string VisualComponentRootAddress = "/api/services/app/ComponentData";

        public ThirdpartyApiProvider()
        {
            this.Item = new List<ThirdpartyApiDefinition>();
        }

        private List<ThirdpartyApiDefinition> Item { get; set; }

        public List<ThirdpartyApiDefinition> ListApiDefinitions()
        {
            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListPerHourYields",
                    ThirdpartyApiCodes.Visual_Component_HourlyYiled,
                    "每小时产量(总)"));

            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListHourlyMachineYiled",
                    ThirdpartyApiCodes.Visual_Component_HourlyMachineYiled,
                    "设备每小时产量"));
            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListHourlyMachineYiledByShiftDay",
                    ThirdpartyApiCodes.Visual_Component_HourlyMachineYiledByShiftDay,
                    "设备工厂日每小时产量"));


            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListRealtimeMachineInfos",
                    ThirdpartyApiCodes.Visual_Component_MachineStatus,
                    "设备实时状态"));

            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListMachineAlarming",
                    ThirdpartyApiCodes.Visual_Component_MachineAlarming,
                    "设备报警信息"));

            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListToolWarnings",
                    ThirdpartyApiCodes.Visual_Component_ToolWarning,
                    "刀具预警"));

            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListMachineStateDistribution",
                    ThirdpartyApiCodes.Visual_Component_StateDistribution,
                    "设备状态分布"));


            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListMachineActivation",
                    ThirdpartyApiCodes.Visual_Component_MachineActivation,
                    "当班次设备稼动率"));

            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListMachineActivationByDay",
                    ThirdpartyApiCodes.Visual_Component_MachineActivationByDay,
                    "当天设备稼动率"));


            this.Item.Add(
              new ThirdpartyApiDefinition(
                  $"{VisualComponentRootAddress}/ListCurrentShiftCapcity",
                  ThirdpartyApiCodes.Visual_Component_ListCurrentShiftCapcity,
                  "当前班次产量"));

            this.Item.Add(
              new ThirdpartyApiDefinition(
                  $"{VisualComponentRootAddress}/ListGanttChart",
                  ThirdpartyApiCodes.Visual_Component_ListGanttChart,
                  "设备状态"));

            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListPlanRate",
                    ThirdpartyApiCodes.Visual_Component_ListPlanRate,
                    "计划达成率"));

            this.Item.Add(
                new ThirdpartyApiDefinition(
                    $"{VisualComponentRootAddress}/ListConfigRealtimeMachineInfos",
                    ThirdpartyApiCodes.Visual_Component_ListConfigRealtimeMachineInfos,
                    "设备实时状态(参数可选)"));

            return this.Item;
        }
    }
}