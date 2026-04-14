public enum ReconciliationStatus
{
    Pending,
    Processing,
    Matched,
    Mismatch,
    RetryScheduled,
    Failed,
    DeadLettered
}