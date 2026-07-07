using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.DeleteKyDanhGia
{
    public class DeleteKyDanhGiaCommand : IRequest<Response<bool>>
    {
        public Guid IdKyDanhGia { get; set; }
    }
}
