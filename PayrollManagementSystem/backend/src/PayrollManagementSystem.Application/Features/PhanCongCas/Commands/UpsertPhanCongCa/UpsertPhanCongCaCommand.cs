using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using System;

namespace PayrollManagementSystem.Application.Features.PhanCongCas.Commands.UpsertPhanCongCa
{
    public class UpsertPhanCongCaCommand : IRequest<Response<bool>>
    {
        public string CccdNhanVien { get; set; } = null!;
        public DateOnly NgayLamViec { get; set; }
        public Guid? IdCaLamViec { get; set; }
        public string? GhiChu { get; set; }
    }
}
