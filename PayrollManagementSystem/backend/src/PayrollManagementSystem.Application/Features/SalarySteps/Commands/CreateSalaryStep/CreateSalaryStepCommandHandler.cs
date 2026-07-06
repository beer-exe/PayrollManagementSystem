using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Commands.CreateSalaryStep
{
    public class CreateSalaryStepCommandHandler : IRequestHandler<CreateSalaryStepCommand, Response<string>>
    {
        private readonly IApplicationDbContext _context;
        public CreateSalaryStepCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<string>> Handle(CreateSalaryStepCommand request, CancellationToken cancellationToken)
        {
            bool exists = await _context.BacLuongs.AnyAsync(x => x.IdChucVu == request.PositionId && x.TenBacLuong == request.StepName, cancellationToken);
            if (exists) throw new Common.Exceptions.ApiException($"Bậc lương '{request.StepName}' đã tồn tại.");

            var newStep = new Domain.Models.BacLuong
            {
                IdBacLuong = Guid.NewGuid().ToString(),
                IdChucVu = request.PositionId,
                TenBacLuong = request.StepName,
                LuongP1 = request.P1Salary,
                NgayApDung = DateOnly.FromDateTime(request.EffectiveDate.Date),
                NgayKetThuc = null,
                TrangThai = Domain.Enums.TrangThaiBacLuong.HIEU_LUC
            };

            _context.BacLuongs.Add(newStep);
            await _context.SaveChangesAsync(cancellationToken);
            return new Response<string>(newStep.IdBacLuong, "Thêm mới bậc lương thành công.");
        }
    }
}
