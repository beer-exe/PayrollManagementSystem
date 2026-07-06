using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.DTOs;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Queries.GetKyDanhGias
{
    public class GetKyDanhGiasQuery : IRequest<Response<IEnumerable<KyDanhGiaDto>>>
    {
    }
}
