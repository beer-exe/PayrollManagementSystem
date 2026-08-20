using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Queries.GetCaLamViecTrongNgay
{
    public class GetCaLamViecTrongNgayQuery : IRequest<Response<CaLamViecTrongNgayDto>>
    {
        public string Cccd { get; set; } = null!;
        public DateOnly Ngay { get; set; }
    }
}
