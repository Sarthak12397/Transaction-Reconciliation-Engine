public enum ExternalPaymentStatus
{
    Pending,
    Processing,
    Matched,
    Mismatch,
    RetryScheduled,
    Failed,
    DeadLettered
}