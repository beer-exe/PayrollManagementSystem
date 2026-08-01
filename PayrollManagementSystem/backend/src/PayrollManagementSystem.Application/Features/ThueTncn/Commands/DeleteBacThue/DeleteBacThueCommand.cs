using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.DeleteBacThue
{
    public class DeleteBacThueCommand : IRequest<Response<bool>>
    {
        public Guid IdBacThue { get; set; }
    }
}
