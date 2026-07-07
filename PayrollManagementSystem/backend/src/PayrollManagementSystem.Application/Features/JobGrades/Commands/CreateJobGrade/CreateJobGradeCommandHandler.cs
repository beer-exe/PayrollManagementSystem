using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.JobGrades.Commands.CreateJobGrade
{
    public class CreateJobGradeCommandHandler : IRequestHandler<CreateJobGradeCommand, Response<string>>
    {
        private readonly IApplicationDbContext _context;

        public CreateJobGradeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<string>> Handle(CreateJobGradeCommand request, CancellationToken cancellationToken)
        {
            // Tự động sinh ID ngạch lương (hoặc tùy logic hệ thống)
            var count = _context.NgachLuongs.Count();
            var newId = $"NL{(count + 1).ToString("D3")}";

            // Hoặc có thể yêu cầu frontend gửi lên (nhưng theo command DTO thì frontend không gửi lên Id)
            // Tạm thời gen một ID ngẫu nhiên hoặc sequence. 
            // Ta dùng Guid nếu hệ thống thiết kế ID là chuỗi dài, hoặc dùng logic sinh ID tuần tự.
            // Để an toàn (nếu idNgachLuong là chuỗi tự do), ta dùng Guid.NewGuid() hoặc tự build logic.
            // Do trong DB trường IdNgachLuong là string, ta sinh random string nếu cần.
            var id = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

            var jobGrade = new NgachLuong
            {
                IdNgachLuong = id,
                TenNgachLuong = request.TenNgachLuong,
                MoTa = request.MoTa,
                TrangThai = PayrollManagementSystem.Domain.Enums.TrangThaiNgachLuong.HOAT_DONG
            };

            await _context.NgachLuongs.AddAsync(jobGrade, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<string>(jobGrade.IdNgachLuong, "Thêm mới ngạch lương thành công");
        }
    }
}
