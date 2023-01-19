namespace Wimi.BtlCore.Dto
{
    /// <summary>
    /// This interface is defined to standardize to request a limited result.
    /// </summary>
    public interface IDrawCounter
    {
        /// <summary>
        /// Draw counter.
        /// This is used by DataTables to ensure that 
        /// the Ajax returns from server-side processing requests are drawn in sequence by DataTables 
        /// (Ajax requests are asynchronous and thus can return out of sequence). 
        /// This is used as part of the draw return parameter.
        /// </summary>
        int Draw { get; set; }
    }
}