using PayrollManagementSystem.Application.Features.Kpi.DTOs;

namespace PayrollManagementSystem.API.DTOs
{
    public class AssignKpiRequestDto
    {
        public List<ChiTietKpiInput> ChiTietKpis { get; set; } = new();
    }
}
