using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Commands.CreateMucQuyDoi
{
    public class CreateMucQuyDoiCommandHandler : IRequestHandler<CreateMucQuyDoiCommand, Response<System.Guid>>
    {
        private readonly IApplicationDbContext _context;
        public CreateMucQuyDoiCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<System.Guid>> Handle(CreateMucQuyDoiCommand request, CancellationToken cancellationToken)
        {
            var entity = new Domain.Models.MucQuyDoiP2
            {
                XepLoai = request.XepLoai,
                DiemToiThieu = request.DiemToiThieu,
                DiemToiDa = request.DiemToiDa,
                HeSoP2 = request.HeSoP2
            };
            _context.MucQuyDoiP2s.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return new Response<System.Guid>(entity.IdQuyDoi, "Thêm mới thành công.");
        }
    }
}
