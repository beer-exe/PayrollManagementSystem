using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.CreatePosition
{
    public class CreatePositionCommandHandler : IRequestHandler<CreatePositionCommand, Response<string>>
    {
        private readonly IApplicationDbContext _context;
        public CreatePositionCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<string>> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
        {
            if (await _context.ChucVus.AnyAsync(cv => cv.IdChucVu == request.IdChucVu, cancellationToken))
                throw new Common.Exceptions.ApiException($"Mã chức vụ '{request.IdChucVu}' đã tồn tại.");

            var chucVu = new Domain.Models.ChucVu
            {
                IdChucVu = request.IdChucVu,
                TenChucVu = request.TenChucVu,
                MoTaCongViec = request.MoTaCongViec,
                IdNgachLuong = request.IdNgachLuong,
                IdPhongBan = request.IdPhongBan,
                IdChucVuQuanLy = request.IdChucVuQuanLy
            };

            _context.ChucVus.Add(chucVu);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<string>(chucVu.IdChucVu, "Thêm mới chức vụ thành công.");
        }
    }
}
