using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.ChangeStatusKyDanhGia
{
    public class NotifyEvaluationOpenedCommandHandler : IRequestHandler<NotifyEvaluationOpenedCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public NotifyEvaluationOpenedCommandHandler(IApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> Handle(NotifyEvaluationOpenedCommand request, CancellationToken cancellationToken)
        {
            var kyDanhGia = await _context.KyDanhGias.FindAsync(new object[] { request.IdKyDanhGia }, cancellationToken);
            if (kyDanhGia == null) return false;

            var nhanViens = await _context.NhanViens
                .Where(x => x.TrangThai == TrangThaiNhanVien.DANG_LAM_VIEC && !string.IsNullOrEmpty(x.Email))
                .ToListAsync(cancellationToken);

            foreach (var nv in nhanViens)
            {
                var subject = $"Thông báo mở kỳ đánh giá năng lực: {kyDanhGia.TenKyDanhGia}";
                var body = $@"
                    <p>Xin chào {nv.HoTen},</p>
                    <p>Phòng Nhân sự xin thông báo kỳ đánh giá năng lực <b>{kyDanhGia.TenKyDanhGia}</b> đã chính thức mở.</p>
                    <p>Thời gian đánh giá: Từ {kyDanhGia.NgayBatDau:dd/MM/yyyy} đến {kyDanhGia.NgayKetThuc:dd/MM/yyyy}.</p>
                    <p>Vui lòng đăng nhập vào hệ thống để tiến hành tự đánh giá đúng hạn.</p>
                    <p>Trân trọng,</p>
                    <p>Phòng Nhân Sự</p>
                ";
                
                await _emailService.SendAsync(nv.Email!, subject, body);
            }

            return true;
        }
    }
}
