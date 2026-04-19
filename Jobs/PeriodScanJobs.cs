using Microsoft.EntityFrameworkCore;

public class PeriodScanJobs
{
    private readonly AppDbContext _db;
    private readonly ReconciliationService _reconciliationService;

    public PeriodScanJobs(AppDbContext db, ReconciliationService reconciliationservices)
    {
        _db = db;
        _reconciliationService = reconciliationservices;
    }

    public async Task ExecuteAsync()
    {
           var records = await _db.ReconciliationRecords
        .Where(r => r.Status == ReconciliationStatus.Pending
            && r.NextRetryAt <= DateTime.UtcNow )
        .ToListAsync();

    foreach(var record in records)
    {
        await _reconciliationService.ProcessAsync(record.Id);
    }
    }
}
