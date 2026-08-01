using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Employees.Commands.ChangeEmployeeStatus
{
    public class ChangeEmployeeStatusCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public string CacheKeyPrefix => CacheKeyConstants.Departments;

        public string Cccd { get; set; } = null!;
        public TrangThaiNhanVien TrangThaiMoi { get; set; }
        public string LyDo { get; set; } = null!;

        public string NguoiThayDoi { get; set; } = null!;
    }
}