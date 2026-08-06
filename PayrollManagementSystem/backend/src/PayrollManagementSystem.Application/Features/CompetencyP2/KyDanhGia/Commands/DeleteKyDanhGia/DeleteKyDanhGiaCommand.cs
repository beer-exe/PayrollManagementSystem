using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Common.Constants;
using System;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.DeleteKyDanhGia
{
    public class DeleteKyDanhGiaCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public Guid IdKyDanhGia { get; set; }
        
        public string CacheKeyPrefix => CacheKeyConstants.KyDanhGia;
    }
}
