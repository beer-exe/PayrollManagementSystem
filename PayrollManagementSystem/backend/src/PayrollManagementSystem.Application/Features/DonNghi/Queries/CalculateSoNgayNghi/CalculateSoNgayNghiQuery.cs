using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Queries.CalculateSoNgayNghi
{
    public class CalculateSoNgayNghiQuery : IRequest<Response<decimal>>
    {
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public string LoaiNghi { get; set; } = null!;
    }
}
