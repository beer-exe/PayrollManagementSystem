using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.DeleteBacThue
{
    public class DeleteBacThueCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public Guid IdBacThue { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.BacThue;
    }
}
