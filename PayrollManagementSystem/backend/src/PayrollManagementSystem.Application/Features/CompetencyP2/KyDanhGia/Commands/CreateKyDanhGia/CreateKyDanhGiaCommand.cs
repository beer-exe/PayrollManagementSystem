using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Common.Constants;
using System;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.CreateKyDanhGia
{
    public class CreateKyDanhGiaCommand : IRequest<Response<Guid>>, ICacheInvalidatorCommand
    {
        public string TenKyDanhGia { get; set; } = null!;
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        
        public string CacheKeyPrefix => CacheKeyConstants.KyDanhGia;
    }
}
