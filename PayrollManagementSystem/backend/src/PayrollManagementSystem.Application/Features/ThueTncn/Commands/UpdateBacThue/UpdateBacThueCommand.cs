using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpdateBacThue
{
    public class UpdateBacThueCommand : IRequest<Response<bool>>
    {
        public Guid IdBacThue { get; set; }
        public decimal TuGia { get; set; }
        public decimal? DenGia { get; set; }
        public decimal ThueSuat { get; set; }
        public bool IsActive { get; set; }
    }
}
