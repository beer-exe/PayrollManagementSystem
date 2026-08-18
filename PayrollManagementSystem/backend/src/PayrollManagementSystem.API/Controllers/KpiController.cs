using PayrollManagementSystem.API.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Kpi.Commands.ApprovePhieuKpi;
using PayrollManagementSystem.Application.Features.Kpi.Commands.AssignPhieuKpi;
using PayrollManagementSystem.Application.Features.Kpi.Commands.CreateKyKpi;
using PayrollManagementSystem.Application.Features.Kpi.Commands.SaveChiTietKpi;
using PayrollManagementSystem.Application.Features.Kpi.Commands.SubmitPhieuKpi;
using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using PayrollManagementSystem.Application.Features.Kpi.Queries.GetChiTietPhieuKpi;
using PayrollManagementSystem.Application.Features.Kpi.Queries.GetKyKpiList;
using PayrollManagementSystem.Application.Features.Kpi.Queries.GetPhieuKpiByKyKpi;
using PayrollManagementSystem.Application.Features.Kpi.Queries.GetPhieuKpiByNhanVien;

namespace PayrollManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KpiController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public KpiController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("ky-kpi")]
        public async Task<IActionResult> GetKyKpis()
        {
            var result = await _mediator.Send(new GetKyKpiListQuery());
            return Ok(result);
        }

        [HttpPost("ky-kpi")]
        public async Task<IActionResult> CreateKyKpi([FromBody] CreateKyKpiCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("nhan-vien/{taiKhoanId}")]
        public async Task<IActionResult> GetPhieuKpis(Guid taiKhoanId)
        {
            var result = await _mediator.Send(new GetPhieuKpiByNhanVienQuery { TaiKhoanId = taiKhoanId });
            return Ok(result);
        }

        [HttpGet("ky-kpi/{idKyKpi}/phieu")]
        public async Task<IActionResult> GetPhieuKpiByKy(Guid idKyKpi)
        {
            var result = await _mediator.Send(new GetPhieuKpiByKyKpiQuery { 
                IdKyKpi = idKyKpi,
                CurrentUserId = _currentUserService.UserId
            });
            return Ok(result);
        }

        [HttpGet("phieu/{id}")]
        public async Task<IActionResult> GetChiTietPhieuKpi(Guid id)
        {
            var result = await _mediator.Send(new GetChiTietPhieuKpiQuery { 
                IdPhieuKpi = id,
                CurrentUserId = _currentUserService.UserId 
            });
            return Ok(result);
        }

        [HttpPost("phieu/{id}/assign")]
        public async Task<IActionResult> AssignPhieuKpi(Guid id, [FromBody] AssignKpiRequestDto request)
        {
            if (_currentUserService.UserId == null) return Unauthorized();
            
            var command = new AssignPhieuKpiCommand { IdPhieuKpi = id, TaiKhoanIdQuanLy = _currentUserService.UserId.Value, ChiTietKpis = request.ChiTietKpis };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("phieu/{id}/confirm")]
        public async Task<IActionResult> ConfirmPhieuKpi(Guid id)
        {
            if (_currentUserService.UserId == null) return Unauthorized();
            
            var command = new PayrollManagementSystem.Application.Features.Kpi.Commands.ConfirmPhieuKpi.ConfirmPhieuKpiCommand 
            { 
                IdPhieuKpi = id, 
                TaiKhoanIdNhanVien = _currentUserService.UserId.Value 
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("phieu/{id}/chi-tiet")]
        public async Task<IActionResult> SaveChiTietKpi(Guid id, [FromBody] List<ChiTietKpiInput> chiTietKpis)
        {
            var command = new SaveChiTietKpiCommand { IdPhieuKpi = id, ChiTietKpis = chiTietKpis };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("phieu/{id}/submit")]
        public async Task<IActionResult> SubmitPhieuKpi(Guid id)
        {
            var command = new SubmitPhieuKpiCommand { IdPhieuKpi = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("phieu/{id}/approve")]
        public async Task<IActionResult> ApprovePhieuKpi(Guid id, [FromBody] ApproveKpiRequestDto request)
        {
            if (_currentUserService.UserId == null) return Unauthorized();

            var command = new ApprovePhieuKpiCommand { IdPhieuKpi = id, TaiKhoanIdQuanLy = _currentUserService.UserId.Value, NhanXet = request.NhanXet };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
