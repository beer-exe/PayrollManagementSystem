using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.CreateKhungNangLuc
{
    public class CreateKhungNangLucCommand : IRequest<Response<Guid>>, ICacheInvalidatorCommand
    {
        public string IdChucVu { get; set; } = null!;
        public string TenNangLuc { get; set; } = null!;
        public string? MoTa { get; set; }
        public decimal TyTrong { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.KhungNangLuc;
    }
}
