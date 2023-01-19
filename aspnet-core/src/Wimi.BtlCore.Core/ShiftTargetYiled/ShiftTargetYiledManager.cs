namespace Wimi.BtlCore.ShiftTargetYiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abp.Domain.Repositories;

    using Castle.Core.Internal;

    public class ShiftTargetYiledManager : BtlCoreDomainServiceBase, IShiftTargetYiledManager
    {
        private readonly IRepository<ShiftTargetYileds> shiftTargetYiledRepository;

        public ShiftTargetYiledManager(IRepository<ShiftTargetYileds> shiftTargetYiledRepository)
        {
            this.shiftTargetYiledRepository = shiftTargetYiledRepository;
        }

        public string CheckShiftDay(DateTime startTime, DateTime endTime, int productId, int shiftItemId)
        {
            var result = string.Empty;
            var query =
                this.shiftTargetYiledRepository.GetAll()
                    .Where(
                        s =>
                        s.ShiftDay >= startTime && s.ShiftDay <= endTime && s.ProductId == productId
                        && s.ShiftSolutionItemId == shiftItemId);

            foreach (var s in query)
            {
                result += s.GetShiftDayString();
            }
            return result;
        }

        public IEnumerable<DateTime> ShiftDayList(DateTime startTime, DateTime endTime)
        {
            return Enumerable.Range(0, (endTime - startTime).Days + 1).Select(d => startTime.AddDays(d));
        }
    }
}