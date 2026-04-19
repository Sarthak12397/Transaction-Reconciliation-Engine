public class FakeExternalClientSystem : IExternalSystemClient
{
    private static readonly Dictionary<Guid, ExternalRecord?> _store = new();

    public async Task<ExternalRecord?> GetExternalRecordAsync(Guid transactionId)
    {
        await Task.Delay(100);

        if (_store.TryGetValue(transactionId, out var cached))
            return cached;

        var random = Random.Shared.Next(1, 5);

        if (random == 1)
        {
            _store[transactionId] = null;
            return null;
        }

        if (random == 2)
            throw new Exception("eSewa timeout");

        if (random == 3)
        {
            var result = new ExternalRecord(
                senderId: Guid.NewGuid(),
                receiverId: Guid.NewGuid(),
                transactionId: transactionId,
                amount: 20m,
                currency: "NPR",
                status: ExternalPaymentStatus.Failed,
                processedAt: DateTime.UtcNow);

            _store[transactionId] = result;
            return result;
        }

        var matchResult = new ExternalRecord(
            senderId: Guid.NewGuid(),
            receiverId: Guid.NewGuid(),
            transactionId: transactionId,
            amount: 30m,
            currency: "NPR",
            status: ExternalPaymentStatus.Success,
            processedAt: DateTime.UtcNow);

        _store[transactionId] = matchResult;
        return matchResult;
    }
}