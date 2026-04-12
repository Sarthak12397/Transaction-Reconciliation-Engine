public class ExternalRecord
{
    public Guid Id { get; private set; }

    public Guid SenderId { get; private set; }

    public Guid ReceiverId { get; private set; }

    public Guid TransactionId { get; private set; }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; }

    public ExternalPaymentStatus Status { get; private set; }

    public DateTime ProcessedAt { get; private set; }

    public ExternalRecord(
        Guid senderId,
        Guid receiverId,
        Guid transactionId,
        decimal amount,
        string currency,
        DateTime processedAt)
    {
        Id = Guid.NewGuid();
        SenderId = senderId;
        ReceiverId = receiverId;
        TransactionId = transactionId;
        Amount = amount;
        Currency = currency;
        Status = ExternalPaymentStatus.Pending;
        ProcessedAt = processedAt;
    }
}