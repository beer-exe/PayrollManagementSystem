namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.GenerateMockChamCong
{
    public class FileDto
    {
        public byte[] Data { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }
}
