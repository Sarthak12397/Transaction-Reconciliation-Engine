# Data Reconciliation / Consistency Monitor

A backend system that detects when two financial systems 
disagree — and resolves the gap automatically through 
retries, failure classification, and dead-letter handling.

> Built with .NET 10 · PostgreSQL · Hangfire · Docker · Serilog
> Part of a two-system fintech backend portfolio.  
> See also: [Payment Processing System](https://github.com/Sarthak12397/TransactionalBusinessAPI) 
> — the write-side that generates the transactions this engine reconciles.


## System Summary

This system continuously reconciles internal financial 
records with an external payment system (eSewa simulation). 
It ensures eventual consistency through:

- Continuous polling (Hangfire jobs)
- Strict state machine transitions
- Idempotent external interactions
- Retry + dead-letter failure handling
- Full audit trail of every state change

## Problem

Financial systems maintain records across multiple sources.
When your internal database disagrees with an external
system (eSewa, bank, payment gateway) — that gap is real money:

- Your system says NPR 30 completed. eSewa says NPR 20.
- Your system says completed. The bank says never received.
- A timeout leaves the comparison in an unknown state.
- Nobody notices until the audit fails.

Without active reconciliation, these gaps silently accumulate.

## Solution

- **State machine** — every reconciliation attempt moves through
  strict, controlled states. No ambiguity. No illegal jumps.
- **Idempotent external client** — same TransactionId always
  returns the same external state. Requeue never triggers
  duplicate external actions.
- **Failure classification** — transient failures retried safely.
  Permanent failures dead-lettered immediately.
- **Audit trail** — every state change written to DB with
  CorrelationId for full traceability.
- **Read-based reconciliation** — external system is queried 
  for state, not re-triggered. Requeue never causes duplicate 
  external actions.
  
## Reconciliation Lifecycle
<img width="2027" height="940" alt="mermaid-diagram (4)" src="https://github.com/user-attachments/assets/9eee5378-4191-4af9-acff-13f0f734adcf" />

## Architecture Diagram
<img width="2119" height="2418" alt="mermaid" src="https://github.com/user-attachments/assets/8c91e41d-3b54-4ba7-bb15-eecd1f00b4e4" />


## Example Flow

### 1. Seed test data
```json
POST /api/reconciliation/seed
```
```json
{
  "reconciliationId": "f42ceeaf-...",
  "transactionId": "133748b4-...",
  "internalAmount": 30,
  "externalAmount": 20,
  "message": "Seed created. Hangfire will process within 1 minute."
}
```
### 2. Hangfire picks it up automatically

Pending → Processing → calls eSewa (FakeExternalSystemClient)

### 3. If amounts match:

Processing → Matched → Done 

### 4. If amounts differ:

Processing → Mismatch → Flagged for human review 

### 5. If external system unreachable:

Processing → RetryScheduled (attempt 1, backoff 2min)

→ Processing → RetryScheduled (attempt 2, backoff 4min)

→ Processing → RetryScheduled (attempt 3, backoff 8min)

→ DeadLettered 

### 6. Manual requeue:

POST /api/reconciliation/{id}/requeue

→ DeadLettered → Pending → cycle restarts

## Failure Handling

| Scenario | Classification | Behavior |
|----------|---------------|----------|
| External timeout | Transient | Retry with exponential backoff |
| External unavailable | Transient | Retry with exponential backoff |
| Amount mismatch | Mismatch | Flagged for human review |
| Max retries hit | Permanent | Dead-lettered |
| Stuck in Processing | — | StuckRecoveryJob rescues it |
| Duplicate worker claim | — | Blocked via atomic DB lock |


## Background Jobs

| Job | Schedule | Purpose |
|-----|----------|---------|
| PeriodicScanJob | Every minute | Finds Pending records |
| RetryJob | Every minute | Processes due retries |
| StuckRecoveryJob | Every 5 minutes | Rescues stuck Processing |


## How to Run

### With Docker (recommended):
```bash
git clone https://github.com/Sarthak12397/Transaction-Reconciliation-Engine.git
cd Transaction-Reconciliation-Engine
docker-compose up --build
```

API: `http://localhost:8080`
Hangfire Dashboard: `http://localhost:8080/hangfire`

### Without Docker:
Configure `appsettings.json` with your PostgreSQL connection string.

```bash
dotnet restore
dotnet ef database update
dotnet run
```

API: `http://localhost:5278`
Hangfire Dashboard: `http://localhost:5278/hangfire`


## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/reconciliation/seed` | Create test record |
| POST | `/api/reconciliation/{id}/requeue` | Re-queue dead-lettered record |
| GET | `/api/reconciliation/list/dead-lettered` | List dead-lettered records |


## Design Decisions

| Decision | Why |
|----------|-----|
| State machine | Deterministic flow — no ambiguous states |
| Idempotent external client | Same TransactionId = same answer. No duplicate actions |
| Dead-letter table | Failed records visible, queryable, recoverable |
| Atomic DB lock | Two workers cannot process same record simultaneously |
| Audit trail (DB-level) | Every state change persisted — not just logged |
| Hangfire | Persistent jobs — retries survive restarts |
| Serilog + CorrelationId | Full traceability across retries and state changes |



## What I'd Improve

| Improvement | Reason | Priority |
|-------------|--------|----------|
| Real eSewa/bank adapter | Replace fake client with actual API | High |
| JWT Authentication | Secure requeue endpoint — ops team only | High |
| Webhook support | External systems push updates — no polling | High |
| Mismatch resolution workflow | Ops team reviews and resolves gaps | Medium |
| OpenTelemetry | Distributed tracing beyond CorrelationId | Medium |



## Tech Stack

| Technology | Purpose |
|------------|---------|
| .NET 10 / ASP.NET Core | API framework |
| PostgreSQL | Primary database |
| Entity Framework Core | ORM + migrations |
| Hangfire | Background job scheduling |
| Serilog | Structured logging |
| xUnit + FluentAssertions | 9 unit tests |
| Docker + docker-compose | Containerization |


