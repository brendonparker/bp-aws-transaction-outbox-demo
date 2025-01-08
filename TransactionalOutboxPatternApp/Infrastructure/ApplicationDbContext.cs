using System.Text.Json;
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

            entity.Property(e => e.EntityType)
                .IsRequired();
            entity.Property(e => e.EntityId)
                .IsRequired();
            entity.Property(e => e.EventType)
                .IsRequired();
            entity.Property(e => e.JsonContent)
                .HasColumnType("jsonb")
                .IsRequired();
            entity.Property(e => e.IsProcessed)
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var result = 0;
        await using var tx = await Database.BeginTransactionAsync(cancellationToken);

        result = await base.SaveChangesAsync(cancellationToken);

        var transactionOutboxRecords = ChangeTracker
            .Entries<AggregateRoot>()
            .SelectMany(
                entry => entry.Entity.GetIntegrationEvents(),
                (entry, evnt) => new TransactionOutbox
                {
                    Id = 0,
                    EntityType = entry.Entity.GetType().FullName!,
                    EntityId = entry.Entity.Id,
                    EventType = evnt.GetType().FullName!,
                    JsonContent = JsonSerializer.Serialize(evnt, evnt.GetType()),
                    CreatedAt = DateTime.UtcNow
                })
            .ToArray();

        if (transactionOutboxRecords.Length != 0)
        {
            TransactionOutbox.AddRange(transactionOutboxRecords);
            result += await base.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
        return result;
    }
}