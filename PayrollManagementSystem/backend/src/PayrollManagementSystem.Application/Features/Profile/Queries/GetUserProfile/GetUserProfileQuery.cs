using MediatR;
using PayrollManagementSystem.Application.Features.Profile.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Profile.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<Response<UserProfileDto>>
    {
        public Guid TaiKhoanId { get; set; }
    }
}