using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.ThueTncn.DTOs;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetCauHinhGiamTru
{
    public class GetCauHinhGiamTruQuery : IRequest<Response<CauHinhGiamTruDto>> { }
}
