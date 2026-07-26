using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Features.PhanCongCas.DTOs;
using System;
using System.Collections.Generic;

namespace PayrollManagementSystem.Application.Features.PhanCongCas.Queries.GetPhanCongCaByDateRange
{
    public class GetPhanCongCaByDateRangeQuery : IRequest<Response<IEnumerable<PhanCongCaDto>>>
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? IdPhongBan { get; set; }
    }
}
