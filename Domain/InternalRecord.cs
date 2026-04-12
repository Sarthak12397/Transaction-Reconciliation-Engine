public class InternalRecord
{
    public Guid Id
    {
        get; private set;
    
    }
    public Guid SenderId
    {
        get; private set;
    }
    public Guid ReceiverId 
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

    public InternalPaymentStatus Status
    {
        get; private set;
    }

 public DateTime CreatedAt
    {
        get;private set;
    }



    



    

public InternalRecord(
    Guid senderId,
    Guid receiverId,
    Guid transactionId,
    decimal amount, InternalPaymentStatus status,
    string currency)
{
    Id = Guid.NewGuid();
    SenderId = senderId;
    ReceiverId  = receiverId; 
    TransactionId = transactionId;
    Amount = amount;
    Currency = currency;
    Status =  InternalPaymentStatus.Pending;;
    CreatedAt = DateTime.UtcNow;
}

}

