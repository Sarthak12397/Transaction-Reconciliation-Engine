using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ReconciliationController: ControllerBase
{
        private readonly ReconciliationService _reconciliationService;
        private readonly AppDbContext _db; 
        public ReconciliationController(ReconciliationService service, AppDbContext db)
    {
                _reconciliationService = service;
                _db = db;

    }
    [HttpPost("{id}/requeue")]
      public async Task<IActionResult> Requeue(Guid id)

    {
        var existing = await _db.ReconciliationRecords.FirstOrDefaultAsync(t=> t.Id == id);

        if(existing == null) 

return NotFound($"{id} not found");

        if (existing.Status != ReconciliationStatus.DeadLettered)
        
return BadRequest("Record is not DeadLettered");

             existing.MarkAsPending();
await _db.SaveChangesAsync();
return Ok();
            
        
        

     }
     [HttpPost("seed")]
public async Task<IActionResult> Seed()
{
    var transactionId = Guid.NewGuid();
    var senderId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();

    var internalRecord = new InternalRecord(
        senderId,
        receiverId,
        transactionId,
        30m,
        "NPR");

    var externalRecord = new ExternalRecord(
        senderId,
        receiverId,
        transactionId,
        20m,          // ← different amount = mismatch
        "NPR",
        ExternalPaymentStatus.Success,
        DateTime.UtcNow);

    await _db.InternalRecords.AddAsync(internalRecord);
    await _db.ExternalRecords.AddAsync(externalRecord);

    var reconciliationRecord = new ReconciliationRecord(
        internalRecord.Id,
        externalRecord.Id);

    await _db.ReconciliationRecords.AddAsync(reconciliationRecord);
    await _db.SaveChangesAsync();

    return Ok(new {
        ReconciliationId = reconciliationRecord.Id,
        TransactionId = transactionId,
        InternalAmount = 30m,
        ExternalAmount = 20m,
        Message = "Seed created. Hangfire will process within 1 minute."
    });
}


}