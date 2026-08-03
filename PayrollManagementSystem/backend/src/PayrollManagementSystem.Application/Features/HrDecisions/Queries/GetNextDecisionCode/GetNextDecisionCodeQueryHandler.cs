using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.HrDecisions.Queries.GetNextDecisionCode
{
    public class GetNextDecisionCodeQueryHandler : IRequestHandler<GetNextDecisionCodeQuery, string>
    {
        private readonly IApplicationDbContext _context;

        public GetNextDecisionCodeQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(GetNextDecisionCodeQuery request, CancellationToken cancellationToken)
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"QD{request.Type}-{year}-";

            var lastDecision = await _context.QuyetDinhNhanSus
                .Where(q => q.SoQuyetDinh.StartsWith(prefix))
                .OrderByDescending(q => q.SoQuyetDinh)
                .FirstOrDefaultAsync(cancellationToken);

            int nextSequence = 1;

            if (lastDecision != null)
            {
                var lastCode = lastDecision.SoQuyetDinh;
                // Ví dụ: QDTD-2026-000001 -> cắt 6 ký tự cuối
                var sequencePart = lastCode.Substring(prefix.Length);
                
                if (int.TryParse(sequencePart, out int parsedSequence))
                {
                    nextSequence = parsedSequence + 1;
                }
            }

            return $"{prefix}{nextSequence.ToString("D6")}";
        }
    }
}
