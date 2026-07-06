using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(IApplicationDbContext context)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!request.GetType().Name.EndsWith("Command"))
            return await next();

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next();
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return response;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}