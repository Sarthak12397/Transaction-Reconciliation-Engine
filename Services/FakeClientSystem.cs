

public class FakeExternalClientSystem: IExternalSystemClient
{
    public async Task<ExternalRecord?> GetExternalRecordAsync(Guid transactionId)
    {
        var random = new Random().Next(1,5);

        if(random == 1)
        {
            
            return RecordCompare.Unavailable;
        }

        if(random == 2)
        {
              throw new Exception("Timeout");

      
        }
        if(random == 3)
        {
                       return new ExternalRecord(
              senderId:Guid.NewGuid(),
              receiverId: Guid.NewGuid(),
              transactionId:transactionId,
              amount: 20m,
              currency: "NPR",
              status: ExternalPaymentStatus.Failed,
              processedAt: DateTime.UtcNow


            );

        }
        if(random == 4)
        {
                       return new ExternalRecord(
              senderId:Guid.NewGuid(),
              receiverId: Guid.NewGuid(),
              transactionId:transactionId,
              amount: 30m,
              currency: "NPR",
              status: ExternalPaymentStatus.Success,
              processedAt: DateTime.UtcNow


            );
        }
                       return new ExternalRecord(
              senderId:Guid.NewGuid(),
              receiverId: Guid.NewGuid(),
              transactionId:transactionId,
              amount: 60m,
              currency: "NPR",
              status: ExternalPaymentStatus.NotFound,
              processedAt: DateTime.UtcNow


            );

        




    }
}