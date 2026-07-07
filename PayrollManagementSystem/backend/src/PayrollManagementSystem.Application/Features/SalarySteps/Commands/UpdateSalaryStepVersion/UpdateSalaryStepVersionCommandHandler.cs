using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Commands.UpdateSalaryStepVersion
{
    public class UpdateSalaryStepVersionCommandHandler : IRequestHandler<UpdateSalaryStepVersionCommand, Response<string>>
    {
        private readonly IApplicationDbContext _context;
        public UpdateSalaryStepVersionCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<string>> Handle(UpdateSalaryStepVersionCommand request, CancellationToken cancellationToken)
        {
            var currentActive = await _context.BacLuongs
                        .Where(x => x.IdNgachLuong == request.JobGradeId && x.TenBacLuong == request.StepName && x.TrangThai == TrangThaiBacLuong.HIEU_LUC)
                        .OrderByDescending(x => x.NgayApDung)
                        .FirstOrDefaultAsync(cancellationToken);

            if (currentActive == null) throw new Common.Exceptions.ApiException("Không tìm thấy dữ liệu hiện hành.");

            var newEffectiveDateOnly = DateOnly.FromDateTime(request.NewEffectiveDate);

            if (newEffectiveDateOnly <= currentActive.NgayApDung)
                throw new Common.Exceptions.ApiException("Ngày áp dụng mới phải lớn hơn ngày hiện hành.");

            currentActive.NgayKetThuc = newEffectiveDateOnly.AddDays(-1);
            currentActive.TrangThai = TrangThaiBacLuong.HET_HIEU_LUC;

            var newVersion = new BacLuong
            {
                IdBacLuong = Guid.NewGuid().ToString(),
                IdNgachLuong = request.JobGradeId,
                TenBacLuong = request.StepName,
                LuongP1 = request.NewP1Salary,
                NgayApDung = newEffectiveDateOnly,
                NgayKetThuc = null,
                TrangThai = TrangThaiBacLuong.HIEU_LUC
            };

            _context.BacLuongs.Add(newVersion);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<string>(newVersion.IdBacLuong, "Cập nhật phiên bản lương thành công.");
        }
    }
}
