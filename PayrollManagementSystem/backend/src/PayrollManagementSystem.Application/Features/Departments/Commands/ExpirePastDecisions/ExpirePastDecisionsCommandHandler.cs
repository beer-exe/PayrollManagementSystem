using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.ExpirePastDecisions
{
    public class ExpirePastDecisionsCommandHandler : IRequestHandler<ExpirePastDecisionsCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public ExpirePastDecisionsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ExpirePastDecisionsCommand request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Get all active decisions grouped by Cccd
            var activeDecisions = await _context.QuyetDinhNhanSus
                .Where(q => q.TrangThai == TrangThaiQuyetDinh.HIEU_LUC)
                .ToListAsync(cancellationToken);

            var groupedByEmployee = activeDecisions.GroupBy(q => q.Cccd).ToList();

            foreach (var group in groupedByEmployee)
            {
                // If an employee has multiple active decisions
                if (group.Count() > 1)
                {
                    // Sort them by NgayHieuLuc desc, CreatedAt desc
                    var sortedDecisions = group
                        .OrderByDescending(q => q.NgayHieuLuc)
                        .ThenByDescending(q => q.CreatedAt)
                        .ToList();

                    // Find the one that should be currently active
                    // (the latest one that has effective date <= today)
                    var trueCurrentDecision = sortedDecisions.FirstOrDefault(q => q.NgayHieuLuc <= today);

                    if (trueCurrentDecision != null)
                    {
                        // Any decision that has effective date <= today and is NOT the trueCurrentDecision, is expired
                        foreach (var decision in sortedDecisions)
                        {
                            if (decision.SoQuyetDinh != trueCurrentDecision.SoQuyetDinh && decision.NgayHieuLuc <= today)
                            {
                                decision.TrangThai = TrangThaiQuyetDinh.HET_HAN;
                                decision.NgayHetHan = trueCurrentDecision.NgayHieuLuc; // It expired when the new one took effect
                                _context.QuyetDinhNhanSus.Update(decision);
                            }
                        }
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
