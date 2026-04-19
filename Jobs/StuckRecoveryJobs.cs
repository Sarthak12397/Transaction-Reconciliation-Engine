using Microsoft.EntityFrameworkCore;

public class StuckRecoveryJobs
{
        private readonly AppDbContext _db;

    public StuckRecoveryJobs(AppDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteAsync()
    {
       var records = await _db.ReconciliationRecords
      .Where(r => r.Status == ReconciliationStatus.Processing
    &&r.LastAttemptedAt <= DateTime.UtcNow.AddMinutes(-10))
        .ToListAsync();

    foreach(var record in records)
    {
         record.MarkAsRetryScheduled(DateTime.UtcNow.AddMinutes(5));
    }
             await _db.SaveChangesAsync();

    }

}