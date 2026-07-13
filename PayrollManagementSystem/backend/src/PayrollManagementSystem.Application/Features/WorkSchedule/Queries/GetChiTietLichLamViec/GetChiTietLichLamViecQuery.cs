using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.WorkSchedule.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetChiTietLichLamViec
{
    public class GetChiTietLichLamViecQuery : IRequest<PagedResponse<List<ChiTietLichLamViecDto>>>
    {
        public Guid IdLich { get; set; }
        public int Thang { get; set; } = 1;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 31;
    }
}
