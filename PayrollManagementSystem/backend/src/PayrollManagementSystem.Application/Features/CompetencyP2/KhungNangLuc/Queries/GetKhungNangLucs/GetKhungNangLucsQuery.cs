using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.DTOs;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Queries.GetKhungNangLucs
{
    public class GetKhungNangLucsQuery : IRequest<Response<IEnumerable<KhungNangLucDto>>>
    {
        public string IdChucVu { get; set; } = null!;
    }
}
