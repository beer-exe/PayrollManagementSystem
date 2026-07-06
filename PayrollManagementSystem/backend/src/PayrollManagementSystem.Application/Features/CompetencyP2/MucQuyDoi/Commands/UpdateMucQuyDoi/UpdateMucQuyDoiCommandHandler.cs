using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Exceptions;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Commands.UpdateMucQuyDoi
{
    public class UpdateMucQuyDoiCommandHandler : IRequestHandler<UpdateMucQuyDoiCommand, Response<System.Guid>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateMucQuyDoiCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<System.Guid>> Handle(UpdateMucQuyDoiCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.MucQuyDoiP2s.FindAsync(new object[] { request.IdQuyDoi }, cancellationToken);
            if (entity == null)
            {
                throw new ApiException($"Không tìm thấy cấu hình với Id {request.IdQuyDoi}");
            }

            entity.XepLoai = request.XepLoai;
            entity.DiemToiThieu = request.DiemToiThieu;
            entity.DiemToiDa = request.DiemToiDa;
            entity.HeSoP2 = request.HeSoP2;

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<System.Guid>(entity.IdQuyDoi);
        }
    }
}
