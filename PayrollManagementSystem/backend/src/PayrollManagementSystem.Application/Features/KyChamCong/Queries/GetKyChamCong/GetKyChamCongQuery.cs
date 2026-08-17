using MediatR;
using PayrollManagementSystem.Application.Features.KyChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.KyChamCong.Queries.GetKyChamCong
{
    public class GetKyChamCongQuery : IRequest<Response<KyChamCongDto>>
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
    }
}
