using System.Threading.Tasks;

namespace Wimi.BtlCore.Identity
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}