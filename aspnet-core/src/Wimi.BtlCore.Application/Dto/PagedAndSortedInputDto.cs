namespace Wimi.BtlCore.Dto
{
    using System.Linq;

    using Abp.Application.Services.Dto;
    using Abp.Extensions;

    public class PagedAndSortedInputDto : DatatablesPagedInputDto, ISortedResultRequest
    {
        private string sorting;

        public DatatablesColumnsInputDto[] Columns { get; set; }

        public DatatablesSortedInputDto[] Order { get; set; }

        // Sorting field for manual setting sorting,
        // datatables, either single or multi sorting, just via array of order and columns
        public string Sorting
        {
            get
            {
                if (!this.sorting.IsNullOrEmpty())
                {
                    return this.sorting;
                }

                if (this.Order == null || this.Order.Length == 0)
                {
                    return string.Empty;
                }

                var sortringList =
                    this.Order.Select(orderItem => $"{this.Columns[orderItem.Column].Data} {orderItem.Dir.ToUpper()}")
                        .ToArray();

                return string.Join(",", sortringList);
            }

            set
            {
                this.sorting = value;
            }
        }
    }
}