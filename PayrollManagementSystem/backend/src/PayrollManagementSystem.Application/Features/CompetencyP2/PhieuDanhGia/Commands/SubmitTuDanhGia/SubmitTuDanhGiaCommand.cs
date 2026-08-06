using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.SubmitTuDanhGia
{
    public class SubmitTuDanhGiaCommand : IRequest<Response<bool>>
    {
        public Guid IdPhieu { get; set; }
        public bool IsSubmit { get; set; }
        public List<ChiTietTuDanhGiaDto> ChiTiets { get; set; } = new List<ChiTietTuDanhGiaDto>();
    }
}
