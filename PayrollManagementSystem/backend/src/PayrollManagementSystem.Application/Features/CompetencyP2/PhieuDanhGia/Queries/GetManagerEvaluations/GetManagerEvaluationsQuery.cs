using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.DTOs;
using System.Collections.Generic;
using System;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Queries.GetManagerEvaluations
{
    public class GetManagerEvaluationsQuery : IRequest<Response<IEnumerable<PhieuDanhGiaDto>>>
    {
        public Guid TaiKhoanId { get; set; }
        public bool IsHr { get; set; }
    }
}
