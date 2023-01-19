using System.Linq;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.StateInfos;


namespace Wimi.BtlCore.EntityFrameworkCore.Seed.Host
{
    public class DefaultStatusInfoCreator
    {
        private readonly BtlCoreDbContext context;

        public DefaultStatusInfoCreator(BtlCoreDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            this.InitStatusInfo();
        }

        private void InitStatusInfo()
        {
            if (!this.context.StatusInfos.Any(s => s.Code == EnumMachineState.Offline.ToString()))
                this.context.StatusInfos.Add(
                    new StateInfo
                    {
                        DisplayName = "离线",
                        Type = EnumMachineStateType.State,
                        IsPlaned = true,
                        Hexcode = "#c4c4c4",
                        Code = EnumMachineState.Offline.ToString(),
                        OriginalCode = (int)EnumMachineState.Offline,
                        IsStatic = true
                    });

            if (!this.context.StatusInfos.Any(s => s.Code == EnumMachineState.Free.ToString()))
                this.context.StatusInfos.Add(
                    new StateInfo
                    {
                        DisplayName = "空闲",
                        Type = EnumMachineStateType.State,
                        IsPlaned = true,
                        Hexcode = "#f2a332",
                        Code = EnumMachineState.Free.ToString(),
                        OriginalCode = (int)EnumMachineState.Free,
                        IsStatic = true
                    });

            if (!this.context.StatusInfos.Any(s => s.Code == EnumMachineState.Run.ToString()))
                this.context.StatusInfos.Add(
                    new StateInfo
                    {
                        DisplayName = "运行",
                        Type = EnumMachineStateType.State,
                        IsPlaned = true,
                        Hexcode = "#4cae4c",
                        Code = EnumMachineState.Run.ToString(),
                        OriginalCode = (int)EnumMachineState.Run,
                        IsStatic = true
                    });
            if (!this.context.StatusInfos.Any(s => s.Code == EnumMachineState.Stop.ToString()))
                this.context.StatusInfos.Add(
                    new StateInfo
                    {
                        DisplayName = "停机",
                        Type = EnumMachineStateType.State,
                        IsPlaned = true,
                        Hexcode = "#d43a36",
                        Code = EnumMachineState.Stop.ToString(),
                        OriginalCode = (int)EnumMachineState.Stop,
                        IsStatic = true
                    });
            if (!this.context.StatusInfos.Any(s => s.Code == EnumMachineState.Debug.ToString()))
                this.context.StatusInfos.Add(
                    new StateInfo
                    {
                        DisplayName = "设定",
                        Type = EnumMachineStateType.State,
                        IsPlaned = true,
                        Hexcode = "#1d89cf",
                        Code = EnumMachineState.Debug.ToString(),
                        OriginalCode = (int)EnumMachineState.Debug,
                        IsStatic = true
                    });

            this.context.SaveChanges();
        }
    }
}
