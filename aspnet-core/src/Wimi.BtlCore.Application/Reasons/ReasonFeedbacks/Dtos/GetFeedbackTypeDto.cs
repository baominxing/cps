using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Reasons.ReasonFeedbacks.Dtos
{
    using Abp.Application.Services.Dto;

    public class GetFeedbackTypeDto : EntityDto
    {
        public int StateId { get; set; }

        public string StateCode { get; set; }

        public string StateDisplayName { get; set; }
    }
}
