using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.ThueTncn.DTOs;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetBacThueList
{
    public class GetBacThueListQuery : IRequest<Response<List<BacThueDto>>> { }
}
