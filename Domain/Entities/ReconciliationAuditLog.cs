public class ReconciliationAuditLog
{
    
    public Guid Id {get; private set;}
    public Guid ReconciliationRecordId { get; private set; }
public Guid CorrelationId { get; private set; }

    public ReconciliationStatus FromState {get; private set;}
    public ReconciliationStatus ToState {get; private set;}
    public DateTime CreatedAt {get; private set;}
    public string? Reason {get; private set;}
    public int RetryCount{get; private set;}
    public ReconciliationAuditLog(
    Guid reconciliationRecordId,
    Guid correlationId,
    ReconciliationStatus fromState,
    ReconciliationStatus toState,
    int retryCount,
    string? reason)
{
    Id = Guid.NewGuid();
    ReconciliationRecordId = reconciliationRecordId;
    CorrelationId = correlationId;
    FromState = fromState;
    ToState = toState;
    RetryCount = retryCount;
    Reason = reason;
    CreatedAt = DateTime.UtcNow;
}

}