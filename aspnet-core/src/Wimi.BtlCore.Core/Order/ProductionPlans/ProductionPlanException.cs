using System;

namespace Wimi.BtlCore.Order.ProductionPlans
{
    public class ProductionPlanException : Exception
    {
    }

    public class ProductNotExistException : ProductionPlanException
    {
        public static readonly ProductNotExistException Default = new ProductNotExistException();
    }

    public class CarftNotExistException : ProductionPlanException
    {
    }
}
