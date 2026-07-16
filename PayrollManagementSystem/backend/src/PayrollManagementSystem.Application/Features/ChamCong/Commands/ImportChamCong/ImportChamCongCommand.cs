using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.ImportChamCong
{
    /// <summary>
    /// Import hàng loạt chấm công từ file CSV.
    /// Định dạng CSV chuẩn (có header row):
    ///   CCCD, NgayChamCong (dd/MM/yyyy), GioVao (HH:mm), GioRa (HH:mm), GhiChu
    /// </summary>
    public class ImportChamCongCommand : IRequest<Response<ImportChamCongResultDto>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = null!;

        public string CacheKeyPrefix => "ChamCong_";
    }
}
