using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.DeleteKhoanKhauTru
{
    public class DeleteKhoanKhauTruCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand, ITransactionalCommand
    {
        public Guid IdKhoanKhauTru { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.KhoanKhauTru;
    }
}
