using MediatR;
using Microsoft.Extensions.Logging;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Behaviours;

public class TransactionBehavior<TRequest, TResponse>(IApplicationDbContext context,ILogger<TransactionBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ITransactionalCommand)
            return await next();

        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Begin Transaction for {RequestName}", requestName);

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next();
            await transaction.CommitAsync(cancellationToken);
            
            logger.LogInformation("Commit Transaction for {RequestName}", requestName);
            
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Rollback Transaction for {RequestName} due to error: {Message}", requestName, ex.Message);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}