using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using System;
using System.Collections.Generic;

namespace PayrollManagementSystem.Application.Features.WorkShifts.Commands.UpdateCaLamViec
{
    public class UpdateCaLamViecCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid Id { get; set; }
        public string TenCa { get; set; } = null!;
        public string GioBatDau { get; set; } = null!;
        public string GioKetThuc { get; set; } = null!;
        public bool XuyenNgay { get; set; }
        public decimal HeSoLuong { get; set; }
        public bool TrangThai { get; set; }

        public List<UpdateKhungGioNghiCommand> KhungGioNghis { get; set; } = new();

        public string CacheKeyPrefix => CacheKeyConstants.CaLamViec;
    }
}
