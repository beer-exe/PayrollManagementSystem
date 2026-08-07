using MediatR;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.GenerateMockChamCong
{
    public class GenerateMockChamCongCommand : IRequest<FileDto>
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
    }
}
