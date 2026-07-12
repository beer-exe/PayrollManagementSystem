using PayrollManagementSystem.Domain.Common;

namespace PayrollManagementSystem.Application.Common.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Explicitly filter out soft-deleted entities.
        /// Note: A Global Query Filter is already applied in DbContext, so this is just for explicitness in the code as requested.
        /// </summary>
        public static IQueryable<T> WhereNotDeleted<T>(this IQueryable<T> query) where T : BaseAuditableEntity
        {
            return query.Where(x => !x.IsDeleted);
        }
    }
}
