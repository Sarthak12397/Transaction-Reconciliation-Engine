public class InternalRecord
{
    public Guid id
    {
        get; private set;
    
    }
    public Guid senderId
    {
        get; private set;
    }
    public Guid RecieverId
    {
        get;private set;
    
    }
    public Guid TransactionId { get; private set; } 
    public decimal Amount
    {
        get; private set;
    }
    public string Currency
    {
        get;private set;
    }

 public DateTime CreatedAt
    {
        get;private set;
    }



    

    public InternalRecord(
        Guid SenderId,
        Guid recieverId,
        Guid transactionId,
        decimal amount,
        string currency)
    {
        id = Guid.NewGuid(); 
        senderId = SenderId;
        RecieverId = recieverId;
        TransactionId = transactionId;
        Amount = amount;
        Currency = currency;
        CreatedAt = DateTime.UtcNow;
    }

}