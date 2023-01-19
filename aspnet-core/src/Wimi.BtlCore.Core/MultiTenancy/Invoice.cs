using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.MultiTenancy
{
    [Table("Invoices")]
    public class Invoice : Entity<int>
    {
        [Comment("发票号")]
        public string InvoiceNo { get; set; }

        [Comment("发票日期")]
        public DateTime InvoiceDate { get; set; }

        [Comment("法定名称")]
        public string TenantLegalName { get; set; }

        [Comment("租户地址")]
        public string TenantAddress { get; set; }

        [Comment("租客号码")]
        public string TenantTaxNo { get; set; }
    }
}
