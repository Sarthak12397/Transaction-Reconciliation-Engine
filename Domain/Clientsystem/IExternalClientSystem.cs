public interface IExternalSystemClient

{
    Task<ExternalRecord?> GetExternalRecordAsync(  Guid transactionId);
}