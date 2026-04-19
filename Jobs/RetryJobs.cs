using Microsoft.EntityFrameworkCore;

public class RetryJobs
{
    private readonly AppDbContext _db;
    private readonly ReconciliationService _reconciliationService;

    public RetryJobs(AppDbContext db, ReconciliationService reconciliationservices)
    {
        _db = db;
        _reconciliationService = reconciliationservices;
    }

    public async Task ExecuteAsync()
    {
           var records = await _db.ReconciliationRecords
      .Where(r => r.Status == ReconciliationStatus.RetryScheduled
    && r.NextRetryAt.HasValue 
    && r.NextRetryAt.Value <= DateTime.UtcNow)
        .ToListAsync();

    foreach(var record in records)
    {
        await _reconciliationService.ProcessAsync(record.Id);
    }
    }
}
