public enum StateMachine
{
    Pending,
    Processing,
    Matched,
    Mismatch,
    RetryScheduled,
    Failed,
    DeadLettered
}