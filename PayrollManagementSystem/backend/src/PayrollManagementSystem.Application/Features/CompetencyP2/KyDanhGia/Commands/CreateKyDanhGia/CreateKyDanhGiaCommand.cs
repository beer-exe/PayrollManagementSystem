using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.CreateKyDanhGia
{
    public class CreateKyDanhGiaCommand : IRequest<Response<Guid>>
    {
        public string TenKyDanhGia { get; set; } = null!;
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
    }
}
