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
    // 1. Find record
    var existing = await _db.ReconciliationRecords
        .FirstOrDefaultAsync(r => r.Id == id);

    if (existing == null)
        return NotFound($"{id} not found");

    if (existing.Status != ReconciliationStatus.DeadLettered)
        return BadRequest("Only dead-lettered records can be requeued");

    existing.MarkAsPending();

    // 5. Persist
    await _db.SaveChangesAsync();

  
    await _reconciliationService.ProcessAsync(existing.Id);

    return Ok(new
    {
        Message = "Requeued successfully",
        Id = existing.Id,
        Status = existing.Status
    });
}

     //Demo
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
[HttpGet("list/dead-lettered")]
public async Task<IActionResult> GetDeadLettered()
{
    var records = await _db.ReconciliationRecords
        .Where(r => r.Status == ReconciliationStatus.DeadLettered)
        .ToListAsync();
    return Ok(records);
}

}