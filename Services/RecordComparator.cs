
public class ComparisonResult
{
    public InternalRecord InternalRecord {get; private set;}
    public ExternalRecord ExternalRecord {get; private set;}
     public Guid TransactionId{get; private set;}

     public RecordCompare Status {get; private set;}
     public string? Reason{get; private set;}


     public ComparisonResult(
        InternalRecord internalrecord,
        ExternalRecord externalrecords,
        Guid transactionid,
        RecordCompare status,
        String? reason
     )
    {
       InternalRecord = internalrecord;
       ExternalRecord = externalrecords; 
       TransactionId = transactionid;
       Status = status;
       Reason = reason;
    }

}