using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.CreateBacThue
{
    public class CreateBacThueCommand : IRequest<Response<Guid>>
    {
        public int Bac { get; set; }
        public decimal TuGia { get; set; }
        public decimal? DenGia { get; set; }
        public decimal ThueSuat { get; set; }
    }
}
