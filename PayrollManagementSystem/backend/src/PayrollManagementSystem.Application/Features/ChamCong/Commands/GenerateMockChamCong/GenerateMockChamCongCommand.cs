using MediatR;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.GenerateMockChamCong
{
    public class GenerateMockChamCongCommand : IRequest<FileDto>
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
    }

    public class FileDto
    {
        public byte[] Data { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }
}
