using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.SubmitManagerEvaluation
{
    public class SubmitManagerEvaluationCommand : IRequest<Response<bool>>, ITransactionalCommand
    {
        public Guid TaiKhoanId { get; set; }
        public bool IsHr { get; set; }
        public Guid IdPhieu { get; set; }
        public bool IsSubmit { get; set; }
        public string? NhanXetChung { get; set; }
        public List<ChiTietManagerEvaluationDto> ChiTiets { get; set; } = new List<ChiTietManagerEvaluationDto>();
    }
}
