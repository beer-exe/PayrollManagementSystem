namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.SubmitManagerEvaluation
{
    public class ChiTietManagerEvaluationDto
    {
        public Guid IdChiTiet { get; set; }
        public int DiemQuanLyDanhGia { get; set; }
        public string? NhanXetQuanLy { get; set; }
    }
}
