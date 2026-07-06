using MediatR;

namespace PayrollManagementSystem.Application.Features.Employees.Queries.ExportEmployees
{
    public class ExportEmployeesQuery : IRequest<byte[]>
    {
        public string? SearchTerm { get; set; }
        public string? IdPb { get; set; }
    }
}
