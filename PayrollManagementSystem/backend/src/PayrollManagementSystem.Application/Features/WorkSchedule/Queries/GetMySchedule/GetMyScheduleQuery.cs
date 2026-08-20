using MediatR;
using PayrollManagementSystem.Application.Features.WorkSchedule.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetMySchedule
{
    public class GetMyScheduleQuery : IRequest<Response<IEnumerable<MyScheduleDayDto>>>
    {
        public Guid UserId { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
    }
}
