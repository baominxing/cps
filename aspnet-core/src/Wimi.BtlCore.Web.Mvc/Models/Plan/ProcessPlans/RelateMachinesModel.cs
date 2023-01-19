namespace Wimi.BtlCore.Web.Models.Plan.ProcessPlans
{
    public class RelateMachinesModel
    {
        public RelateMachinesModel(int planId)
        {
            PlanId = planId;
        }

        public int PlanId { get; set; }
    }
}