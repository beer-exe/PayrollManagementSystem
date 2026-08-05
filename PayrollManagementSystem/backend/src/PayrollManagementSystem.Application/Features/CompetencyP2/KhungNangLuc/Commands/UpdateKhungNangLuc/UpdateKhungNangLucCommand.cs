using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Common.Constants;
using System;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.UpdateKhungNangLuc
{
    public class UpdateKhungNangLucCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public Guid IdTieuChi { get; set; }
        public string TenNangLuc { get; set; } = null!;
        public string? MoTa { get; set; }
        public decimal TyTrong { get; set; }
        
        public string CacheKeyPrefix => CacheKeyConstants.KhungNangLuc;
    }
}
