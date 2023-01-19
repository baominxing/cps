using System.Collections.Generic;
using Wimi.BtlCore.Cutter.Dto;

namespace Wimi.BtlCore.Web.Models.Common
{
    public interface ICutterTypeViewModal
    {
        List<CutterTypeDto> CutterTypes { get; set; }
    }
}
