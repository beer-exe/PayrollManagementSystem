using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<Response<Guid>>, ITransactionalCommand
    {
        public string TenTaiKhoan { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public Guid IdVaiTro { get; set; }
        public string Cccd { get; set; } = null!;
    }
}
