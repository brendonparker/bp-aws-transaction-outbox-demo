using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using TransactionalOutboxPatternApp.Domain;

namespace TransactionalOutboxPatternApp.Infrastructure;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> dbContextOptions,
    IOptions<DatabaseConfig> dbConfigOptions) : DbContext(dbContextOptions)
{
    public DbSet<TransactionOutbox> TransactionOutbox => Set<TransactionOutbox>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        NpgsqlConnectionStringBuilder connectionStringBuilder = new()
        {
            Database = dbConfigOptions.Value.DbName,
            Username = dbConfigOptions.Value.Username,
            Host = dbConfigOptions.Value.Host,
            Password = dbConfigOptions.Value.Password
        };
        var connString = connectionStringBuilder.ToString();
        optionsBuilder.UseNpgsql(connString)
            .UseSnakeCaseNamingConvention()
            .EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionOutbox>(entity =>
        {
            entity.ToTable("transaction_outbox");
            entity.HasKey(x => x.Id);

            entity.Property(e => e.IsProcessed)
                .IsRequired();

            entity.Property(e => e.JsonContent)
                .IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("order");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Status)
                .IsRequired();
        });
    }
}