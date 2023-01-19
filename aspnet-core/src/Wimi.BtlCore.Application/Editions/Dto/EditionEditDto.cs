using System.ComponentModel.DataAnnotations;

using Abp.Application.Editions;
using Abp.AutoMapper;

namespace Wimi.BtlCore.Editions.Dto
{
    [AutoMap(typeof(Edition))]
    public class EditionEditDto
    {
        [Required]
        public string DisplayName { get; set; }

        public int? Id { get; set; }
    }
}