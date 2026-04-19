using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class AuditInterceptor : SaveChangesInterceptor
{
   public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        
 if (eventData.Context is null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var changes = eventData.Context.ChangeTracker.Entries<ReconciliationRecord>()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        foreach (var entry in changes)
        {
            var original = entry.OriginalValues.GetValue<ReconciliationStatus>("Status");
            var current = entry.CurrentValues.GetValue<ReconciliationStatus>("Status");

            if (original != current)
            {
                var log = new ReconciliationAuditLog(
                    reconciliationRecordId: entry.Entity.Id,
                    correlationId: entry.Entity.CorrelationId,
                    fromState: original,
                    toState: current,
                    retryCount: entry.Entity.RetryCount,
                    reason: entry.Entity.FailureReason
                );

                eventData.Context.Set<ReconciliationAuditLog>().Add(log);
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    }
