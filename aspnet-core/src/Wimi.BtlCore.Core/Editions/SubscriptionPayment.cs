using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Editions
{
    [Table("AppSubscriptionPayments")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class SubscriptionPayment : FullAuditedEntity<long>
    {
        [Comment("支付方式")]
        public SubscriptionPaymentGatewayType Gateway { get; set; }

        [Comment("数量")]
        public decimal Amount { get; set; }

        [Comment("支付状态")]
        public SubscriptionPaymentStatus Status { get; set; }

        [Comment("版本Id")]
        public int EditionId { get; set; }

        [Comment("租户Id")]
        public int TenantId { get; set; }

        [Comment("日数量")]
        public int DayCount { get; set; }

        [Comment("支付时长 月/年")]
        public PaymentPeriodType? PaymentPeriodType { get; set; }

        [Comment("支付Id")]
        public string PaymentId { get; set; }

        [Comment("版本")]
        public Edition Edition { get; set; }

        [Comment("发票号")]
        public string InvoiceNo { get; set; }

        public void Cancel()
        {
            if (Status == SubscriptionPaymentStatus.Processing)
            {
                Status = SubscriptionPaymentStatus.Cancelled;
            }
        }
    }
}
