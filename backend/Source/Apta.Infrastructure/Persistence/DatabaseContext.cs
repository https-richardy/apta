namespace Apta.Infrastructure.Persistence;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Administrator> Administrators { get; private set; }
    public DbSet<Customer> Customers { get; private set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* https://learn.microsoft.com/en-us/ef/core/modeling/ */
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
