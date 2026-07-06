using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Commands.CreateMucQuyDoi
{
    public class CreateMucQuyDoiCommand : IRequest<Response<System.Guid>>
    {
        public string XepLoai { get; set; } = null!;
        public decimal DiemToiThieu { get; set; }
        public decimal DiemToiDa { get; set; }
        public decimal HeSoP2 { get; set; }
    }
}
