using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.SubmitManagerEvaluation
{
    public class ChiTietManagerEvaluationDto
    {
        public Guid IdChiTiet { get; set; }
        public int DiemQuanLyDanhGia { get; set; }
        public string? NhanXetQuanLy { get; set; }
    }

    public class SubmitManagerEvaluationCommand : IRequest<Response<bool>>, ITransactionalCommand
    {
        public Guid TaiKhoanId { get; set; }
        public bool IsHr { get; set; }
        public Guid IdPhieu { get; set; }
        public bool IsSubmit { get; set; } // true: Chốt, false: Lưu nháp
        public string? NhanXetChung { get; set; }
        public List<ChiTietManagerEvaluationDto> ChiTiets { get; set; } = new List<ChiTietManagerEvaluationDto>();
    }
}
