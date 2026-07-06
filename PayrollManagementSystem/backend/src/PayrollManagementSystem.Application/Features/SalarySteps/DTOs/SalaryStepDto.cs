namespace PayrollManagementSystem.Application.Features.SalarySteps.DTOs
{
    public class SalaryStepDto
    {
        public string Id { get; set; } = null!;
        public string StepName { get; set; } = null!;
        public decimal P1Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
