public class ReconciliationRecord
{
    public Guid Id
    {
        get; private set;
    }
public Guid InternalRecordId { get; private set; }

    public InternalRecord? InternalRecord
    {
        get; private set;
    }
public Guid ExternalRecordId { get; private set; }

    public ExternalRecord? ExternalRecord
    {
        get; private set;
    }
public ReconciliationStatus Status { get; private set; 

}
    public int RetryCount
    {
        get;
        private set;
    }


    public DateTime? NextRetryAt
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
    public DateTime UpdatedAt { get; private set; }
    public int MaxRetryCount { get; private set; } = 3;

public DateTime? ResolvedAt { get; private set; }
public Guid CorrelationId { get; private set; }

public ReconciliationRecord(
    Guid internalRecordId,
    Guid externalRecordId)
{
    Id = Guid.NewGuid();
    InternalRecordId = internalRecordId;
    ExternalRecordId = externalRecordId;
    Status = ReconciliationStatus.Pending;
    RetryCount = 0;
    CreatedAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
    CorrelationId = Guid.NewGuid();

}




    public void MarkAsProcessing()
    {
        if(Status != ReconciliationStatus.Pending && Status != ReconciliationStatus.RetryScheduled)
        {
            throw new InvalidOperationException($"Cannot transition to Processing from {Status}");


        }
         LastAttemptedAt = DateTime.UtcNow;

        Status = ReconciliationStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }


    
   public void MarkAsMatched()
    {
        if(Status != ReconciliationStatus.Processing)
        {

        throw new InvalidOperationException($"Cannot mark as matched from state {Status}");

        }

        Status = ReconciliationStatus.Matched;
            LastAttemptedAt = DateTime.UtcNow;
         NextRetryAt = null;
         FailureReason = null;

        ResolvedAt = DateTime.UtcNow;
         UpdatedAt = DateTime.UtcNow;
    }
    
    public void MarkAsMismatch(string reason)
    {
        if(Status != ReconciliationStatus.Processing)
        {
                    throw new InvalidOperationException($"Cannot mark as mismatch from state {Status}");

        }

        FailureReason = reason;
        Status = ReconciliationStatus.Mismatch;
         LastAttemptedAt = DateTime.UtcNow;
         ResolvedAt = DateTime.UtcNow;
           UpdatedAt = DateTime.UtcNow;
    }


   public void MarkAsRetryScheduled(DateTime nextRetryAt)
    {
        if (Status != ReconciliationStatus.Processing)
        {
            throw new InvalidOperationException(
                $"Cannot schedule retry from state {Status}");
        }

 if (RetryCount >= MaxRetryCount)
        throw new InvalidOperationException("Max retries reached. Use MarkAsDeadLettered.");

        RetryCount++;

        Status = ReconciliationStatus.RetryScheduled;
        LastAttemptedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        NextRetryAt = nextRetryAt;
    }

    public void MarkAsDeadLettered(string reason)
    {
            if (Status != ReconciliationStatus.Processing)
    {
        throw new InvalidOperationException($"Cannot dead-letter from state {Status}");

    }
      if (RetryCount < MaxRetryCount)
    {
        throw new InvalidOperationException("Cannot dead-letter before max retries");
    }
        FailureReason =reason;
        Status = ReconciliationStatus.DeadLettered;
         LastAttemptedAt = DateTime.UtcNow;
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }


    public void MarkAsFailed(string reason)
    {
        if(Status != ReconciliationStatus.Processing)
        {
                    throw new InvalidOperationException($"Cannot Fail from {Status}");

        }
        Status = ReconciliationStatus.Failed;
        FailureReason =reason;
        LastAttemptedAt = DateTime.UtcNow;
        
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }


   
public void MarkAsPending()
{
    if (Status != ReconciliationStatus.DeadLettered)
        throw new InvalidOperationException($"Cannot requeue from {Status}");

    Status = ReconciliationStatus.Pending;
    RetryCount = 0;
    FailureReason = null;
    NextRetryAt = null;
    UpdatedAt = DateTime.UtcNow;
}


}