using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.DeleteKhungNangLuc
{
    public class DeleteKhungNangLucCommand : IRequest<Response<bool>>
    {
        public Guid IdTieuChi { get; set; }
    }
}
