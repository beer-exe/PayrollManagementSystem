using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Common.Constants;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.CreateBacThue
{
    public class CreateBacThueCommand : IRequest<Response<Guid>>, ICacheInvalidatorCommand
    {
        public int Bac { get; set; }
        public decimal TuGia { get; set; }
        public decimal? DenGia { get; set; }
        public decimal ThueSuat { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.BacThue;
    }
}
