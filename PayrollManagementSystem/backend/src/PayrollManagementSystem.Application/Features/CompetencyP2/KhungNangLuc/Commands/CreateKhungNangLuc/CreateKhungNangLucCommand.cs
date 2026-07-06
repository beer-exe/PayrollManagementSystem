using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.CreateKhungNangLuc
{
    public class CreateKhungNangLucCommand : IRequest<Response<Guid>>
    {
        public string IdChucVu { get; set; } = null!;
        public string TenNangLuc { get; set; } = null!;
        public string YeuCauToiThieu { get; set; } = null!;
        public decimal TyTrong { get; set; }
    }
}
