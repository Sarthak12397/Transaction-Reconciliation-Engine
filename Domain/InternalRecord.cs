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


    

}