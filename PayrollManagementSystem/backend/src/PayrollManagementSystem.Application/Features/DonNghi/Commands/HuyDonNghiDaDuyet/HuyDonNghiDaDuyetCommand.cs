using MediatR;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.HuyDonNghiDaDuyet
{
    public class HuyDonNghiDaDuyetCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid Id { get; set; }
        
        public string CacheKeyPrefix => "DonNghi,NgayPhep";
    }
}
