namespace Wimi.BtlCore.ShiftTargetYiled
{
    using System;
    using System.Collections.Generic;

    using Abp.Domain.Services;

    public interface IShiftTargetYiledManager : IDomainService
    {
        string CheckShiftDay(DateTime startTime, DateTime endTime, int productId, int shiftItemId);

        IEnumerable<DateTime> ShiftDayList(DateTime startTime, DateTime endTime);
    }
}