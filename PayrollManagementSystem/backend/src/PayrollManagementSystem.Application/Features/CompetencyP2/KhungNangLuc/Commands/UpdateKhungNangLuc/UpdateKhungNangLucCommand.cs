using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.UpdateKhungNangLuc
{
    public class UpdateKhungNangLucCommand : IRequest<Response<bool>>
    {
        public Guid IdTieuChi { get; set; }
        public string TenNangLuc { get; set; } = null!;
        public string? MoTa { get; set; }
        public decimal TyTrong { get; set; }
    }
}
