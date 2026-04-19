using Microsoft.EntityFrameworkCore;

public class ReconciliationService
{
    private readonly AppDbContext _db;
    private readonly RecordComparator _comparator;
    private readonly IExternalSystemClient _externalClient;
    private readonly ILogger<ReconciliationService> _logger;

    public ReconciliationService(
        AppDbContext db,
        RecordComparator comparator,
        IExternalSystemClient externalClient,
        ILogger<ReconciliationService> logger)
    {
        _db = db;
        _comparator = comparator;
        _externalClient = externalClient;
        _logger = logger;
    }

    public async Task ProcessAsync(Guid reconciliationRecordId)
    {
        var updated = await _db.ReconciliationRecords
            .Where(r => r.Id == reconciliationRecordId
                && (r.Status == ReconciliationStatus.Pending
                || r.Status == ReconciliationStatus.RetryScheduled))
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Status, ReconciliationStatus.Processing)
                .SetProperty(r => r.UpdatedAt, DateTime.UtcNow));

        if (updated == 0) return;

        var record = await _db.ReconciliationRecords
            .Include(r => r.InternalRecord)
            .FirstOrDefaultAsync(r => r.Id == reconciliationRecordId);

        if (record == null) return;

_db.ChangeTracker.Clear();
        if (record.Status != ReconciliationStatus.Processing)
            record.MarkAsProcessing();

        _logger.LogInformation(
            "Processing reconciliation {ReconciliationId} CorrelationId {CorrelationId}",
            record.Id, record.CorrelationId);

        try
        {
            var externalRecord = await _externalClient
                .GetExternalRecordAsync(record.InternalRecord!.TransactionId);

            if (externalRecord == null)
            {
                HandleRetryOrDeadLetter(record, "External record unavailable");
                await _db.SaveChangesAsync();
                return;
            }

            var result = _comparator.Compare(record.InternalRecord, externalRecord);

            if (result.Status == RecordCompare.Match)
            {
                record.MarkAsMatched();
                _logger.LogInformation(
                    "Matched {TransactionId}",
                    record.InternalRecord.TransactionId);
            }
            else if (result.Status == RecordCompare.Mismatch)
            {
                record.MarkAsMismatch(result.Reason ?? "Unknown mismatch");
                _logger.LogWarning(
                    "Mismatch {TransactionId} Reason {Reason}",
                    record.InternalRecord.TransactionId, result.Reason);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "External system error for record {ReconciliationId}",
                record.Id);

            HandleRetryOrDeadLetter(record, ex.Message);
        }

        await _db.SaveChangesAsync();
    }

    private void HandleRetryOrDeadLetter(ReconciliationRecord record, string reason)
    {
        if (record.RetryCount >= record.MaxRetryCount)
        {
            record.MarkAsDeadLettered(reason);
            _logger.LogError(
                "DeadLettered {ReconciliationId} after max retries", record.Id);
        }
        else
        {
            var backoff = TimeSpan.FromMinutes(Math.Pow(2, record.RetryCount + 1));
            record.MarkAsRetryScheduled(DateTime.UtcNow.Add(backoff));
            _logger.LogWarning(
                "Retry scheduled {ReconciliationId} attempt {RetryCount}",
                record.Id, record.RetryCount);
        }
    }
}