using MediatR;
using PayrollManagementSystem.Application.Features.DonNghi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Queries.GetMyDonNghiList
{
    public class GetMyDonNghiListQuery : IRequest<Response<List<DonNghiDto>>>
    {
        public Guid UserId { get; set; }
        public int? Thang { get; set; }
        public int? Nam { get; set; }
        public string? TrangThai { get; set; }
        public string? LoaiNghi { get; set; }
    }
}
