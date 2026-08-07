using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Common.Constants;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.DeleteBacThue
{
    public class DeleteBacThueCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public Guid IdBacThue { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.BacThue;
    }
}
