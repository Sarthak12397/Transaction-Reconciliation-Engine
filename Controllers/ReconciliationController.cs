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

         throw new KeyNotFoundException($" {id} not found");

        if (existing.Status != ReconciliationStatus.DeadLettered)
        
             BadRequest("Not working");

             existing.MarkAsPending();
await _db.SaveChangesAsync();
return Ok();
            
        
        

     }


}