using MediatR;
using PayrollManagementSystem.Application.Features.DonNghi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Queries.GetMyNgayPhep
{
    public class GetMyNgayPhepQuery : IRequest<Response<NgayPhepDto?>>
    {
        public Guid UserId { get; set; }
        public int Nam { get; set; }
    }
}
