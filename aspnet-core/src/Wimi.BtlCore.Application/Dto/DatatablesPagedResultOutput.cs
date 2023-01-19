namespace Wimi.BtlCore.Dto
{
    using System;
    using System.Collections.Generic;

    using Abp.Application.Services.Dto;

    [Serializable]
    public class DatatablesPagedResultOutput<T> : PagedResultDto<T>
    {
        public DatatablesPagedResultOutput()
        {
        }
        
        public DatatablesPagedResultOutput(int filteredCount, IReadOnlyList<T> items, int? totalCount)
            : this(items)
        {
            this.RecordsFiltered = filteredCount;
            this.TotalCount = totalCount ?? this.RecordsFiltered;
        }
        
        public DatatablesPagedResultOutput(int filteredCount, IReadOnlyList<T> items, int? totalCount, int draw = 1)
            : this(filteredCount, items, totalCount)
        {
            this.Draw = draw;
        }
        
        public DatatablesPagedResultOutput(int filteredCount, IReadOnlyList<T> items, IDrawCounter draw)
            : this(items)
        {
            this.RecordsFiltered = filteredCount;
            this.Draw = draw.Draw;
        }
        
        public DatatablesPagedResultOutput(int totalCount, IReadOnlyList<T> items)
            : base(totalCount, items)
        {
            this.RecordsFiltered = totalCount;
        }

        protected DatatablesPagedResultOutput(IReadOnlyList<T> items)
        {
            this.Items = items;
        }

        /// <summary>
        ///     绘制计数器。这个是用来确保Ajax从服务器返回的是对应的（Ajax是异步的，因此返回的顺序是不确定的）。 要求在服务器接收到此参数后再返回
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        ///     必要。过滤后的记录数（如果有接收到前台的过滤条件，则返回的是过滤后的记录数）
        /// </summary>
        public int RecordsFiltered { get; set; }

        public int RecordsTotal => this.TotalCount;
    }
}