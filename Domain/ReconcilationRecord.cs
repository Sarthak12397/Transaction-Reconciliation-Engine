public class ReconcilationRecord
{
    public Guid Id
    {
        get; private set;
    }

    public InternalRecord InternalRecordid
    {
        get; private set;
    }

    public ExternalRecord ExternalRecord
    {
        get; private set;
    }

    public int RetryCount
    {
        get;
        private set;
    }


    public int NextRetryAt
    {
        get;
        private set;
    }
    public DateTime? LastAttemptedAt{get; private set;}

    public String? FailureReason
    {
        get;private set;
    }
    public DateTime CreatedAt
    {
        get; private set;
    }

}