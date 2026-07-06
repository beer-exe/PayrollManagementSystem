using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.SubmitTuDanhGia
{
    public class ChiTietTuDanhGiaDto
    {
        public Guid IdChiTiet { get; set; }
        public int DiemTuDanhGia { get; set; }
        public string? NhanXetNhanVien { get; set; }
    }

    public class SubmitTuDanhGiaCommand : IRequest<Response<bool>>
    {
        public Guid IdPhieu { get; set; }
        public bool IsSubmit { get; set; } // true: Gửi quản lý, false: Lưu nháp
        public List<ChiTietTuDanhGiaDto> ChiTiets { get; set; } = new List<ChiTietTuDanhGiaDto>();
    }
}
