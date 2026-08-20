using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PayrollManagementSystem.Infrastructure.Persistence;

namespace PayrollManagementSystem.UnitTests.Mocks
{
    public static class MockDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new ApplicationDbContext(options);

            context.Database.EnsureCreated();
            return context;
        }

        public static void Destroy(ApplicationDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
