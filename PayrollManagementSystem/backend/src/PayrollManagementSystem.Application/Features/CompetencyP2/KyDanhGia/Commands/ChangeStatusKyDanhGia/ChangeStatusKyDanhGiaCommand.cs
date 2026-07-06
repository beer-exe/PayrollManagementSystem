using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using System;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.ChangeStatusKyDanhGia
{
    public class ChangeStatusKyDanhGiaCommand : IRequest<Response<bool>>
    {
        public Guid IdKyDanhGia { get; set; }
        public TrangThaiKyDanhGia TrangThaiMoi { get; set; }
        public bool Force { get; set; } = false;
    }
}
