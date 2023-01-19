using Abp.Dependency;

namespace Wimi.BtlCore.Cartons
{
    public interface ICartonRepository : ITransientDependency
    {
        bool AppointNumber(string serialNumber);

        void InsertCartonSerialNumber(CartonSerialNumber cartonSerialNumber);
    }
}
