using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ReconciliationRecord> ReconciliationRecords { get; set; }
    public DbSet<InternalRecord> InternalRecords { get; set; }
    public DbSet<ExternalRecord> ExternalRecords { get; set; }
    public DbSet<ReconciliationAuditLog> ReconciliationAuditLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            modelBuilder.Entity<InternalRecord>()
        .HasIndex(r => r.TransactionId)
        .IsUnique();

    modelBuilder.Entity<ReconciliationRecord>()
        .HasIndex(r => r.Status);

    modelBuilder.Entity<ReconciliationRecord>()
        .HasIndex(r => r.NextRetryAt);

    }
}