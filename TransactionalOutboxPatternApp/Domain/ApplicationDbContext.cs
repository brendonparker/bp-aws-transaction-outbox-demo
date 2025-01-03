using Microsoft.EntityFrameworkCore;

namespace TransactionalOutboxPatternApp.Domain;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<TransactionOutbox> TransactionOutbox => Set<TransactionOutbox>();
    public DbSet<Order> Orders => Set<Order>();

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

            entity.Property(e => e.Status)
                .IsRequired();
        });
    }
}